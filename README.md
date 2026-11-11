# Where's our Drummer

A directory plus classifieds for musicians and bands. Musicians post profiles with instruments, skills, and sample music links. Bands post listings when they need a new or temporary member.

CSE325 Block 6 2026 group project.

## Tech stack

- C# / .NET 10, ASP.NET Core
- Blazor Web App (Interactive Server rendering)
- ASP.NET Core Identity (authentication)
- EF Core with PostgreSQL (Npgsql). Postgres is the only provider; changing it needs group agreement.
- Docker + Render for deployment
- xUnit tests

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- EF Core CLI: `dotnet tool install --global dotnet-ef`
- PostgreSQL, either via Docker (below) or installed natively (see "Running without Docker")

## Run locally (with Docker)

```
docker compose up -d
dotnet restore
dotnet ef database update --project src/WheresOurDrummer.Web
dotnet run --project src/WheresOurDrummer.Web
```

`docker compose up -d` starts Postgres 17 on port 5432 with dev-only credentials (see `docker-compose.yml`). They match the connection string in `src/WheresOurDrummer.Web/appsettings.Development.json`. Data persists in the `wod_pgdata` volume; `docker compose down -v` wipes it.

Instead of `dotnet ef database update` you can start the app with `RUN_MIGRATIONS=true` (PowerShell: `$env:RUN_MIGRATIONS = "true"`) to apply migrations and seed roles on startup.

## Running without Docker

1. Install PostgreSQL 17 from https://www.postgresql.org/download/ (remember the password you set for the `postgres` user; the installer listens on port 5432).
2. Create a database and a login role (run in `psql`, or pgAdmin's query tool):

   ```
   psql -U postgres -c "CREATE USER wod_dev WITH PASSWORD 'choose-a-password';"
   psql -U postgres -c "CREATE DATABASE wheresourdrummer OWNER wod_dev;"
   ```

3. Store the connection string in user-secrets (this overrides `appsettings.Development.json` and is never committed):

   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=wheresourdrummer;Username=wod_dev;Password=choose-a-password" --project src/WheresOurDrummer.Web
   ```

4. Apply migrations and run:

   ```
   dotnet ef database update --project src/WheresOurDrummer.Web
   dotnet run --project src/WheresOurDrummer.Web
   ```

## Resetting your local database after the Postgres switch

The project used SQLite before. If you have an old local database file, it is no longer used and can be deleted.

1. Pull the branch, then delete the old file: `src/WheresOurDrummer.Web/Data/wheresourdrummer.db` (and any `.db-shm` / `.db-wal` next to it). They are git-ignored, so this only affects your machine.
2. Start Postgres (`docker compose up -d`, or your native install).
3. Run `dotnet ef database update --project src/WheresOurDrummer.Web`.

The `InitialCreate` migration was regenerated for Postgres, so the old SQLite schema cannot be upgraded; start fresh. To reset a Postgres database later: `docker compose down -v` then repeat steps 2 and 3 (native install: `psql -U postgres -c "DROP DATABASE wheresourdrummer;"`, recreate it, then step 3).

## Run tests

```
dotnet test
```

## Migrations

Read the migration rule in [CONTRIBUTING.md](CONTRIBUTING.md) first (announce in the group chat; one open migration at a time). Migrations target PostgreSQL.

```
dotnet ef migrations add <Name> --project src/WheresOurDrummer.Web --output-dir Data/Migrations
dotnet ef database update --project src/WheresOurDrummer.Web
```

## Deployment (Render)

The app deploys to [Render](https://render.com) as a Docker web service defined by `render.yaml`, with Postgres.

1. Create a Postgres database (Render Postgres, or an external provider such as Neon or Supabase).
2. In Render choose New > Blueprint and select this repo; it reads `render.yaml`.
3. When prompted, paste the database URL (`postgres://...`) as `DATABASE_URL`. It is never committed.
4. Every push to `main` auto-deploys. On start the app applies migrations and seeds roles (`RUN_MIGRATIONS=true`), and Render health-checks `/healthz`.

The Dockerfile builds `src/WheresOurDrummer.Web` and runs it as a non-root user. Login cookie keys are stored in the database so restarts do not log everyone out.

> **Free tier warnings:** a free Render web service sleeps after 15 minutes idle (the next request takes a while to wake it), and a free Render Postgres database expires after 30 days. Use an external Postgres or a paid plan for anything long-lived.

### Environment variables

| Variable | Purpose | Default / notes |
| --- | --- | --- |
| `DATABASE_URL` | Postgres URI (`postgres://user:pass@host:port/db?sslmode=require`). Overrides `ConnectionStrings:DefaultConnection`. | Unset locally; set in the Render dashboard. SSL mode is `Prefer` unless `sslmode` is in the URL. |
| `RUN_MIGRATIONS` | If `true`, apply migrations and seed roles at startup. | Unset (skipped), also in Development. `true` on Render. |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production`. | `Production` in the Docker image and on Render. |
| `PORT` | Port to listen on (`http://0.0.0.0:$PORT`). | Render sets it; otherwise 8080 (or your launch profile locally). |

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
