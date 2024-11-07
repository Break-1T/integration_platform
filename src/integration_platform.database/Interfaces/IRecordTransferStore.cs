using integration_platform.database.Filters;
using integration_platform.database.Models;
using integration_platform.utils.classes;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace integration_platform.database.Interfaces;

/// <summary>
/// IRecordTransferStore.
/// </summary>
public interface IRecordTransferStore
{
    /// <summary>
    /// Adds the record transfer asynchronous.
    /// </summary>
    /// <param name="recordTransfer">The record transfer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult> AddRecordTransferAsync(RecordTransfer recordTransfer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the record transfer asynchronous.
    /// </summary>
    /// <param name="recordTransfer">The record transfer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult> UpdateRecordTransferAsync(RecordTransfer recordTransfer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the record transfer asynchronous.
    /// </summary>
    /// <param name="recordTransferId">The record transfer identifier.</param>
    /// <param name="includeContent">if set to <c>true</c> [include content].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult<RecordTransfer>> GetRecordTransferAsync(long? recordTransferId, bool includeContent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the record transfers asynchronous.
    /// </summary>
    /// <param name="recordTransferFilter">The record transfer filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult<IPagedList<RecordTransfer>>> GetRecordTransfersAsync(RecordTransferFilter recordTransferFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the record transfer ids asynchronous.
    /// </summary>
    /// <param name="recordTransferFilter">The record transfer filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult<long?[]>> GetRecordTransferIdsAsync(RecordTransferFilter recordTransferFilter, CancellationToken cancellationToken = default);
}
