using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using integration_platform.Interfaces;
using integration_platform.Models.RequestModels;

namespace integration_platform.Controllers;

/// <summary>
/// JobsController.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[ApiController]
[Route("[controller]")]
public class JobsController(IJobsService jobsService) : ControllerBase
{
    /// <summary>
    /// Updates the job.
    /// </summary>
    /// <param name="updateJobRequest">The update job request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost("update")]
    public async Task<IActionResult> UpdateJob(
        [FromBody] UpdateJobRequest updateJobRequest,
        CancellationToken cancellationToken = default)
    {
        var updateJobResult = await jobsService.UpdateJobAsync(
            updateJobRequest.JobName, 
            updateJobRequest.JobGroup, 
            updateJobRequest.Settings, 
            updateJobRequest.CronSchedule, 
            cancellationToken);

        if (!updateJobResult.IsSuccess)
        {
            return this.BadRequest(updateJobResult.ErrorMessage);
        }

        return this.Ok();
    }

    /// <summary>
    /// Gets the jobs.
    /// </summary>
    /// <response code="200">Success</response>
    [HttpGet("get-jobs")]
    public async Task<IActionResult> GetJobs(CancellationToken cancellationToken = default)
    {
        var getJobsResult = await jobsService.GetJobsAsync(cancellationToken);

        if (!getJobsResult.IsSuccess)
        {
            return this.BadRequest(getJobsResult.ErrorMessage);
        }

        return this.Ok(getJobsResult.Result);
    }
}
