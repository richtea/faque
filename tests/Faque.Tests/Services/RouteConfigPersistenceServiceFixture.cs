using System.IO.Abstractions;
using System.Reflection;
using Faque.Models;
using Faque.Services;
using Faque.Startup;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;

namespace Faque.Tests.Services;

public class RouteConfigPersistenceServiceFixture
{
    private readonly Mock<IFileSystem> _fileSystemMock;

    private readonly Mock<ILogger> _loggerMock;

    private readonly RouteConfigStore _routeStore;

    private readonly IOptions<FaqueOptions> _options;

    private readonly IOptions<JsonOptions> _jsonOptions;

    public RouteConfigPersistenceServiceFixture()
    {
        _fileSystemMock = new Mock<IFileSystem>();
        _loggerMock = new Mock<ILogger>();
        _routeStore = new RouteConfigStore();

        var options = new FaqueOptions
        {
            DataDirectory = "/data",
            RouteConfigDebounceSeconds = 0, // Immediate save for testing
        };
        _options = Options.Create(options);

        _jsonOptions = Options.Create(new JsonOptions());
    }

    [Fact]
    public async Task should_not_miss_changes_during_save()
    {
        // **** ARRANGE ****
        // We use a real RouteConfigPersistenceService but mock the file system
        var service = new RouteConfigPersistenceService(
            _routeStore,
            _options,
            _fileSystemMock.Object,
            _jsonOptions,
            _loggerMock.Object);

        var pathMock = new Mock<IPath>();
        _fileSystemMock.Setup(f => f.Path).Returns(pathMock.Object);
        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns("/data/config");

        var directoryMock = new Mock<IDirectory>();
        _fileSystemMock.Setup(f => f.Directory).Returns(directoryMock.Object);

        var fileMock = new Mock<IFile>();
        _fileSystemMock.Setup(f => f.File).Returns(fileMock.Object);

        // This is where we simulate the race condition.
        // We delay the WriteAllTextAsync call and trigger another route change during it.
        fileMock.Setup(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string _, string _, CancellationToken _) =>
            {
                // Trigger another change while saving is "in progress"
                _routeStore.Upsert(
                    new RouteConfiguration
                    {
                        Method = "POST",
                        PathPattern = "/another",
                        Response = new RouteResponse { StatusCode = 201 },
                    },
                    null);

                await Task.Yield();
            });

        // Trigger the initial change to start the save process
        _routeStore.Upsert(
            new RouteConfiguration
            {
                Method = "GET",
                PathPattern = "/initial",
                Response = new RouteResponse { StatusCode = 200 },
            },
            null);

        // **** ACT ****
        // Start the background service loop (via ExecuteAsync indirectly)
        // or just call SaveRoutesAsync if it was public.
        // Since SaveRoutesAsync is private, we'll use reflection or rely on the background loop.
        // Actually, we can just call the private method via reflection for a unit test.
        var method = typeof(RouteConfigPersistenceService).GetMethod(
            "SaveRoutesAsync",
            BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)method!.Invoke(service, [CancellationToken.None])!;

        // **** ASSERT ****
        // After the save completes, _hasPendingChanges should be TRUE because a change happened during the save.
        var field = typeof(RouteConfigPersistenceService).GetField(
            "_hasPendingChanges",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var hasPendingChanges = (bool)field!.GetValue(service)!;

        hasPendingChanges.Should().BeTrue(
            "because a change occurred during the save operation and should not have been cleared");
    }
}
