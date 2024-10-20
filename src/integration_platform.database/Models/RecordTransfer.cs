using integration_platform.database.Enums;

namespace integration_platform.database.Models;

public class RecordTransfer : BaseRecord
{
    /// <summary>
    /// Gets or sets the record transfer identifier.
    /// </summary>
    public long? RecordTransferId { get; set; }

    /// <summary>
    /// Gets or sets the response content identifier.
    /// </summary>
    public long? ResponseContentId { get; set; }
    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    public RecordTransferContent ResponseContent { get; set; }

    /// <summary>
    /// Gets or sets the request content identifier.
    /// </summary>
    public long? RequestContentId { get; set; }

    /// <summary>
    /// Gets or sets the content of the request.
    /// </summary>
    public RecordTransferContent RequestContent { get; set; }

    /// <summary>
    /// Gets or sets the type of the record.
    /// </summary>
    public string RecordType { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the source identifier.
    /// </summary>
    public string SourceId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public RecordTransferStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the target identifier.
    /// </summary>
    public string TargetId { get; set; }
}
