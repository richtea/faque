using Faque.Models;
using Faque.Services;

namespace Faque.Tests.Services;

public class RouteConfigStoreFixture
{
    [Fact]
    public void should_store_and_retrieve_route()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse { StatusCode = 200 },
            Enabled = true,
        };

        // **** ACT ****
        var upsertResult = store.Upsert(route, null);
        var getResult = store.Get("GET", "/api/users");

        // **** ASSERT ****
        upsertResult.IsSuccess.Should().BeTrue();
        getResult.IsSuccess.Should().BeTrue();
        getResult.Value.PathPattern.Should().Be("/api/users");
    }

    [Fact]
    public void should_return_not_found_for_missing_route()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();

        // **** ACT ****
        var result = store.Get("GET", "/api/missing");

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_update_existing_route()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse
            {
                StatusCode = 200,
                Body = "Original",
            },
            Enabled = true,
        };

        store.Upsert(route, null);

        var updatedRoute = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse
            {
                StatusCode = 200,
                Body = "Updated",
            },
            Enabled = true,
        };

        // **** ACT ****
        var result = store.Upsert(updatedRoute, null);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.Response.Body.Should().Be("Updated");
    }

    [Fact]
    public void should_detect_concurrent_modification()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse { StatusCode = 200 },
            Enabled = true,
        };

        var firstResult = store.Upsert(route, null);
        var version = firstResult.Value.Version;

        // Simulate another update
        store.Upsert(route, version);

        // Try to update with a stale version
        var staleUpdate = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse
            {
                StatusCode = 200,
                Body = "Stale",
            },
            Enabled = true,
        };

        // **** ACT ****
        var result = store.Upsert(staleUpdate, version);

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_delete_route()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse { StatusCode = 200 },
            Enabled = true,
        };

        store.Upsert(route, null);

        // **** ACT ****
        var deleteResult = store.Delete("GET", "/api/users");
        var getResult = store.Get("GET", "/api/users");

        // **** ASSERT ****
        deleteResult.IsSuccess.Should().BeTrue();
        getResult.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_validate_http_method()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "INVALID",
            PathPattern = "/api/users",
            Response = new RouteResponse { StatusCode = 200 },
            Enabled = true,
        };

        // **** ACT ****
        var result = store.Upsert(route, null);

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_validate_status_code_range()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var route = new RouteConfiguration
        {
            Method = "GET",
            PathPattern = "/api/users",
            Response = new RouteResponse { StatusCode = 999 },
            Enabled = true,
        };

        // **** ACT ****
        var result = store.Upsert(route, null);

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task should_allow_concurrent_additions_of_different_routes()
    {
        // **** ARRANGE ****
        var store = new RouteConfigStore();
        var tasks = new List<Task<bool>>();

        for (var i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(
                Task.Run(() =>
                {
                    var route = new RouteConfiguration
                    {
                        Method = "GET",
                        PathPattern = $"/api/route{index}",
                        Response = new RouteResponse { StatusCode = 200 },
                        Enabled = true,
                    };
                    return store.Upsert(route, null).IsSuccess;
                }));
        }

        // **** ACT ****
        await Task.WhenAll(tasks.ToArray());

        // **** ASSERT ****
        tasks.Select(t => t.Result).Should().OnlyContain(result => result);
        store.GetAll().Should().HaveCount(10);
    }
}
