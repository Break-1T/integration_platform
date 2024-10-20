//using Microsoft.Extensions.Logging;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace integration_platform.Classes;

//public class Extractor : BaseIntegrationJob
//{
//    #region Protected Properties

//    /// <summary>
//    /// Gets the managers factory.
//    /// </summary>
//    /// <value>
//    /// The managers factory.
//    /// </value>
//    protected IManagersFactory ManagersFactory { get; }

//    protected bool ProcessAllAvailableRecords { get; set; }

//    /// <summary>
//    /// Gets or sets a value indicating whether [use integration layer].
//    /// </summary>
//    /// <value>
//    ///   <c>true</c> if [use integration layer]; otherwise, <c>false</c>.
//    /// </value>
//    protected bool UseIntegrationLayer { get; set; }

//    #endregion Protected Properties

//    #region Protected Constructors

//    /// <summary>
//    /// Initializes a new instance of the <see cref="Extractor" /> class.
//    /// </summary>
//    /// <param name="logger">The logger.</param>
//    /// <param name="unitOfWorkFactory">The unit of work factory.</param>
//    /// <param name="managersFactory">The managers factory.</param>
//    /// <inheritdoc />
//    protected Extractor(ILogger logger, IUnitOfWorkFactory unitOfWorkFactory,
//        IManagersFactory managersFactory, ISignalRNotificator signalRNotificator) : base(logger, unitOfWorkFactory, signalRNotificator)
//    {
//        this.ManagersFactory = managersFactory;
//    }

//    #endregion Protected Constructors

//    #region Private Methods

//    private async Task<List<long>> GetDocumentTransferIdList(DocumentTransferStatus documentTransferStatus)
//    {
//        await using var unitOfWork = this.UnitOfWorkFactory.GetUnitOfWork();
//        {
//            var tManager = this.ManagersFactory.CreateTransferManager(unitOfWork, this.SourceNode,
//                this.UseIntegrationLayer ? API.Constants.IntegratorNode : this.TargetNode);

//            if (this.ProcessAllAvailableRecords)
//            {
//                return await tManager.GetDocumentTransfersIdAsync(this.DocumentType, 0, documentTransferStatus);
//            }

//            return await tManager.GetDocumentTransfersIdAsync(this.DocumentType, this.ChunkSize, documentTransferStatus);
//        }
//    }

//    #endregion Private Methods

//    #region Protected Methods

//    /// <summary>
//    /// Determines whether this instance [can create document transfer] the specified transfer manager.
//    /// </summary>
//    /// <param name="transferManager">The transfer manager.</param>
//    /// <param name="entityStatusManager">The entity status manager.</param>
//    /// <returns>
//    ///   <c>true</c> if this instance [can receive entities] the specified transfer manager; otherwise, <c>false</c>.
//    /// </returns>
//    protected virtual Task<bool> CanCreateDocumentTransferAsync(ITransferManager transferManager,
//        IEntityManager entityStatusManager)
//    {
//        return Task.FromResult(true);
//    }

//    protected abstract Task CreateDocumentTransferListAsync(ITransferManager transferManager);

//    /// <summary>
//    /// Initializes this instance.
//    /// </summary>
//    /// <returns></returns>
//    protected override async Task<List<SettingDescription>> InitializeDefaultHandlerSettingsAsync()
//    {
//        var dhs = await base.InitializeDefaultHandlerSettingsAsync();

//        dhs.AddOrUpdateDefaultHandlerSetting(
//            settingName: UseIntegrationLayerSettingName,
//            defaultValue: "true",
//            isRequired: false,
//            readOnly: false,
//            settingType: SettingType.Bool,
//            description: "This flag determines which document will have TargetNode. " +
//                         "If [Use Integration Layer] is set to TRUE, " +
//                         "then the TargetNode will be set to 'Integrator', " +
//                         "otherwise what is defined in the TargetNode setting. " +
//                         "This is necessary if transformation is not needed.",
//            group: "System",
//            permissionLevel: "Admin"
//        );

