# HookNorton

A lightweight webhook/API testing service for development and automated testing. HookNorton exposes a configurable fake
HTTP API that records incoming requests and responds deterministically based on route configuration.

## Features

- **Configurable Routes**: Define fake API endpoints with custom HTTP methods, paths, status codes, headers, and
  response bodies
- **Pattern Matching**: Support for simple wildcard patterns (`*` and `**`) in URL paths
- **Request Recording**: Automatically captures all incoming requests with full history
- **Web UI**: React-based interface to manage routes and view request history
- **Persistence**: Request history and route configurations can be persisted across container restarts
- **Deterministic Responses**: Predictable responses for reliable testing and CI/CD pipelines

## Running the image with Docker

**Run the container:**

```bash
docker run -p 8080:8080 \
  -e Kestrel__Endpoints__http__Url='http://*:8080' \
  -v hook-norton-data:/app/data \
  api:latest
```

This exposes the application on port 8080 and mounts a named volume for data persistence.

## API Usage

Once running, you can:

1. **Access the Web UI** at the application root (served from the React frontend)
2. **Call the fake API** at the routes you've configured (e.g., `GET /api/users` if configured)
3. **Manage routes** via the API:
   - `GET /routes` - List all routes
   - `POST /routes` - Create a new route
   - `PUT /routes/{id}` - Update a route
   - `DELETE /routes/{id}` - Delete a route
4. **View request history** at `/requests`
5. **Use a browsable interface** at `/$$/docs`

For detailed API documentation, refer to [docs/api-specification.md](docs/api-specification.md).

## Development

See [docs](docs/DEVELOPMENT.md).
