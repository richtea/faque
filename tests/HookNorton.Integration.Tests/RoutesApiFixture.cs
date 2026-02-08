using System.Net;
using System.Text.Json;
using HookNorton.Integration.Tests.Infrastructure;

namespace HookNorton.Integration.Tests;

public class RoutesApiFixture
{
    [Fact]
    public async Task should_create_and_retrieve_route()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        var routeRequest = new
        {
            response = new
            {
                statusCode = 200,
                headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" },
                body = "{\"message\": \"Hello\"}",
            },
            enabled = true,
        };

        using var host = IntegrationTestHost.Create();
        var createResponse = await host.Client.PutAsJsonAsync(
            "/$$/api/routes/GET/%2Fapi%2Ftest",
            routeRequest,
            ct);

        // **** ACT ****
        var getResponse = await host.Client.GetAsync("/$$/api/routes/GET/%2Fapi%2Ftest", ct);

        // **** ASSERT ****
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var route = await getResponse.Content.ReadFromJsonAsync<JsonElement>(ct);
        route.GetProperty("method").GetString().Should().Be("GET");
        route.GetProperty("pathPattern").GetString().Should().Be("/api/test");
    }

    [Fact]
    public async Task should_return_404_for_non_existent_route()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        // **** ACT ****
        using var host = IntegrationTestHost.Create();
        var response = await host.Client.GetAsync("/$$/api/routes/GET/%2Fapi%2Fnonexistent", ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var contentType = response.Content.Headers.ContentType?.MediaType;
        contentType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task should_delete_route()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        var routeRequest = new
        {
            response = new
            {
                statusCode = 200,
                headers = new Dictionary<string, string>(),
                body = string.Empty,
            },
            enabled = true,
        };

        using var host = IntegrationTestHost.Create();
        await host.Client.PutAsJsonAsync(
            "/$$/api/routes/DELETE/%2Fapi%2Fdelete-test",
            routeRequest,
            ct);

        // **** ACT ****
        var deleteResponse = await host.Client.DeleteAsync("/$$/api/routes/DELETE/%2Fapi%2Fdelete-test", ct);
        var getResponse = await host.Client.GetAsync("/$$/api/routes/DELETE/%2Fapi%2Fdelete-test", ct);

        // **** ASSERT ****
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task should_list_all_routes()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        // **** ACT ****
        using var host = IntegrationTestHost.Create();
        var response = await host.Client.GetAsync("/$$/api/routes", ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
        content.Should().NotBeNull();
        content.ValueKind.Should().Be(JsonValueKind.Object);
        content.TryGetProperty("routes", out _).Should().BeTrue();
    }

    [Fact]
    public async Task should_return_422_for_invalid_status_code()
    {
        // **** ARRANGE ****
        var ct = TestContext.Current.CancellationToken;

        var invalidRoute = new
        {
            response = new
            {
                statusCode = 999,
                headers = new Dictionary<string, string>(),
                body = string.Empty,
            },
            enabled = true,
        };

        // **** ACT ****
        using var host = IntegrationTestHost.Create();
        var response = await host.Client.PutAsJsonAsync(
            "/$$/api/routes/GET/%2Fapi%2Finvalid",
            invalidRoute,
            ct);

        // **** ASSERT ****
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