//        dhs.AddOrUpdateDefaultHandlerSetting(
//            settingName: ExtractorTypeSettingName,
//            defaultValue: ExtractorTypeCreateReceiveParseValue,
//            settingType: SettingType.SelectBox,
//            options: new[]
//            {
//                ExtractorTypeCreateReceiveParseValue,
//                ExtractorTypeCreateReceiveValue,
//                ExtractorTypeReceiveParseValue,
//                ExtractorTypeCreateOnlyValue,
//                ExtractorTypeReceiveOnlyValue,
//                ExtractorTypeParseOnlyValue,
//            },
//            description: "The type of action that the extractor will perform during execution.",
//            group: "System",
//            permissionLevel: "Admin"
//        );

//        dhs.AddOrUpdateDefaultHandlerSetting(
//            settingName: ProcessAllAvailableRecordsSettingName,
//            defaultValue: "true",
//            isRequired: false,
//            readOnly: false,
//            settingType: SettingType.Bool,
//            description: "This flag determines how many documents will be processed. " +
//                         "If [Process all available records] is set to TRUE, " +
//                         "then the task will run until it has processed all documents, " +
//                         "otherwise it will process the count that is set in ChunkSize setting.",
//            group: "System",
//            permissionLevel: "Admin"
//        );

//        return dhs;
//    }

//    protected abstract Task<ParseResult> ParseDocumentTransferAsync(
//        IDocumentTransferReadOnly documentTransfer);

//    protected virtual Task<ReceiveResult> ReceiveDocumentTransferAsync(IDocumentTransferReadOnly documentTransfer)
//    {
//        var receiveResult = new ReceiveResult();

//        if (documentTransfer.Content == null)
//        {
//            receiveResult.StatusMessage = "Document Transfer doesn't contain any data";
//            receiveResult.SetFailed();
//        }

//        return Task.FromResult(receiveResult);
//    }

//    #endregion Protected Methods

//    #region Public Methods

//    /// <summary>
//    /// Befores the executing.
//    /// </summary>
//    /// <returns></returns>
//    public override async Task<bool> BeforeExecutingAsync()
//    {
//        var res = await base.BeforeExecutingAsync();
//        if (res)
//        {
//            this.UseIntegrationLayer =
//                bool.TryParse(this.HandlerSettings[UseIntegrationLayerSettingName], out var val1) && val1;

//            this.ExtractorType = this.HandlerSettings[ExtractorTypeSettingName];

//            this.ProcessAllAvailableRecords =
//                bool.TryParse(this.HandlerSettings[ProcessAllAvailableRecordsSettingName], out val1) && val1;
//        }

//        return res;
//    }

//    /// <summary>
//    /// Executes the specified task parameters.
//    /// </summary>
//    /// <returns></returns>
//    /// <inheritdoc />
//    public override async Task<bool> ExecuteAsync()
//    {
//        // creates NEW Document Transfers
//        if (this.ExtractorType is ExtractorTypeCreateOnlyValue or ExtractorTypeCreateReceiveParseValue or
//            ExtractorTypeCreateReceiveValue)
//        {
//            await using var unitOfWork = this.UnitOfWorkFactory.GetUnitOfWork();
//            var tManager = this.ManagersFactory.CreateTransferManager(unitOfWork, this.SourceNode,
//                this.UseIntegrationLayer ? API.Constants.IntegratorNode : this.TargetNode);

//            var esManager =
//                this.ManagersFactory.CreateEntityManager(unitOfWork, this.SourceNode, this.TargetNode);

//            if (await this.CanCreateDocumentTransferAsync(tManager, esManager))
//            {
//                await this.CreateDocumentTransferListAsync(tManager);
//            }

//            await unitOfWork.SaveChangesAsync();
//        }

//        // updates Document Transfers to Processing status
//        if (this.ExtractorType is ExtractorTypeReceiveOnlyValue or ExtractorTypeCreateReceiveParseValue or
//            ExtractorTypeReceiveParseValue or ExtractorTypeCreateReceiveValue)
//        {
//            var documentTransferIdList = await GetDocumentTransferIdList(DocumentTransferStatus.New);

//            this.OnProgressNotification(string.Format(LogMessages.TotalCountOfDocTransfers, nameof(DocumentTransferStatus.New), documentTransferIdList.Count));

