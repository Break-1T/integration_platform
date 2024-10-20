namespace integration_platform.database.Enums;

/// <summary>
/// TransformRecordStatus.
/// </summary>
public enum TransformRecordStatus
{
    AwaitingConfirmation = 1,
    NotConfirmed = 2,
    Confirmed = 3,
    Ignored = 4,
    AwaitingTransformation = 5,
    UnexpectedError = 6,
    SendError = 7,
    TransformError = 8,
}
