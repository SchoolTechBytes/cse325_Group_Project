# Contributing

## Branches and pull requests

- Branch names: `feature/<card-slug>` (for example `feature/musician-list`).
- One reviewer per PR.
- No direct pushes to `main` after the initial setup; everything goes through a PR.
- Before opening a PR, run `dotnet build` and `dotnet test`, and fill in the PR template checklist.
- Do not add features outside your assigned Trello card.

## Secrets

Use `dotnet user-secrets` only. Never commit passwords, keys, connection strings for shared databases, or `.db` files.

```
dotnet user-secrets set "Key" "value" --project src/WheresOurDrummer.Web
```

## Migrations

- Announce in the group chat **before** adding a migration.
- Only one person has an open migration at a time. Merge it to `main` before the next person starts one.
- Teammates pull `main` and run `dotnet ef database update` after a migration lands.

## Naming

- Code name for projects, namespaces, and folders: `WheresOurDrummer`.
- Display name for users: use `AppInfo.Name` (never hardcode "Where's our Drummer", and never put an apostrophe in paths or commands).
