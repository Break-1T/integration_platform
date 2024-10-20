using System;

namespace integration_platform.database.Models;

public class BaseRecord
{
    /// <summary>
    /// Gets or sets the record created.
    /// </summary>
    public DateTime RecCreated { get; set; }

    /// <summary>
    /// Gets or sets the record modified.
    /// </summary>
    public DateTime RecModified { get; set; }
}
