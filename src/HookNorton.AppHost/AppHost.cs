var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.HookNorton>("api")
    .WithHttpHealthCheck("/$$/health")
    .WithExternalHttpEndpoints();

builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
