namespace integration_platform.database.Models;

/// <summary>
/// RecordTransferContent.
/// </summary>
/// <seealso cref="integration_platform.database.Models.BaseRecord" />
public class RecordTransferContent : BaseRecord
{
    /// <summary>
    /// Gets or sets the record transfer content identifier.
    /// </summary>
    public long? RecordTransferContentId { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public byte[] Content { get; set; }

}
