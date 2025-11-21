Development: running API and Web client

This repository contains multiple projects:
- `PoemApp.API` - ASP.NET Core Web API (hosts the Blazor WebAssembly client when published/built as Hosted)
- `PoemApp.Web` - Blazor WebAssembly client (can be run standalone during development)

Two development modes are supported:

1) Hosted (single process) - API serves the client
- This mode builds/publishes the client static assets into `PoemApp.API/wwwroot` and runs the API which serves the client.
- Run:
  - Stop any running PoemApp.API process if present (see note below).
  - dotnet run --project PoemApp.API
- Notes:
  - The API project contains an MSBuild target that publishes the client and copies client `wwwroot` files into `PoemApp.API/wwwroot` during the API build.
  - If you get a file lock error when building ("file is being used by another process"), stop the running PoemApp.API.exe first (Task Manager or the provided script `stop-api.ps1`).

2) Separate (recommended during active client development)
- Run the API and Web client as two independent processes. The client will call the API using the configured ApiBaseUrl.
- Steps:
  1. Start the API server (default uses ports from `PoemApp.API` launch settings):
     - dotnet run --project PoemApp.API
  2. Start the Web client:
     - dotnet run --project PoemApp.Web
  3. Ensure `PoemApp.Web/wwwroot/appsettings.Development.json` contains the `ApiBaseUrl` value pointing to the running API (example `https://localhost:5001/`). The Web project will prefer this configuration during development.
- Notes:
  - CORS is already configured in the API to allow development requests.

Stopping a running API process (Windows)
- Use Task Manager to find and stop `PoemApp.API` (or `PoemApp.API.exe`).
- Or use PowerShell script included: `stop-api.ps1` (run in an elevated PowerShell if necessary):
  - .\stop-api.ps1

Scripts
- `run-dev.ps1` - starts API and Web client in separate windows (PowerShell) for development.
- `stop-api.ps1` - stops any running PoemApp.API processes by name.

Troubleshooting: file locked on rebuild
- Cause: a previous run of the API is still executing and locks binaries in `bin/Debug/net10.0`.
- Fix:
  - Stop the running process (Task Manager or `stop-api.ps1`).
  - Re-run the build / dotnet run command.

If you'd like, I can add VS launch profiles or configure a `dotnet watch` setup to auto-restart on changes. Reply if you want that added.