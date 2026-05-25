# M.A.R.V.I.N. – Copilot Instructions

## Project Overview

M.A.R.V.I.N. (Multiple Application, Resource Variable Information Node) is a .NET 8 home-automation suite comprising three projects in one solution (`marvin2.sln`):

| Project | Type | Purpose |
|---------|------|---------|
| `data/` | Class library | Shared EF Core models, `ChoreContext`, services (`ChoreService`, `PiService`), and migrations |
| `discord/` | Console app | Discord.Net bot with slash commands and a daily chore-announcement timer |
| `web/` | ASP.NET Core MVC | Web UI for viewing Pi-hole stats and managing chores in the database |

Both `discord` and `web` reference `data`. Neither has its own database layer—all DB access goes through `data`.

## Build & Run

```bash
# Build the entire solution
dotnet build

# Run the Discord bot
dotnet run --project discord

# Run the web app
dotnet run --project web
```

There are no test projects in the solution.

## Configuration

All three projects share config files at the **solution root** (`appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`). Each project copies them to its output directory on build. The active environment is selected via `ASPNETCORE_ENVIRONMENT` (defaults to `Development`).

Required configuration keys:

```
Database:ConnectionString       # Full MySQL connection string (used by ChoreService / web)
Database:Server                 # Used directly by ChoreContext.OnConfiguring as fallback
Database:UserID
Database:Password
Database:Database
Discord:Token
Discord:ServerID
Discord:Channels:Announce
Discord:Channels:Chore_List
PiHole:BaseAddress
PiHole:APIKey
Greetings                       # JSON array of startup greeting strings
Responses                       # JSON array of slash-command acknowledgement strings
```

## Database & Migrations

- MySQL via **Pomelo.EntityFrameworkCore.MySql**.
- `PersonChore` is an abstract `[NotMapped]` base class. `DailyChore`, `WeeklyChore`, and `MonthlyChore` derive from it and are mapped with **Table-Per-Hierarchy (TPH)** via `ChoreContext.OnModelCreating`.
- `ChoreContextFactory` (implements `IDesignTimeDbContextFactory`) is used by EF tooling. It expects the environment name as the first argument:

```bash
dotnet ef migrations add <MigrationName> --project data -- Development
dotnet ef database update --project data -- Development
```

## Discord Bot Architecture

- **`Startup.cs`** (in `discord/`) is the DI root. All services are registered as singletons.
- **`StartupService`** handles login, slash command registration (`Client_Ready`), and a daily timer that fires at 06:00 to post chores automatically.
- **Slash command pattern**: every command implements `ISlashCommandHandler` with three methods:
  - `CreateBuilder()` – returns a configured `SlashCommandBuilder` for registration.
  - `HandleCommand(command, channel)` – invoked when a user runs the slash command.
  - `TriggerResponse(channel)` – invoked by the timer (no user interaction).
- **Response strategy**: slash commands *immediately* `RespondAsync` with a random string from `ResponseService` (to beat Discord's 3-second interaction timeout), then perform the real work by posting to a `SocketTextChannel` directly. This is intentional—do not change this pattern without considering the timeout.
- Text-command infrastructure (`CommandHandler`, `ChoreModule`) exists but `MessageReceived` is commented out; the bot currently uses slash commands exclusively.

## Key Conventions

- **Public/private method mirroring**: services in `data/` use a public method (PascalCase) that delegates to a private method (camelCase) with the same logic. E.g., `GetDailyChores()` calls `getDailyChores()`. Follow this pattern when adding new service methods.
- **`PiService` HttpClient**: uses `DangerousAcceptAnyServerCertificateValidator` to trust the Pi-hole's self-signed cert. This is intentional and scoped to local Pi-hole calls only. Never reuse this `HttpClient` for external requests.
- **`PiService` authentication**: authenticates in the constructor synchronously (`.Result`). The session id is added as a default request header (`"sid"`). Methods are sync wrappers over private async implementations (same public/private pattern as above).
- **Chore filtering at runtime**: `ListChores` loads all `PersonChore` entries and switches on `GetType().Name` to filter by today's day/date. Filtering is done in-memory, not in the query.
- **XML doc comments**: all public types and members in `data/` and `discord/` carry `<summary>` doc comments. Continue this practice for any new public API.
