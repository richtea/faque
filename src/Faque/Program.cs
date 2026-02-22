using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using Faque.Middleware;
using Faque.Startup;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

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

app.UseStaticFiles(
    new StaticFileOptions
    {
        RequestPath = "/$$/web",
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    }); // Serve static files from wwwroot

// Service defaults
app.MapDefaultEndpoints();

// Use Fake API middleware before routing
app.UseFakeApi();

app.UseRouting();
app.MapControllers();

app.MapOpenApi("/$$/openapi/{documentName}.json");
app.MapScalarApiReference(
    "/$$/docs",
    options =>
    {
        options.WithTitle("Faque API Reference");
        options.WithOpenApiRoutePattern("/$$/openapi/{documentName}.json");
    });

await app.InitAndRunAsync();

/// <summary>
/// The entry point for the application.
/// </summary>

// ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable ASP0027
public partial class Program;
#pragma warning restore ASP0027
