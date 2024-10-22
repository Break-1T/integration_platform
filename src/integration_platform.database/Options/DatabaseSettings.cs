using System;

namespace integration_platform.database.Options;

public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>
    /// The connection string.
    /// </value>
    public string ConnectionString { get; set; } = "Server=[Instance];Port=5432;Database=[Database];Userid=[Username];Password=[Password];No Reset On Close=True";

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <returns></returns>
    public string GetConnectionString(string databaseName = null)
    {
        var connectionString = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionString"))
            ? this.ConnectionString
            : Environment.GetEnvironmentVariable("ConnectionString");

        return connectionString
            .Replace("[Username]", Environment.GetEnvironmentVariable("Username"))
            .Replace("[Password]", Environment.GetEnvironmentVariable("Password"))
            .Replace("[Instance]", Environment.GetEnvironmentVariable("Instance"))
            .Replace("[Database]", databaseName ?? Environment.GetEnvironmentVariable("Database"));
    }
}
