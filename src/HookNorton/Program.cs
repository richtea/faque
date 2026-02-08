using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using HookNorton.Middleware;
using HookNorton.Startup;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(cb =>
    {
        cb.RegisterLogger();
    })
    .UseSerilog();

var app = builder.Build();

// Configure middleware pipeline

// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();

app.UseStaticFiles(); // Serve static files from wwwroot

// Use Fake API middleware before routing
app.UseFakeApi();

app.UseRouting();

app.MapControllers();

app.MapOpenApi("/$$/openapi/{documentName}.json");
app.MapScalarApiReference(
    "/$$/scalar",
    options =>
{
    options.WithTitle("HookNorton API Reference");
    options.WithOpenApiRoutePattern("/$$/openapi/{documentName}.json");
});

Log.Information("HookNorton starting on HTTP: http://localhost:8080, HTTPS: https://localhost:8081");

await app.InitAndRunAsync();

/// <summary>
/// The entry point for the application.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable ASP0027
public partial class Program;
#pragma warning restore ASP0027
