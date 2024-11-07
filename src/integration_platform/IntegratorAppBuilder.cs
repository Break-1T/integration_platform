using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net;
using integration_platform.database.Extensions;
using integration_platform.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;

namespace integration_platform;

public static class IntegratorAppBuilder
{
    public static WebApplication CreateWebApplication(string[] args, Action<IServiceCollection> configureServices = null, Action<IApplicationBuilder> configureApp = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.UseKestrel(options =>
        {
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
            options.Listen(IPAddress.Any, 8080);
        });

        builder.Host.UseSerilog((whb, loggerConfiguration) =>
        {
            loggerConfiguration
                .WriteTo.Console()
                .WriteTo.Debug();
        }, writeToProviders: true, preserveStaticLogger: false);

        builder.Configuration.SetBasePath(AppContext.BaseDirectory);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddCors();
        builder.Services.AddHealthChecks();
        builder.Services.AddDatabaseServices(builder.Configuration);
        builder.Services.AddIntegratorPlatform(builder.Configuration);

        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddMvcCore()
            .AddDataAnnotations()
            .AddApiExplorer()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState);
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        configureApp?.Invoke(app);

        app.UseSwagger();
        app.UseSwaggerUI();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        });

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true
        });

        app.MapGet("/", context => context.Response.WriteAsync($"Worker Service ${Environment.GetEnvironmentVariable("WORKER_NAME")}"));

        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

        app.UseHttpsRedirection();

        app.MapControllers();

        return app;
    }
}