//            await Parallel.ForEachAsync(documentTransferIdList, new ParallelOptions
//            {
//                MaxDegreeOfParallelism = MaxTheadCount,
//            }, async (docId, token) =>
//            {
//                IDocumentTransfer documentTransfer;

//                this.OnProgressNotification(string.Format(LogMessages.PerformingDocumentTransferStatusCheck, docId));

//                await using (var uow = this.UnitOfWorkFactory.GetUnitOfWork())
//                {
//                    var tManager = this.ManagersFactory.CreateTransferManager(uow, this.SourceNode,
//                        this.UseIntegrationLayer ? API.Constants.IntegratorNode : this.TargetNode);

//                    documentTransfer = await tManager.GetDocumentTransferWithContentAsync(docId);
//                }

//                var receiveResult = await this.ReceiveDocumentTransferAsync(documentTransfer);

//                if (receiveResult.DocumentTransferStatus != null)
//                {
//                    documentTransfer.Status = receiveResult.DocumentTransferStatus.Value;
//                }

//                documentTransfer.StatusMessage = receiveResult.StatusMessage;

//                this.OnProgressNotification(string.Format(LogMessages.PerformedDocumentTransferStatusCheck, docId, documentTransfer.Status));

//                if (!string.IsNullOrEmpty(receiveResult.TargetId))
//                {
//                    documentTransfer.TargetId = receiveResult.TargetId;
//                }

//                if (receiveResult.HasContent)
//                    documentTransfer.Content = receiveResult.Content;
//                if (receiveResult.HasAlternateContent)
//                    documentTransfer.AlternateContent = receiveResult.AlternateContent;

//                await using (var uow = this.UnitOfWorkFactory.GetUnitOfWork())
//                {

//                    var tManager = this.ManagersFactory.CreateTransferManager(uow, this.SourceNode,
//                        this.UseIntegrationLayer ? API.Constants.IntegratorNode : this.TargetNode);
//                    await tManager.UpdateDocumentAsync(documentTransfer);

//                    await uow.SaveChangesAsync();
//                }
//            });
//        }

//        // creates Entity Statuses
//        if (this.ExtractorType is ExtractorTypeParseOnlyValue or ExtractorTypeCreateReceiveParseValue or
//            ExtractorTypeReceiveParseValue)
//        {
//            List<long> documentTransferIdList;

//            documentTransferIdList = await GetDocumentTransferIdList(DocumentTransferStatus.Processing);
//            this.OnProgressNotification(string.Format(LogMessages.TotalCountOfDocTransfers, nameof(DocumentTransferStatus.Processing), documentTransferIdList.Count));

//            await Parallel.ForEachAsync(documentTransferIdList, new ParallelOptions
//            {
//                MaxDegreeOfParallelism = MaxTheadCount,
//                CancellationToken = this.CancellationToken ?? System.Threading.CancellationToken.None,
//            }, async (docId, token) =>
//            {
//                this.OnProgressNotification(string.Format(LogMessages.PerformingEntityStatusCreation, docId));

//                await using var uow = this.UnitOfWorkFactory.GetUnitOfWork();
//                var tManager = this.ManagersFactory.CreateTransferManager(uow, this.SourceNode,
//                    this.UseIntegrationLayer ? API.Constants.IntegratorNode : this.TargetNode);
//                var esManager =
//                    this.ManagersFactory.CreateEntityManager(uow, this.SourceNode,
//                        this.TargetNode);

//                var documentTransfer = await tManager.GetDocumentTransferWithContentAsync(docId);

//                var parseResult = await this.ParseDocumentTransferAsync(documentTransfer);

//                if (parseResult.EntityStatusList?.Count > 0)
//                    await esManager.CreateEntityStatusListAsync(parseResult.EntityStatusList);

//                documentTransfer.Status = parseResult.DocumentStatus;
//                documentTransfer.StatusMessage = parseResult.StatusMessage;

//                await tManager.UpdateDocumentAsync(documentTransfer);

//                await uow.SaveChangesAsync();

//                this.OnProgressNotification(string.Format(LogMessages.PerformedCreationEntity, docId));

//            });
//        }

//        return await base.ExecuteAsync();
//    }
//}