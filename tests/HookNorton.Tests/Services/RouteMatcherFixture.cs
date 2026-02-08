using HookNorton.Models;
using HookNorton.Services;

namespace HookNorton.Tests.Services;

public class RouteMatcherFixture
{
    private readonly RouteMatcher _matcher;

    public RouteMatcherFixture()
    {
        _matcher = new RouteMatcher();
    }

    [Fact]
    public void should_match_exact_path()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.PathPattern.Should().Be("/api/users");
    }

    [Fact]
    public void should_match_single_segment_wildcard()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users/*", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users/123", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.PathPattern.Should().Be("/api/users/*");
    }

    [Fact]
    public void should_not_match_single_segment_wildcard_across_multiple_segments()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users/*", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users/123/posts", routes);

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_match_multi_segment_wildcard()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/**", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users/123/posts", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.PathPattern.Should().Be("/api/**");
    }

    [Fact]
    public void should_match_wildcard_in_middle_of_path()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/*/status", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users/status", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void should_return_first_matching_enabled_route()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/**", Enabled = true, Response = new() { Body = "First" } },
            new() { Method = "GET", PathPattern = "/api/users/*", Enabled = true, Response = new() { Body = "Second" } },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users/123", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.Response.Body.Should().Be("First");
    }

    [Fact]
    public void should_skip_disabled_routes()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users", Enabled = false },
            new() { Method = "GET", PathPattern = "/api/users", Enabled = true, Response = new() { Body = "Enabled" } },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/users", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
        result.Value.Response.Body.Should().Be("Enabled");
    }

    [Fact]
    public void should_return_failure_when_no_route_matches()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("GET", "/api/orders", routes);

        // **** ASSERT ****
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void should_match_method_case_insensitively()
    {
        // **** ARRANGE ****
        var routes = new List<RouteConfiguration>
        {
            new() { Method = "GET", PathPattern = "/api/users", Enabled = true },
        };

        // **** ACT ****
        var result = _matcher.FindMatch("get", "/api/users", routes);

        // **** ASSERT ****
        result.IsSuccess.Should().BeTrue();
    }
}
