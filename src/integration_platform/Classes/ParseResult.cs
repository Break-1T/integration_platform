using integration_platform.database.Enums;
using integration_platform.database.Models;
using System.Collections.Generic;

namespace integration_platform.Classes;

public class ParseResult
{
    public List<TransformRecord> TransformRecords { get; set; }
    public RecordTransferStatus Status { get; set; }
    public string StatusMessage { get; set; }
}
