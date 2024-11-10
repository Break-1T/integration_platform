using integration_platform.Interfaces;
using integration_platform.Models;
using integration_platform.utils.classes;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace integration_platform.Services;

public class JobsService(ILogger<JobsService> logger, ISchedulerFactory schedulerFactory) : IJobsService
{
    /// <inheritdoc/>
    public async Task<ServiceResult<List<IntegratorJob>>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
		try
		{
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

			var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

			var getJobsTasks = jobKeys.Select(async jobKey =>
			{
				var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);
				var trigger = await scheduler.GetTrigger(new TriggerKey(jobKey.Name, jobKey.Group), cancellationToken);
				return new IntegratorJob
				{
					JobName = jobDetail.Key.Name,
					JobGroup = jobDetail.Key.Group,
					CronSchedule = (trigger as ICronTrigger).CronExpressionString,
					Settings = jobDetail.JobDataMap.ToDictionary(),
				};
			});

			var jobs = (await Task.WhenAll(getJobsTasks)).Select(x=>x).ToList();

			return ServiceResult<List<IntegratorJob>>.FromSuccess(jobs);
		}
		catch (Exception e)
		{
            logger.LogError(e, e.Message);
            return ServiceResult<List<IntegratorJob>>.FromError(e.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult> UpdateJobAsync(string jobName, string jobGroup, Dictionary<string, object> settings, string cronSchedule, CancellationToken cancellationToken = default)
    {
		try
		{
			var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

			var jobDetail = await scheduler.GetJobDetail(JobKey.Create(jobName, jobGroup), cancellationToken);

			return ServiceResult.FromSuccess();
		}
		catch (Exception e)
		{
			logger.LogError(e, e.Message);
			return ServiceResult.FromError(e.Message);
		}
    }
}
