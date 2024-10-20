using Quartz;
using System;
using System.Threading.Tasks;

namespace integration_platform.Classes;

public abstract class BaseIntegrationJob : IJob
{
    public JobDataMap JobSettings { get; set; }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            this.JobSettings = context.MergedJobDataMap;

            if (!ValidateHandlerSettings())
            {
                throw new JobExecutionException("Job Settings is invalid.");
            }

            var executeJob = await ExecuteJob();

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

    public abstract Task<bool> ExecuteJob();

    public virtual bool ValidateHandlerSettings()
    {
        return true;
    }
}
