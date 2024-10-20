using integration_platform.Classes;
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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>IServiceProvider.</returns>
    public static async Task<IServiceProvider> AddIntegratorJob<TJob>(
        this IServiceProvider serviceProvider, 
        string jobName, 
        string jobGroup, 
        string cronSchedule,
        JobDataMap jobDataMap,
        CancellationToken cancellationToken = default)
        where TJob : BaseIntegrationJob
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var schedFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

        var scheduler = await schedFactory.GetScheduler(cancellationToken);
        
        var jobKey = JobKey.Create(jobName, jobGroup);

        var jobDetail = JobBuilder
            .Create<TJob>()
            .UsingJobData(jobDataMap)
            .WithIdentity(jobKey)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(jobName,jobGroup)
            .WithCronSchedule(cronSchedule)
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);

        return serviceProvider;
    }
}
