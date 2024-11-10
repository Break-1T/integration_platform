using integration_platform.Models;
using integration_platform.utils.classes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace integration_platform.Interfaces;

public interface IJobsService
{
    /// <summary>
    /// Updates the job asynchronous.
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="jobGroup">The job group.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="cronSchedule">The cron schedule.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<ServiceResult> UpdateJobAsync(string jobName, string jobGroup, Dictionary<string, object> settings, string cronSchedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the jobs asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<ServiceResult<List<IntegratorJob>>> GetJobsAsync(CancellationToken cancellationToken = default);
}
