var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.HookNorton>("api")
    .WithHttpHealthCheck("/$$/health");

builder.Build().Run();
