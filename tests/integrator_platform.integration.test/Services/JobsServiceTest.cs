using integration_platform.Extensions;
using integration_platform.Interfaces;
using integrator_platform.integration.test.Classes;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace integrator_platform.integration.test.Services;

[TestFixture]
internal class JobsServiceTest
{
    private TestServer _testServer;
    private IJobsService _jobsService;

    public JobsServiceTest()
    {
        var fixture = new CustomWebApplicationFactory(
            services =>
            {
                services.AddIntegratorJob<TestJob>();
            },
            serviceProvider =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    this._jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();
                }
                
                serviceProvider.AddIntegratorJobAsync<TestJob>("test_job", "test_group", "0 */10 * * * ?", []).GetAwaiter().GetResult();

            });


        this._testServer = fixture.Server;
    }

    [Test]
    public async Task UpdateJobAsync_ValidParams_JobUpdated()
    {
        // Arrange

        // Act
        var result = await this._jobsService.UpdateJobAsync("test_job", "test_group", [], "");

        // Assert
        Assert.That(result.IsSuccess);
    }

    [Test]
    public async Task GetJobsAsync_JobsObtained_ReturnSuccess()
    {
        // Arrange

        // Act
        var result = await this._jobsService.GetJobsAsync();

        // Assert
        Assert.That(result.IsSuccess);
        Assert.That(result.Result.Count, Is.EqualTo(1));
    }
}
