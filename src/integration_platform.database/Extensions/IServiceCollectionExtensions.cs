using integration_platform.database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace integration_platform.database.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection serviceDescriptors, IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();

        serviceDescriptors.AddOptions<DatabaseSettings>()
            .Configure(opt => opt = databaseSettings)
            .Bind(configuration);

        serviceDescriptors.AddPooledDbContextFactory<IntegrationPlatformDbContext>(options =>
        {
            options.UseNpgsql(databaseSettings.GetConnectionString(), npgOptions =>
            {
                npgOptions.EnableRetryOnFailure();
            });
        });

        using (var scope = serviceDescriptors.BuildServiceProvider().CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<IntegrationPlatformDbContext>>();
            using var dbContext = factory.CreateDbContext();
            dbContext.Database.Migrate();
        }

        return serviceDescriptors;
    }
}
