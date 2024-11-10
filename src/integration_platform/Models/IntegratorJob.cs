using System.Collections.Generic;

namespace integration_platform.Models;

/// <summary>
/// IntegratorJob.
/// </summary>
public class IntegratorJob
{
    /// <summary>
    /// Gets or sets the name of the job.
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// Gets or sets the job group.
    /// </summary>
    public string JobGroup { get; set; }

    /// <summary>
    /// Gets or sets the cron schedule.
    /// </summary>
    public string CronSchedule { get; set; }

    /// <summary>
    /// Gets or sets the settings.
    /// </summary>
    public Dictionary<string, object> Settings { get; set; }
}
