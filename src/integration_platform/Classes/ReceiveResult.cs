namespace integration_platform.Classes;

public class ReceiveResult
{
    public string StatusMessage { get; set; }
    public bool IsSuccess { get; set; }
    public string ResponseContent { get; set; }
    public string SourceId { get; set; }
    public string TargetId { get; set; }
}
