HookNorton — Software Specification (Revised)
1. Purpose & Scope
HookNorton is a lightweight webhook/API testing service used during development and automated testing. It exposes a configurable fake HTTP API that records incoming requests and responds deterministically based on path and method configuration.
The system prioritizes simplicity, debuggability, and determinism over scale or performance.

2. High-Level Architecture
Single-container application:
* Backend: ASP.NET Core (Kestrel), C#
* Frontend: React (TypeScript), served as static assets
* Storage:
    * In-memory (bounded) with optional persistence to local filesystem
```text
/opt/hooknorton
├── config   (route configurations)
└── data     (request history)
```
Persistence is optional and controlled by mounting volumes in containerized deployments.

3. Core Concepts
3.1 Route Configuration
Each fake API route is defined by:
* HTTP method (single method per route)
* Path pattern (simple wildcard patterns, e.g. /api/orders/*)
* Response:
    * HTTP status code
    * Headers
    * Static body (text or JSON)
* Enabled/disabled flag (optional)
Matching rules:
* Method must match exactly
* Path supports simple glob-style patterns
* First matching route wins
* If no route matches → 404 Not Found

3.2 Request Recording
For every request received by the Fake API:
* Timestamp
* HTTP method
* Path + query string
* Headers
* Raw request body (size-limited)
Request history characteristics:
* Stored in-memory, capped to latest N requests
* FIFO eviction when capacity is exceeded
* Optionally persisted to /opt/hooknorton/data
* Raw body stored; UI may render JSON bodies as parsed JSON when applicable

4. API Surface
4.1 Developer API
Used by the UI and external automation.
Configuration endpoints
* Create/update/delete route configurations
* List all configured routes
* Load/save configurations from /opt/hooknorton/config
Request history endpoints
* Retrieve latest N requests
* Filter by path and/or method (optional)
* Clear request history
System endpoints
* Health check
* Reset configuration
* Reset request history(configuration and history resets are separate actions)
All endpoints are RESTful, JSON-based.

4.2 Fake API
* Catch-all HTTP handler
* Accepts all HTTP methods
* Matches requests against configured routes
* Records every request before responding
* Returns configured response or 404
No artificial latency or dynamic response logic.

4.3 Web UI
React single-page application for manual inspection only:
* View request history (latest N)
* Inspect request details (headers, raw body, parsed JSON view)
* Create/edit/delete route configurations
* Enable/disable routes
* Clear history and/or configuration
UI is a thin client over the Developer API.

5. Backend Components
* FakeApiController / Middleware
    * Handles all fake API traffic
* RouteMatcher
    * Simple pattern matching (glob-style)
* RouteConfigStore
    * In-memory representation + filesystem persistence
* RequestRecorder
    * Thread-safe, bounded request log
* PersistenceService
    * Reads/writes configs and history to /opt/hooknorton
* DeveloperApiControllers
    * Management and inspection endpoints
* Startup / Hosting
    * Configurable:
        * Max request history size (N)
        * Max request body size
        * Persistence enabled/disabled

6. Non-Functional Characteristics
* Deployment
    * Single container
    * Works with docker run, docker compose, .NET Aspire
* Persistence
    * Filesystem-based, optional
* Concurrency
    * Thread-safe in-memory structures
* Security
    * No authentication (local/dev usage)
* Observability
    * Basic structured logging to stdout

7. Explicit Non-Goals
* Performance or load testing
* Large-scale data retention
* Dynamic or scripted responses (for now)
* Authentication/authorization