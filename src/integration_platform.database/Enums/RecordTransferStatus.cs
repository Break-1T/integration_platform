namespace integration_platform.database.Enums;

/// <summary>
/// RecordTransferStatus.
/// </summary>
public enum RecordTransferStatus
{
    New = 0,
    Processing,
    Processed,
    Failed,
    Ignored
}
