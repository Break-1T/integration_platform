using integration_platform.database.Filters;
using integration_platform.database.Interfaces;
using integration_platform.database.Models;
using integration_platform.utils.classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EF;

namespace integration_platform.database.Stores;

/// <summary>
/// RecordTransferStore.
/// </summary>
/// <seealso cref="integration_platform.database.Interfaces.IRecordTransferStore" />
public class RecordTransferStore(ILogger<RecordTransferStore> logger, IDbContextFactory<IntegrationPlatformDbContext> dbContextFactory) : IRecordTransferStore
{
    private readonly ILogger<RecordTransferStore> _logger = logger;
    private readonly IDbContextFactory<IntegrationPlatformDbContext> _dbContextFactory = dbContextFactory;
	private IntegrationPlatformDbContext _dbContext => this._dbContextFactory.CreateDbContext();

    /// <inheritdoc/>
    public async Task<ServiceResult> AddRecordTransferAsync(RecordTransfer recordTransfer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(recordTransfer));

		try
		{
			this._dbContext.RecordTransfers.Add(recordTransfer);
			var result = await this._dbContext.SaveChangesAsync(cancellationToken);

			return result == 1
				? ServiceResult.FromSuccess()
				: ServiceResult.FromError($"Error: could not add record transfer with id: '{recordTransfer.RecordTransferId}'");
		}
		catch (Exception e)
		{
			this._logger.LogError(e, e.Message);
			return ServiceResult.FromError($"UnexpectedError: Could not add Record Transfer. Details: '{e.Message}'.");
		}
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<RecordTransfer>> GetRecordTransferAsync(long? recordTransferId, bool includeContent = false, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<RecordTransfer> query = this._dbContext.RecordTransfers;

            if (includeContent)
            {
                query = query.Include(x => x.RequestContent).Include(x=>x.ResponseContent);
            }

            var result = await query.FirstOrDefaultAsync(x => x.RecordTransferId == recordTransferId, cancellationToken);

            return ServiceResult<RecordTransfer>.FromSuccess(result);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult<RecordTransfer>.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IPagedList<RecordTransfer>>> GetRecordTransfersAsync(RecordTransferFilter recordTransferFilter, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<RecordTransfer> query = this._dbContext.RecordTransfers;

            if (recordTransferFilter.RecordTransferIds != null)
            {
                query = query.Where(rt => recordTransferFilter.RecordTransferIds.Any(filterId => filterId == rt.RecordTransferId));
            }

            var result = await query.ToPagedListAsync(recordTransferFilter.PageNumber, recordTransferFilter.PageSize, null, cancellationToken);

            return ServiceResult<IPagedList<RecordTransfer>>.FromSuccess(result);

        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult<IPagedList<RecordTransfer>>.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<long?[]>> GetRecordTransferIdsAsync(RecordTransferFilter recordTransferFilter, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = recordTransferFilter.ApplyFilters(this._dbContext.RecordTransfers);

            var result = await query.Select(x=>x.RecordTransferId).ToArrayAsync(cancellationToken);

            return ServiceResult<long?[]>.FromSuccess(result);

        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult<long?[]>.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }

    public async Task<ServiceResult> UpdateRecordTransferAsync(RecordTransfer recordTransfer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(recordTransfer));

        try
        {
            recordTransfer.RecModified = DateTime.UtcNow;

            this._dbContext.RecordTransfers.Update(recordTransfer);
            var result = await this._dbContext.SaveChangesAsync(cancellationToken);

            return result == 1
                ? ServiceResult.FromSuccess()
                : ServiceResult.FromError($"Error: Could not update record transfer with id: '{recordTransfer.RecordTransferId}'");
        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult.FromError($"UnexpectedError: Could not update Record Transfer. Details: '{e.Message}'.");
        }
    }
}
