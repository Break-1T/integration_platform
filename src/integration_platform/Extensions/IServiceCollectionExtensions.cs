using integration_platform.Classes.Base;
using integration_platform.Constant;
using integration_platform.database.Options;
using integration_platform.Interfaces;
using integration_platform.Options;
using integration_platform.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.AspNetCore;
using System;

namespace integration_platform.Extensions;

public static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddIntegratorPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        var schedulerSettingsSection = configuration.GetSection(IntegratorConstants.SchedulerSettingsSectionKey);
        var schedulerSettings = schedulerSettingsSection.Get<SchedulerSettings>() ?? new SchedulerSettings();

        services.AddOptions<SchedulerSettings>()
            .Configure(x=> x = schedulerSettings)
            .ValidateOnStart()
            .ValidateDataAnnotations();

        var dbSettings = new DatabaseSettings();

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
            dbSettings = options.Value;
        }

        services.AddQuartz(option =>
        {
            option.SchedulerId = Environment.MachineName;
            option.SchedulerName = Environment.GetEnvironmentVariable(IntegratorConstants.WorkerNameEnvVariable);
            option.MaxBatchSize = schedulerSettings.MaxRunTasks;
            option.UseDefaultThreadPool(maxConcurrency: schedulerSettings.MaxRunTasks);

            option.UsePersistentStore(storeConfig =>
            {
                storeConfig.RetryInterval = TimeSpan.FromSeconds(60);
                //storeConfig.UseProperties = true;

                storeConfig.UsePostgres(postgresConfig =>
                {
                    postgresConfig.TablePrefix = "scheduler.QRTZ_";
                    postgresConfig.ConnectionString = dbSettings.GetConnectionString();
                });

                storeConfig.UseNewtonsoftJsonSerializer();
            });
        });

        services.AddQuartzServer(o =>
        {
            o.WaitForJobsToComplete = true;
            o.AwaitApplicationStarted = true;
        });

        services.AddScoped<IJobsService, JobsService>();

        return services;
    }
    
    public static IServiceCollection AddIntegratorJob<TJob>(this IServiceCollection services) 
        where TJob : BaseIntegrationJob
    {
        return services
            .AddScoped(typeof(TJob), typeof(TJob))
            .AddTransient(typeof(TJob), typeof(TJob));
    }
}
