using integration_platform.Classes.Base;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
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
        JobDataMap jobDataMap,
        bool isEnabled = false,
        TimeZoneInfo timeZoneInfo = null,
        CancellationToken cancellationToken = default)
        where TJob : BaseIntegrationJob
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var schedFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var integrationJob = scope.ServiceProvider.GetRequiredService<TJob>();

        var scheduler = await schedFactory.GetScheduler(cancellationToken);
        
        var jobKey = JobKey.Create(jobName, jobGroup);
        var triggerKey = new TriggerKey(jobName, jobGroup);

        var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);

        if (jobDetail != null)
        {
            integrationJob.InitDefaultJobSettings(jobDetail.JobDataMap);
            jobDetail = jobDetail.GetJobBuilder().UsingJobData(jobDataMap).Build();
            
        }
        else
        {
            jobDetail = JobBuilder
                .Create<TJob>()
                .UsingJobData(jobDataMap)
                .WithIdentity(jobKey)
                .Build();
        }
        await scheduler.AddJob(jobDetail, true, cancellationToken);

        var trigger = await scheduler.GetTrigger(triggerKey);

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
                .UsingJobData(jobDataMap);

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
                .UsingJobData(jobDataMap)
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
