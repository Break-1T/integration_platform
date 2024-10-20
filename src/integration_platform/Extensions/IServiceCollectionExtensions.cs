using integration_platform.Constant;
using integration_platform.database.Constants;
using integration_platform.database.Options;
using integration_platform.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using System;

namespace integration_platform.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddIntegratorPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        var schedulerSettingsSection = configuration.GetSection(IntegratorConstants.SchedulerSettingsSectionKey);
        var dbSection = configuration.GetSection(DatabaseConstants.DatabaseSectionKey);

        services.AddOptions<SchedulerSettings>()
            .Bind(schedulerSettingsSection)
            .ValidateOnStart()
            .ValidateDataAnnotations();

        var schedulerSettings = schedulerSettingsSection.Get<SchedulerSettings>();
        var dbSettings = dbSection.Get<DatabaseSettings>();
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
                    postgresConfig.ConnectionString = dbSettings.ConnectionString;
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
