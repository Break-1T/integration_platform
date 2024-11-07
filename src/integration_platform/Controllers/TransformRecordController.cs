using integration_platform.database.Filters;
using integration_platform.database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace integration_platform.Controllers;

[ApiController]
[Route("[controller]")]
public class TransformRecordController(ITransformRecordStore transformRecordStore) : ControllerBase
{
    /// <summary>
    /// Gets the transform records.
    /// </summary>
    /// <param name="filter">The filter.</param>
    [HttpPost("get-record-transfers")]
    public async Task<IActionResult> GetTransformRecords([FromBody] TransformRecordFilter filter)
    {
        var result = await transformRecordStore.GetTransformRecordsAsync(filter);

        if (!result.IsSuccess)
        {
            return this.BadRequest(result.ErrorMessage);
        }

        return Ok(result.Result);
    }
}
