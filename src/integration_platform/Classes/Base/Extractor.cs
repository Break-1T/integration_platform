using integration_platform.database.Enums;
using integration_platform.database.Filters;
using integration_platform.database.Interfaces;
using integration_platform.database.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace integration_platform.Classes.Base;

public abstract class Extractor : BaseIntegrationJob
{
    /// <summary>
    /// The record transfer setting name
    /// </summary>
    public const string RecordTransferTypeSettingName = "Record Transfer Type";

    /// <summary>
    /// The transform recor setting name
    /// </summary>
    public const string TransformRecordTypeSettingName = "Transform Record Type";

    /// <summary>
    /// The source node setting name
    /// </summary>
    public const string SourceNodeSettingName = "Source Node";

    /// <summary>
    /// The target node setting name
    /// </summary>
    public const string TargetNodeSettingName = "Target Node";

    /// <summary>
    /// Gets the record transfer store.
    /// </summary>
    public IRecordTransferStore RecordTransferStore { get; }

    /// <summary>
    /// Gets the rransform record store.
    /// </summary>
    public ITransformRecordStore TransformRecordStore { get; }

    /// <summary>
    /// Gets the type of the document.
    /// </summary>
    protected string TransformRecordType { get; private set; }

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    protected string RecordTransferType { get; private set; }

    /// <summary>
    /// Gets the source.
    /// </summary>
    protected string SourceNode { get; private set; }

    /// <summary>
    /// Gets the target.
    /// </summary>
    protected string TargetNode { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Extractor"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="recordTransferStore">The record transfer store.</param>
    /// <param name="transformRecordStore">The transform record store.</param>
    protected Extractor(ILogger<Extractor> logger, IRecordTransferStore recordTransferStore, ITransformRecordStore transformRecordStore) : base(logger)
    {
        RecordTransferStore = recordTransferStore;
        TransformRecordStore = transformRecordStore;
    }

    public override void InitDefaultJobSettings(JobDataMap settings)
    {
        base.InitDefaultJobSettings(settings);
        settings.Put(SourceNodeSettingName, "");
        settings.Put(TargetNodeSettingName, "");
        settings.Put(RecordTransferTypeSettingName, "");
        settings.Put(TransformRecordTypeSettingName, "");
    }

    protected abstract Task CreateRecordTransfersAsync();

    protected abstract Task<ParseResult> ParseRecordTransferAsync(RecordTransfer documentTransfer, CancellationToken cancellationToken = default);

    protected virtual Task<ReceiveResult> ReceiveDocumentTransferAsync(RecordTransfer recordTransfer, CancellationToken cancellationToken = default)
    {
        var receiveResult = new ReceiveResult();

        if (recordTransfer.RequestContent == null)
        {
            receiveResult.StatusMessage = "Document Transfer doesn't contain any data";
            receiveResult.IsSuccess = false;
        }

        return Task.FromResult(receiveResult);
    }

    public override async Task<bool> BeforeExecutingAsync(CancellationToken cancellationToken)
    {
        var res = await base.BeforeExecutingAsync();
        if (res)
        {
            SourceNode = JobSettings.GetString(SourceNodeSettingName);
            TargetNode = JobSettings.GetString(TargetNodeSettingName);
            TransformRecordType = JobSettings.GetString(TransformRecordTypeSettingName);
            RecordTransferType = JobSettings.GetString(RecordTransferTypeSettingName);
        }

        return res;
    }

    /// <inheritdoc />
    public override async Task<bool> ExecuteJobAsync(CancellationToken cancellationToken)
    {
        // Create NEW Record Transfers
        await CreateRecordTransfersAsync();

        // Update Record Transfers to Processing status
        var getRecordIdsResult = await RecordTransferStore.GetRecordTransferIdsAsync(
            RecordTransferFilter.CreateFilter()
                .WithSource(SourceNode)
                .WithTarget(TargetNode)
                .WithType(RecordTransferType)
                .WithStatus(RecordTransferStatus.New));

        if (!getRecordIdsResult.IsSuccess)
        {
            return false;
        }

        await Parallel.ForEachAsync(getRecordIdsResult.Result, 
            new ParallelOptions { MaxDegreeOfParallelism = MaxThreads, CancellationToken = cancellationToken }, 
            async (recordId, token) =>
        {
            var getRecordTransferResult = await RecordTransferStore.GetRecordTransferAsync(recordId, true, token);

            if (!getRecordTransferResult.IsSuccess)
            {
                this.Logger.LogError($"Could not find record with id: '{recordId}'. Details: '{getRecordTransferResult.ErrorMessage}'");
                return;
            }

            var recordTransfer = getRecordTransferResult.Result;

            var receiveResult = await this.ReceiveDocumentTransferAsync(recordTransfer, token);

            if (receiveResult.IsSuccess)
            {
                recordTransfer.Status = receiveResult.IsSuccess
                    ? RecordTransferStatus.Processed
                    : RecordTransferStatus.Failed;
            }

            recordTransfer.StatusMessage = receiveResult.StatusMessage;
            recordTransfer.TargetId = receiveResult.TargetId;
            recordTransfer.ResponseContent = new RecordTransferContent
            {
                Content = Encoding.UTF8.GetBytes(receiveResult.ResponseContent ?? "")
            };

            var updateResult = await RecordTransferStore.UpdateRecordTransferAsync(recordTransfer, token);

            if (!updateResult.IsSuccess)
            {
                this.Logger.LogError($"Could not update record with id: '{recordId}'. Details: '{updateResult.ErrorMessage}'");
                return;
            }

        });

        // Create Transform Records
        getRecordIdsResult = await RecordTransferStore.GetRecordTransferIdsAsync(
            RecordTransferFilter.CreateFilter()
                .WithSource(SourceNode)
                .WithTarget(TargetNode)
                .WithType(RecordTransferType)
                .WithStatus(RecordTransferStatus.Processing));

        await Parallel.ForEachAsync(getRecordIdsResult.Result, 
            new ParallelOptions { MaxDegreeOfParallelism = MaxThreads, CancellationToken = cancellationToken }, 
            async (recordId, token) =>
        {
            var getRecordTransferResult = await RecordTransferStore.GetRecordTransferAsync(recordId, true, token);

            if (!getRecordTransferResult.IsSuccess)
            {
                this.Logger.LogError($"Could not find record with id: '{recordId}'. Details: '{getRecordTransferResult.ErrorMessage}'");
                return;
            }

            var recordTransfer = getRecordTransferResult.Result;

            var parseResult = await this.ParseRecordTransferAsync(recordTransfer, token);

            recordTransfer.Status = parseResult.Status;
            recordTransfer.StatusMessage = parseResult.StatusMessage;

            var updateRecordResult = await RecordTransferStore.UpdateRecordTransferAsync(recordTransfer, token);
            if (!updateRecordResult.IsSuccess)
            {
                this.Logger.LogError($"Could not update record with id: '{recordId}'. Details: '{getRecordTransferResult.ErrorMessage}'");
            }

            if (parseResult.TransformRecords == null || parseResult.TransformRecords.Count == 0)
            {
                return;
            }

            _= await TransformRecordStore.AddTransformRecordsAsync(parseResult.TransformRecords, token);
        });

        return true;
    }
}