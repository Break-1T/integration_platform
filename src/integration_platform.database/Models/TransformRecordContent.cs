namespace integration_platform.database.Models;

/// <summary>
/// TransformRecordContent.
/// </summary>
/// <seealso cref="integration_platform.database.Models.BaseRecord" />
public class TransformRecordContent : BaseRecord
{
    /// <summary>
    /// Gets or sets the transform record content identifier.
    /// </summary>
    public long? TransformRecordContentId { get; set; }

    /// <summary>
    /// Gets or sets the content of the in.
    /// </summary>
    public byte[] Content { get; set; }

    /// <summary>
    /// Gets or sets the type of the content.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the transform record identifier.
    /// </summary>
    public long? TransformRecordId { get; set; }

    /// <summary>
    /// Gets or sets the transform record.
    /// </summary>
    public TransformRecord TransformRecord { get; set; }
}
