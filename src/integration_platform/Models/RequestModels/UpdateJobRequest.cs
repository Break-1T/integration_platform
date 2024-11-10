using System.Collections.Generic;

namespace integration_platform.Models.RequestModels;

/// <summary>
/// UpdateJobRequest.
/// </summary>
public class UpdateJobRequest
{
    public string JobName { get; set; }
    public string JobGroup { get; set; }
    public Dictionary<string, object> Settings { get; set; }
    public string CronSchedule { get; set; }
}
