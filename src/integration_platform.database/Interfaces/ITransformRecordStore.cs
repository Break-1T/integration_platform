using integration_platform.database.Filters;
using integration_platform.database.Models;
using integration_platform.utils.classes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace integration_platform.database.Interfaces;

/// <summary>
/// ITransformRecordStore.
/// </summary>
public interface ITransformRecordStore
{
    /// <summary>
    /// Adds the transform record asynchronous.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult> AddTransformRecordAsync(TransformRecord entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the transform records asynchronous.
    /// </summary>
    /// <param name="records">The records.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult> AddTransformRecordsAsync(List<TransformRecord> records, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the transform record asynchronous.
    /// </summary>
    /// <param name="transformRecordId">The transform record identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task<ServiceResult<TransformRecord>> GetTransformRecordAsync(long transformRecordId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the transform records asynchronous.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<ServiceResult<IPagedList<TransformRecord>>> GetTransformRecordsAsync(TransformRecordFilter filter, CancellationToken cancellationToken = default);
}
