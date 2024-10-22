using integration_platform.Constant;
using integration_platform.database;
using integration_platform.database.Constants;
using integration_platform.database.Options;
using integration_platform.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.AspNetCore;
using System;

namespace integration_platform.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddIntegratorPlatform(this IServiceCollection services, IConfiguration configuration)
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
                storeConfig.UseProperties = true;

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

        return services;
    }
}
