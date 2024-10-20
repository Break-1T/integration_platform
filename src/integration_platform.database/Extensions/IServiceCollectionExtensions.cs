using integration_platform.database.Constants;
using integration_platform.database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace integration_platform.database.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection serviceDescriptors, IConfiguration configuration)
    {
        var dbSettingsSection = configuration.GetSection(DatabaseConstants.DatabaseSectionKey);

        serviceDescriptors.AddOptions<DatabaseSettings>()
            .Bind(dbSettingsSection)
            .ValidateOnStart();

        var dbSettings = dbSettingsSection.Get<DatabaseSettings>();

        serviceDescriptors.AddPooledDbContextFactory<IntegrationPlatformDbContext>(options =>
        {
            options.UseNpgsql(dbSettings.ConnectionString, npgOptions =>
            {
                npgOptions.EnableRetryOnFailure();
            });
        });

        return serviceDescriptors;
    }
}
