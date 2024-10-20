using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace integration_platform.database;

public class IntegrationPlatformDesignContextFactory : IDesignTimeDbContextFactory<IntegrationPlatformDbContext>
{
    public IntegrationPlatformDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=integration_platform_db;";
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseNpgsql(connectionString);

        return new IntegrationPlatformDbContext(optionsBuilder.Options);
    }
}