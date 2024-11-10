using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using System.IO;
using Microsoft.AspNetCore.TestHost;

namespace integrator_platform.integration.test;

internal class CustomWebApplicationFactory(Action<IServiceCollection> configureTestServices = null, Action<IServiceProvider> configureApp = null) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Trace.Listeners.Add(new ConsoleTraceListener());

        var randomVal = new Random().Next();
        var dbName = $"{randomVal}_integration_platform_test_db";

        var settings = File.ReadAllText("appsettings.json");
        var settingsJson = JsonNode.Parse(settings);

        Environment.SetEnvironmentVariable("WORKER_NAME", "worker");
        Environment.SetEnvironmentVariable("Instance", settingsJson["Instance"].ToString());
        Environment.SetEnvironmentVariable("Username", settingsJson["Username"].ToString());
        Environment.SetEnvironmentVariable("Password", settingsJson["Password"].ToString());
        Environment.SetEnvironmentVariable("Database", dbName);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                // custom settings
            })
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        if (configureTestServices != null)
        {
            builder.ConfigureTestServices(services =>
            {
                configureTestServices(services);
            });
        }

        if (configureApp != null)
        {
            builder.Configure(appBuilder =>
            {
                configureApp(appBuilder.ApplicationServices);
            });
        }

        builder.UseConfiguration(configuration);

        builder.UseEnvironment("Development");

        base.ConfigureWebHost(builder);
    }

    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return base.CreateWebHostBuilder();
    }

    protected override void Dispose(bool disposing)
    {
        var dbContext = this.Services?.GetRequiredService<DbContext>();

        dbContext?.Database?.EnsureDeleted();

        base.Dispose(disposing);
    }

    ~CustomWebApplicationFactory(){
        var dbContext = this.Services?.GetRequiredService<DbContext>();

        dbContext?.Database?.EnsureDeleted();
    }
}
