# Development Guide

This document outlines how to set up your local development environment and contribute to this project.

## Prerequisites

- **[.NET SDK 10.0+](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)**
- **[Node.js](https://nodejs.org/)** and **npm** (version 24 or later)
- **[Aspire CLI]((https://aspire.dev/get-started/install-cli/))**
- **[Docker](https://www.docker.com/)**

## Project Structure

```text
├── src/
│   ├── Faque/                   # ASP.NET Core backend API
│   ├── Faque.AppHost/           # .NET Aspire app orchestration
│   ├── Faque.ServiceDefaults/   # Shared service defaults
│   └── frontend/                # React TypeScript frontend
├── tests/                       # Unit and integration tests
├── docs/                        # Specification documents
└── artifacts/                   # Docker and deployment files
```

## Running Tests

**Unit tests:**

```bash
dotnet test tests/Faque.Tests/
```

**Integration tests:**

```bash
dotnet test tests/Faque.Integration.Tests/
```

## Running Locally

For an integrated development experience with .NET Aspire:

1. **Navigate to the AppHost:**

   ```bash
   cd src/Faque.AppHost
   ```

2. **Run the orchestrated application:**

   ```bash
   dotnet run
   ```

This will start both the backend API and the frontend, with monitoring via the Aspire Dashboard.

## Publishing

### Creating a Container Image

Aspire is configured to create a container image with the following command:

```bash
aspire do prepare-compose --environment production
```

The image contains the combined backend and frontend applications.

## Documentation

- [High-Level Specification](./high-level-specification.md) - Architecture and concepts
- [Backend Implementation](./README.backend.md) - Backend details and design
- [API Specification](./api-specification.md) - Complete API reference
- [Frontend Implementation Plan](./FRONTEND_IMPLEMENTATION_PLAN.md) - Frontend architecture
