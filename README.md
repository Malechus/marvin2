# M.A.R.V.I.N.
### Multiple Application, Resource Variable Information Node

M.A.R.V.I.N. is a .NET 8 home-automation suite that ties together a family chore tracker, Pi-hole network statistics, and a Discord bot into a single cohesive system. It is an update of the [original marvin Discord bot](https://github.com/Malechus/marvin), now expanded with a web interface and additional features.

---

## Solution Structure

The solution (`marvin2.sln`) contains three projects with the following relationships:

```
marvin2.sln
├── data/        # Class library — shared models, database context, EF Core migrations, and services
├── discord/     # Console app — Discord bot; references data/
└── web/         # ASP.NET Core MVC app — web UI; references data/
```

Both `discord` and `web` depend on `data`. Neither project contains its own database layer — all database access is handled through services defined in `data`.

---

## Projects

### data
A class library that serves as the shared core of the solution. It contains:
- **EF Core models**: `Chore`, `Person`, `PersonChore` (abstract base), `DailyChore`, `WeeklyChore`, `MonthlyChore`, `PersonScore`, `MediaFolderItem`
- **`ChoreContext`**: the EF Core `DbContext` using MySQL via Pomelo, with Table-Per-Hierarchy (TPH) mapping for chore types
- **`ChoreContextFactory`**: implements `IDesignTimeDbContextFactory` for EF tooling; accepts the environment name as its first argument
- **Services**: `ChoreService`, `PiService`, `ScoreService`, `MediaFolderService`, `LoggerService`

### discord
A long-running console application that connects to Discord as a bot. It registers and handles slash commands, and runs a daily timer that automatically announces chores at 06:00.

**Slash commands:**

| Command | Description |
|---------|-------------|
| `/listchores` | Lists today's chores for all household members, or optionally for one person |
| `/status` | Reports Pi-hole status, top DNS clients, and top blocked clients |
| `/hunt` | Part of the raccoon mini-game — attempt to catch a raccoon for a point |
| `/shoo` | Part of the raccoon mini-game — attempt to shoo a raccoon away for a point |

Slash commands respond immediately with a random acknowledgement string (to satisfy Discord's 3-second interaction deadline) and then post full results to the configured channel.

### web
An ASP.NET Core MVC application providing a browser-based interface. Features include:
- Dashboard showing Pi-hole blocking status, top DNS clients, top blocked clients, and today's chores
- Chore management page for viewing and adding daily, weekly, and monthly chores

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- A MySQL-compatible database server (e.g., [MariaDB](https://mariadb.org/))
- A [Discord application and bot token](https://discord.com/developers/applications)
- A self-hosted [Pi-hole](https://pi-hole.net/) instance with API access

---

## Configuration

All three projects share configuration files located at the **solution root**. Each project copies them to its output directory at build time. The active environment is selected via the `ASPNETCORE_ENVIRONMENT` environment variable (defaults to `Development`).

Create the following files as needed. **Do not commit files containing real values.**

### `appsettings.json`
Base configuration shared across all environments.

```json
{
    "PiHole": {
        "APIKey": "",
        "BaseAddress": ""
    },
    "MediaFolders": [
        { "Key": "", "DisplayName": "", "Path": "" }
    ],
    "Greetings": [],
    "Responses": [],
    "People": {
        "<discord-user-id>": "<display-name>"
    }
}
```

### `appsettings.{Environment}.json`
Environment-specific overrides (e.g., `appsettings.Development.json`, `appsettings.Production.json`).

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "Database": {
        "Server": "",
        "Port": 3306,
        "Username": "",
        "Password": "",
        "Database": "",
        "ConnectionString": ""
    },
    "Discord": {
        "Token": "",
        "ServerID": "",
        "Channels": {
            "Announce": "",
            "Chores_General": "",
            "Chore_Tracking": "",
            "Chore_List": "",
            "Media_Alerts": ""
        }
    },
    "DevEnv": false
}
```

**Key descriptions:**

| Key | Description |
|-----|-------------|
| `PiHole:APIKey` | API key for the Pi-hole instance |
| `PiHole:BaseAddress` | Base URL of the Pi-hole API (e.g., `https://<host>/api/`) |
| `MediaFolders` | Array of media library locations; each entry requires `Key`, `DisplayName`, and `Path` |
| `Greetings` | Array of strings randomly selected for bot startup messages |
| `Responses` | Array of strings randomly selected as slash command acknowledgements |
| `People` | Map of Discord user IDs (as strings) to display names used for chore assignment |
| `Database:ConnectionString` | Full MySQL connection string used by EF Core |
| `Discord:Token` | Bot token from the Discord developer portal |
| `Discord:ServerID` | ID of the Discord server (guild) the bot serves |
| `Discord:Channels:Announce` | Channel ID where daily chore announcements are posted |
| `Discord:Channels:Chore_List` | Channel ID where on-demand chore lists are posted |
| `DevEnv` | Set to `true` in development to enable development-specific behavior |

---

## Build & Run

```bash
# Build the entire solution
dotnet build

# Run the Discord bot
dotnet run --project discord

# Run the web app
dotnet run --project web
```

---

## Database Migrations

Migrations are managed from the `data` project. Pass the environment name as the first argument so the factory can load the correct connection string.

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project data -- Development

# Apply pending migrations
dotnet ef database update --project data -- Development
```

---

## Open Source Attributions

| Package | Author | License |
|---------|--------|---------|
| [Discord.Net](https://github.com/discord-net/Discord.Net) | Discord.Net Contributors | [MIT](https://github.com/discord-net/Discord.Net/blob/dev/LICENSE) |
| [Microsoft.EntityFrameworkCore](https://github.com/dotnet/efcore) | Microsoft | [MIT](https://github.com/dotnet/efcore/blob/main/LICENSE.txt) |
| [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) | Pomelo Foundation | [MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE) |
| [Microsoft.Extensions.Configuration](https://github.com/dotnet/runtime) | Microsoft | [MIT](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT) |
| [Microsoft.Extensions.Logging](https://github.com/dotnet/runtime) | Microsoft | [MIT](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT) |
| [System.ComponentModel.Annotations](https://github.com/dotnet/runtime) | Microsoft | [MIT](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT) |
| [ASP.NET Core](https://github.com/dotnet/aspnetcore) | Microsoft | [MIT](https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt) |