# AGENTS.md

This document provides a technical overview of the **Faque** project for AI agents and developers. It includes
information on the project's purpose, build/test procedures, and coding standards.

## Project Overview

**Faque** is a lightweight, configurable webhook and API testing service. It allows developers and automated tests to
define "fake" HTTP endpoints that record incoming requests and provide deterministic responses.

### Key Features

- **Configurable Routes**: Define endpoints with specific HTTP methods, paths, status codes, headers, and bodies.
- **Pattern Matching**: Supports wildcard patterns (`*` for a single segment, `**` for multiple segments) in URL paths.
- **Request Recording**: Captures the full history of incoming requests for inspection.
- **Web UI**: A React-based interface (accessible at `/$$/web`) for managing routes and viewing history.
- **Persistence**: Routes and request history can be saved and reloaded across restarts.
- **OpenAPI Integration**: Automatically generates OpenAPI definitions for the configured fake API.

## Build and Test Commands

The project is built using the .NET 10 SDK.

### Build the Solution

```bash
dotnet build
```

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Projects

- **Unit Tests**: `dotnet test tests/Faque.Tests/`
- **Integration Tests**: `dotnet test tests/Faque.Integration.Tests/`

### Run the Application (Aspire Orchestration)

The project uses .NET Aspire for local development.

```bash
cd src/Faque.AppHost
dotnet run
```

## Code Style and Naming Conventions

### General Guidelines

- **Language Version**: C# 14.0
- **Indentation**: 4 spaces (no tabs).
- **Namespaces**: Use file-scoped namespaces (e.g., `namespace Faque.Middleware;`).
- **Using Directives**: Place `using` directives outside the namespace. `System` namespaces should come first.
- **Documentation**: Use XML documentation comments (`/// <summary>`) for all public members.
  `GenerateDocumentationFile` is enabled.

### Naming Conventions

- **Classes, Methods, Properties**: `PascalCase`.
- **Private Fields**: `_camelCase` (e.g., `private readonly RequestRecorder _recorder;`).
- **Local Variables, Parameters**: `camelCase`.
- **Interfaces**: Prefix with `I` (e.g., `IOpenApiSchemaTransformer`).
- **Test Fixtures**: The test fixture class should be named <class-under-test>Fixture (e.g. `MyClassFixture`).
- **Test Methods**: Use `snake_case` describing the expected behavior (e.g., `should_match_exact_path`).

### Other conventions

- **Unit test structure**: Add comments for `**** ARRANGE ***`, `**** ACT ****`, and `**** ASSERT ****` sections of the
  tests.

### Libraries and Frameworks

- **Dependency Injection**: Autofac is used as the DI container.
- **Logging**: Serilog is used for logging.
- **Testing**:
  - **Framework**: xUnit v3.
  - **Assertions**: AwesomeAssertions (FluentAssertions).
  - **Mocking**: Moq.
- **Analysis**:
  - StyleCop.Analyzers is used to enforce style rules.
  - SerilogAnalyzer is used to avoid common mistakes and usage problems.

## Project Structure

- `src/Faque/`: The core ASP.NET Core backend.
- `src/Faque.AppHost/`: .NET Aspire orchestration project.
- `src/frontend/`: React TypeScript frontend.
- `tests/Faque.Tests/`: Unit tests.
- `tests/Faque.Integration.Tests/`: Integration tests using `WebApplicationFactory`.
