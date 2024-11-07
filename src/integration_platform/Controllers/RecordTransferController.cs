using integration_platform.database.Filters;
using integration_platform.database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace integration_platform.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordTransferController(IRecordTransferStore recordTransferStore) : ControllerBase
{
    /// <summary>
    /// Gets the record transfers.
    /// </summary>
    /// <param name="recordTransferFilter">The record transfer filter.</param>
    [HttpPost("get-record-transfers")]
    public async Task<IActionResult> GetRecordTransfers([FromBody] RecordTransferFilter recordTransferFilter)
    {
        var result = await recordTransferStore.GetRecordTransfersAsync(recordTransferFilter);

        if (!result.IsSuccess)
        {
            return this.BadRequest(result.ErrorMessage);
        }

        return Ok(result.Result);
    }
}
