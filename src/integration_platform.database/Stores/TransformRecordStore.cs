using integration_platform.database.Filters;
using integration_platform.database.Interfaces;
using integration_platform.database.Models;
using integration_platform.utils.classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EF;

namespace integration_platform.database.Stores;

/// <summary>
/// TransformRecordStore
/// </summary>
/// <seealso cref="integration_platform.database.Interfaces.ITransformRecordStore" />
public class TransformRecordStore(ILogger<TransformRecordStore> logger, IDbContextFactory<IntegrationPlatformDbContext> dbContextFactory) : ITransformRecordStore
{
    private readonly ILogger<TransformRecordStore> _logger = logger;
    private readonly IDbContextFactory<IntegrationPlatformDbContext> _dbContextFactory = dbContextFactory;
	private IntegrationPlatformDbContext _dbContext => this._dbContextFactory.CreateDbContext();

    /// <inheritdoc/>
    public async Task<ServiceResult> AddTransformRecordAsync(TransformRecord entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(entity));

		try
		{
			this._dbContext.TransformRecords.Add(entity);
			var result = await this._dbContext.SaveChangesAsync(cancellationToken);

			return result == 1
				? ServiceResult.FromSuccess()
				: ServiceResult.FromError($"Error: could not add record transfer with id: '{entity.TransformRecordId}'");
		}
		catch (Exception e)
		{
			this._logger.LogError(e, e.Message);
			return ServiceResult.FromError($"UnexpectedError: '{e.Message}'.");
		}
    }

    /// <inheritdoc/>
    public async Task<ServiceResult> AddTransformRecordsAsync(List<TransformRecord> records, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(records));

        try
        {
            this._dbContext.TransformRecords.AddRange(records);
            await this._dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.FromSuccess();
        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TransformRecord>> GetTransformRecordAsync(long transformRecordId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await this._dbContext.TransformRecords.FindAsync(transformRecordId, cancellationToken);
            return ServiceResult<TransformRecord>.FromSuccess(result);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult<TransformRecord>.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IPagedList<TransformRecord>>> GetTransformRecordsAsync(TransformRecordFilter filter, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = filter.ApplyFilters(this._dbContext.TransformRecords);

            var result = await query.ToPagedListAsync(filter.PageNumber, filter.PageSize, null, cancellationToken);
            return ServiceResult<IPagedList<TransformRecord>>.FromSuccess(result);

        }
        catch (Exception e)
        {
            this._logger.LogError(e, e.Message);
            return ServiceResult<IPagedList<TransformRecord>>.FromError($"UnexpectedError: '{e.Message}'.");
        }
    }
}
