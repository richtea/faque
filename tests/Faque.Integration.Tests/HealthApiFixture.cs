using System.Net;
using Faque.Integration.Tests.Infrastructure;

namespace Faque.Integration.Tests;

public class HealthApiFixture
{
    [Fact]
    public async Task should_return_healthy_status()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        // **** ACT ****
        using var host = IntegrationTestHost.Create();
        var response = await host.Client.GetAsync("/$$/api/health", ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<HealthResponse>(ct);
        content.Should().NotBeNull();
        content!.Status.Should().Be("healthy");
    }

    private record HealthResponse(string Status, DateTimeOffset Timestamp);
}
