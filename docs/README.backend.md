# Faque Backend Implementation

## Overview

The Faque backend has been implemented according to the specifications in [docs/api-specification.md](docs/api-specification.md) and [docs/high-level-specification.md](docs/high-level-specification.md).

## Project Structure

Following the [dotnet project layout conventions](https://gist.github.com/davidfowl/ed7564297c61fe9ab814):

```
/
├── src/
│   └── Faque/              # Main ASP.NET Core Web API project
│       ├── Common/              # Result pattern and error types
│       ├── Configuration/       # IOptions configuration
│       ├── Controllers/         # Developer API controllers
│       ├── Middleware/          # FakeApiMiddleware
│       ├── Models/              # Domain models
│       ├── Services/            # Business logic services
│       └── wwwroot/             # Static files (placeholder UI)
├── tests/
│   ├── Faque.Tests/        # Unit tests (xUnit V3, AwesomeAssertions, Moq)
│   └── Faque.Integration.Tests/  # Integration tests (WebApplicationFactory)
├── docs/                        # Specification documents
└── Faque.sln              # Solution file
```

## Key Features Implemented

### 1. Result Pattern for Error Handling
- `Result<T>` and `Result` types for functional error handling
- No exceptions for expected scenarios (NotFound, Validation, Conflict)
- Automatic conversion to RFC 9457 Problem Details in controllers

### 2. Route Configuration
- Thread-safe `RouteConfigStore` with `ConcurrentDictionary`
- Optimistic concurrency control with version tracking
- Concurrent additions supported, concurrent modifications return 409 Conflict
- Background persistence with debouncing (configurable)

### 3. Pattern Matching
- `RouteMatcher` supports:
  - `*` for single-segment wildcards (e.g., `/api/users/*` matches `/api/users/123`)
  - `**` for multi-segment wildcards (e.g., `/api/**` matches `/api/users/123/posts`)
  - First-match-wins precedence
  - Case-insensitive method matching

### 4. Request Recording
- UUIDv7 from UUIDNext for chronologically sortable request IDs
- Bounded in-memory collection (FIFO eviction)
- Individual file persistence per request (`{uuidv7}.json`)
- Automatic body truncation at configured `MaxBodySize`
- Background cleanup service removes stale files

### 5. Developer API Controllers
- `RoutesController`: CRUD operations for route configuration
- `RequestsController`: Retrieve request history, clear history
- `HealthController`: Health check endpoint
- All errors returned as RFC 9457 Problem Details with `application/problem+json`

### 6. Fake API Middleware
- Intercepts all requests except `/$$/api/*` and `/$$/web/*`
- Records every request with UUIDv7 ID
- Matches against configured routes using `RouteMatcher`
- Returns configured responses or 404 Problem Details

### 7. Configuration
- IOptions pattern for all settings
- Configurable via `appsettings.json`:
  - `MaxRequestHistory`: Maximum requests to keep (default: 1000)
  - `MaxBodySize`: Maximum body size in bytes (default: 1MB)
  - `CleanupIntervalSeconds`: Background cleanup interval (default: 60s)
  - `RouteConfigDebounceSeconds`: Debounce for route persistence (default: 2s)
  - `DataDirectory`: Root directory for persisted data (default: ./data)

### 8. Persistence
- Routes: Single JSON file (`./data/config/routes.json`)
  - Background writer with debouncing
  - Atomic writes via temp file + move
  - Loads on startup (warns and continues with empty config if missing/corrupt)
- Requests: Individual files (`./data/history/{uuidv7}.json`)
  - Synchronous write per request
  - UUIDv7 naming for chronological sorting
  - Background cleanup of stale files

## Running the Application

```bash
# Build
dotnet build

# Run
cd src/Faque
dotnet run

# Access
# HTTP: http://localhost:8080
# HTTPS: https://localhost:8081
# Web UI: http://localhost:8080/$$/web
# Developer API: http://localhost:8080/$$/api
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/Faque.Tests

# Run integration tests only
dotnet test tests/Faque.Integration.Tests
```

## Test Conventions

- Test classes use `Fixture` suffix (e.g., `RouteMatcherFixture`)
- Test methods use `lower_snake_case` naming (e.g., `should_match_single_segment_wildcard`)
- Unit tests use xUnit V3, AwesomeAssertions, and Moq
- Integration tests use `WebApplicationFactory<Program>` with `IClassFixture`

## Code Conventions

- Microsoft .NET naming conventions (StyleCop-style)
- No `this.` prefix
- Class fields prefixed with `_` (underscore)
- Nullable reference types enabled
- Implicit usings enabled

## Next Steps

The following areas can be expanded:

1. **Additional Unit Tests**: More comprehensive coverage of edge cases
2. **Frontend Implementation**: React TypeScript UI for `/$$/web`
3. **Docker Support**: Dockerfile and docker-compose.yml
4. **OpenAPI/Swagger**: API documentation endpoint
5. **Logging Enhancements**: Structured logging with correlation IDs
6. **Metrics**: Prometheus/OpenTelemetry integration
7. **CORS Configuration**: For browser-based API clients

## API Examples

### Create a Route
```bash
curl -X PUT http://localhost:8080/\$\$/api/routes/GET/%2Fapi%2Fusers \
  -H "Content-Type: application/json" \
  -d '{
    "response": {
      "statusCode": 200,
      "headers": { "Content-Type": "application/json" },
      "body": "{\"users\": []}"
    },
    "enabled": true
  }'
```

### Call the Fake API
```bash
curl http://localhost:8080/api/users
# Returns: {"users": []}
```

### View Request History
```bash
curl http://localhost:8080/\$\$/api/requests
```

### Health Check
```bash
curl http://localhost:8080/\$\$/api/health
```

## License

See LICENSE file in repository root.
