using System.Collections.Generic;
using System;
using integration_platform.database.Enums;

namespace integration_platform.database.Models;

public class TransformRecord : BaseRecord
{
    /// <summary>
    /// Gets or sets the transform record identifier.
    /// </summary>
    public long? TransformRecordId { get; set; }

    /// <summary>
    /// Gets or sets the type of the record.
    /// </summary>
    public string RecordType { get; set; }

    /// <summary>
    /// Gets or sets the Entity version.
    /// </summary>
    /// <value>
    /// The Entity version.
    /// </value>
    public DateTime EntityVersion { get; set; }

    /// <summary>
    /// Gets or sets the content list.
    /// </summary>
    public List<TransformRecordContent> ContentList { get; set; }

    /// <summary>
    /// Gets or sets the in record transfer identifier.
    /// </summary>
    public long? InRecordTransferId { get; set; }

    /// <summary>
    /// Gets or sets the in record transfer.
    /// </summary>
    public RecordTransfer InRecordTransfer { get; set; }

    /// <summary>
    /// Gets or sets the out record transfer identifier.
    /// </summary>
    public long? OutRecordTransferId { get; set; }

    /// <summary>
    /// Gets or sets the out record transfer.
    /// </summary>
    public RecordTransfer OutRecordTransfer { get; set; }

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
    public TransformRecordStatus Status { get; set; }

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
