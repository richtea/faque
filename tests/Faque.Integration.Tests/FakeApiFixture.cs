using System.Net;
using System.Text.Json;
using Faque.Integration.Tests.Infrastructure;

namespace Faque.Integration.Tests;

public class FakeApiFixture
{
    [Fact]
    public async Task should_return_configured_response_for_matching_route()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        var routeRequest = new
        {
            response = new
            {
                statusCode = 200,
                headers = new Dictionary<string, string> { ["X-Custom"] = "TestValue" },
                body = "Hello from fake API",
            },
            enabled = true,
        };

        using var host = IntegrationTestHost.Create();
        await host.Client.PutAsJsonAsync(
            "/$$/api/routes/GET/%2Ftest%2Fhello",
            routeRequest,
            ct);

        // **** ACT ****
        var response = await host.Client.GetAsync("/test/hello", ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Custom").FirstOrDefault().Should().Be("TestValue");

        var body = await response.Content.ReadAsStringAsync(ct);
        body.Should().Be("Hello from fake API");
    }

    [Fact]
    public async Task should_return_404_when_no_route_matches()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        // **** ACT ****
        using var host = IntegrationTestHost.Create();
        var response = await host.Client.GetAsync("/unmatched/path", ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync(ct);
        var contentType = response.Content.Headers.ContentType?.MediaType;
        contentType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task should_record_incoming_requests()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        using var host = IntegrationTestHost.Create();
        await host.Client.PostAsync("/test/record", new StringContent("test body"), ct);

        // **** ACT ****
        var requestsResponse = await host.Client.GetAsync("/$$/api/requests", ct);

        // **** ASSERT ****
        requestsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await requestsResponse.Content.ReadFromJsonAsync<JsonElement>(ct);
        var requests = content.GetProperty("requests");

        requests.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task should_match_wildcard_patterns()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        var routeRequest = new
        {
            response = new
            {
                statusCode = 200,
                headers = new Dictionary<string, string>(),
                body = "Wildcard matched",
            },
            enabled = true,
        };

        using var host = IntegrationTestHost.Create();
        await host.Client.PutAsJsonAsync(
            "/$$/api/routes/GET/%2Fapi%2Fusers%2F%2A",
            routeRequest,
            ct);

        // **** ACT ****
        var response1 = await host.Client.GetAsync("/api/users/123", ct);
        var response2 = await host.Client.GetAsync("/api/users/456", ct);

        // **** ASSERT ****
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var body1 = await response1.Content.ReadAsStringAsync(ct);
        var body2 = await response2.Content.ReadAsStringAsync(ct);

        body1.Should().Be("Wildcard matched");
        body2.Should().Be("Wildcard matched");
    }
}
