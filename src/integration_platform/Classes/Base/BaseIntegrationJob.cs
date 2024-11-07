using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace integration_platform.Classes.Base;

public abstract class BaseIntegrationJob : IJob
{
    /// <summary>
    /// Represents the name of the setting for the maximum count of threads to be used during the task.
    /// </summary>
    public const string MaxThreadCountSettingName = "Max Threads";
    protected BaseIntegrationJob(ILogger logger)
    {
        Logger = logger;
    }

    public JobDataMap JobSettings { get; set; }
    public ILogger Logger { get; }
    public int MaxThreads { get; private set; }

    public virtual void InitDefaultJobSettings(JobDataMap settings)
    {
        settings.Put(MaxThreadCountSettingName, 10);
    }

    public virtual Task<bool> BeforeExecutingAsync(CancellationToken cancellationToken = default)
    {
        var result = true;

        MaxThreads = JobSettings.GetInt(MaxThreadCountSettingName);

        return Task.FromResult(result);
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            JobSettings = context.MergedJobDataMap;

            if (!ValidateHandlerSettingsAsync(context.CancellationToken))
            {
                throw new JobExecutionException("Job Settings is invalid.");
            }

            if (!await BeforeExecutingAsync(context.CancellationToken))
            {
                throw new JobExecutionException("Pre Execution Validation Finished: Failed.");
            }

            var executeJob = await ExecuteJobAsync(context.CancellationToken);

            if (!executeJob)
            {
                throw new JobExecutionException("Job Settings is invalid.");
            }

        }
        catch (Exception e)
        {
            throw new JobExecutionException(e.Message, e, e is OutOfMemoryException);
        }
    }

    public abstract Task<bool> ExecuteJobAsync(CancellationToken cancellationToken = default);
    public virtual bool ValidateHandlerSettingsAsync(CancellationToken cancellationToken = default)
    {
        return true;
    }
}
