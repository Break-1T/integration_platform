using integration_platform.Classes.Base;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace integrator_platform.integration.test.Classes;

internal class TestJob(ILogger<TestJob> logger) : BaseIntegrationJob(logger)
{
    public override Task<bool> ExecuteJobAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
