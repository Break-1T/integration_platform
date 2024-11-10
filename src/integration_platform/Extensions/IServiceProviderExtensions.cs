using integration_platform.Classes.Base;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace integration_platform.Extensions;

public static class IServiceProviderExtensions
{
    /// <summary>
    /// Adds the integrator job.
    /// </summary>
    /// <typeparam name="TJob">The type of the job.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="jobGroup">The job group.</param>
    /// <param name="cronSchedule">The cron schedule.</param>
    /// <param name="jobDataMap">The job data map.</param>
    /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>IServiceProvider.</returns>
    public static async Task<IServiceProvider> AddIntegratorJobAsync<TJob>(
        this IServiceProvider serviceProvider, 
        string jobName, 
        string jobGroup, 
        string cronSchedule,
        Dictionary<string, object> settings,
        bool isEnabled = false,
        TimeZoneInfo timeZoneInfo = null,
        CancellationToken cancellationToken = default)
        where TJob : BaseIntegrationJob
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var schedFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var integrationJob = scope.ServiceProvider.GetRequiredService<TJob>();
        
        var defaultSettings = integrationJob.InitDefaultJobSettings();
        var jobSettings = defaultSettings.ToDictionary(key => key.Key, value =>
        {
            return settings.TryGetValue(value.Key, out var settingValue) ? settingValue : value.Value;
        }) as IDictionary<string, object>;
        
        var scheduler = await schedFactory.GetScheduler(cancellationToken);
        
        var jobKey = JobKey.Create(jobName, jobGroup);
        var triggerKey = new TriggerKey(jobName, jobGroup);

        var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);

        if (jobDetail != null)
        {
            foreach (var setting in jobSettings)
            {
                if (jobDetail.JobDataMap.TryGetValue(setting.Key, out var value))
                {
                    jobSettings[setting.Key] = value;
                }
            }

            jobDetail = jobDetail.GetJobBuilder().UsingJobData(new JobDataMap(jobSettings)).Build();
            
        }
        else
        {
            jobDetail = JobBuilder
                .Create<TJob>()
                .UsingJobData(new JobDataMap(jobSettings))
                .WithIdentity(jobKey)
                .StoreDurably()
                .Build();
        }
        await scheduler.AddJob(jobDetail, true, cancellationToken);

        var trigger = await scheduler.GetTrigger(triggerKey, cancellationToken);

        if (trigger != null)
        {
            var triggerBuilder = trigger
                .GetTriggerBuilder()
                .ForJob(jobDetail)
                .WithCronSchedule(
                    cronSchedule,
                    builder => builder
                        .InTimeZone(timeZoneInfo ?? TimeZoneInfo.Utc)
                        .WithMisfireHandlingInstructionDoNothing())
                .UsingJobData(new JobDataMap(jobSettings));

            if (!isEnabled)
            {
                triggerBuilder.StartAt(DateTimeOffset.UtcNow.AddSeconds(30));
            }

            trigger = triggerBuilder.Build();
            await scheduler.RescheduleJob(triggerKey, trigger, cancellationToken);

            if (!isEnabled)
            {
                await scheduler.PauseTrigger(triggerKey, cancellationToken);
            }
        }
        else
        {
            var triggerBuilder = TriggerBuilder
                .Create()
                .ForJob(jobDetail)
                .UsingJobData(new JobDataMap(jobSettings))
                .WithIdentity(jobName, jobGroup)
                .WithCronSchedule(
                    cronSchedule, 
                    builder => builder
                        .InTimeZone(timeZoneInfo ?? TimeZoneInfo.Utc)
                        .WithMisfireHandlingInstructionDoNothing());

            if (!isEnabled)
            {
                triggerBuilder.StartAt(DateTimeOffset.UtcNow.AddSeconds(30));
            }

            trigger = triggerBuilder.Build();

            await scheduler.ScheduleJob(trigger, cancellationToken);

            if (!isEnabled)
            {
                await scheduler.PauseTrigger(triggerKey, cancellationToken);
            }
        }

        return serviceProvider;
    }
}
