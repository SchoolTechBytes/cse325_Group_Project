# Where's our Drummer

A directory plus classifieds for musicians and bands. Musicians post profiles with instruments, skills, and sample music links. Bands post listings when they need a new or temporary member.

CSE325 Block 6 2026 group project.

## Tech stack

- C# / .NET 10, ASP.NET Core
- Blazor Web App (Interactive Server rendering)
- ASP.NET Core Identity (authentication)
- EF Core with SQLite for local development (provider swappable for Azure SQL; see `Program.cs`)
- xUnit tests

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

## Run locally

```
dotnet restore
dotnet ef database update --project src/WheresOurDrummer.Web
dotnet run --project src/WheresOurDrummer.Web
```

The SQLite database file (`Data/wheresourdrummer.db`) is created by the migration step and is git-ignored. The connection string lives in `src/WheresOurDrummer.Web/appsettings.Development.json`. Use `dotnet user-secrets` for anything sensitive.

## Run tests

```
dotnet test
```

## Migrations

Read the migration rule in [CONTRIBUTING.md](CONTRIBUTING.md) first (announce in the group chat; one open migration at a time).

```
dotnet ef migrations add <Name> --project src/WheresOurDrummer.Web --output-dir Data/Migrations
dotnet ef database update --project src/WheresOurDrummer.Web
```

## Project layout

- `src/WheresOurDrummer.Web`: the app (`Components/`, `Data/`, `Services/`)
- `tests/WheresOurDrummer.Tests`: unit tests

## Links

- Trello board: https://trello.com/b/rwR0tV4U/cse-325-net-software-development
- GitHub repo: https://github.com/SchoolTechBytes/cse325_Group_Project
- Live site: _TBD_
- Video: _TBD_

## Collaborators

- Nicholas Nixon
