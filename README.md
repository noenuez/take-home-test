# Loan Management System

Full-stack loan management application: a **.NET 10 / PostgreSQL** REST API and an
**Angular 19** frontend. The whole stack runs with a single Docker command.

> Original brief is preserved in [TASK.md](./TASK.md).
> The brief mentioned SQL Server; this implementation uses **PostgreSQL** as agreed.

## Quick start (one command)

Requires Docker. From the repository root:

```sh
docker compose up --build
```

This starts three containers — PostgreSQL, the API, and the web app (nginx). On boot
the API applies its FluentMigrator migrations and seeds sample loans.

Then open:

```
http://localhost:8080
```

The Angular app is served by nginx, which reverse-proxies `/api/*` to the backend
(same origin — no CORS needed in this setup).

Tear everything down (including the database volume):

```sh
docker compose down -v
```

## What it does

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/loans` | Create a loan (`currentBalance` starts equal to `amount`, status `Active`). |
| `GET` | `/loans/{id}` | Retrieve a single loan. |
| `GET` | `/loans` | List all loans. |
| `POST` | `/loans/{id}/payment` | Deduct a payment from `currentBalance`; status flips to `Paid` at zero. |

The frontend shows all loans in a table and includes a **New Loan** form.

## Architecture

```
frontend (Angular 19, nginx)
        │  /api  →  reverse proxy
        ▼
backend  (.NET 10 Web API)
  Controller → MediatR (CQRS) → ValidationBehavior (FluentValidation)
             → Command/Query Handler → Repository + IUnitOfWork
             → EF Core → PostgreSQL
```

Backend solution (`backend/src`):

| Project | Responsibility |
| --- | --- |
| `Fundo.Applications.WebApi` | HTTP host: controllers, DI, exception handling, migration runner. |
| `Fundo.Applications.Domain` | CQRS commands/queries, handlers, validators, DTOs. |
| `Fundo.Applications.Data` | `FundoDbContext`, entities, EF config, FluentMigrator migrations, repositories. |
| `Fundo.Services.Tests` | xUnit unit + integration tests. |

Key choices:

- **CQRS** via MediatR; **repository pattern** + unit of work over EF Core.
- **FluentValidation** runs in the MediatR pipeline; failures → RFC 9110 `problem+json`
  **400**. Domain errors map to **404** / **409**.
- **FluentMigrator** owns the schema and seed data; migrations run automatically on
  startup (with retry while the database is coming up).
- Auditing (`CreatedBy/At`, `UpdatedBy/At`) applied automatically on save.

## Tests

```sh
cd backend/src
dotnet test
```

- **Unit tests** (Moq): command/query handlers (balance math, status transitions,
  not-found / business-rule errors) and validators.
- **Integration tests**: full HTTP + CQRS pipeline via `WebApplicationFactory` with an
  in-memory database — no PostgreSQL required.

## Local development (without Docker)

Requires **.NET 10 SDK**, **Node 20**, and a PostgreSQL instance.

Backend:

```sh
cd backend/src/Fundo.Applications.WebApi
cp .env.example .env   # set DB_CONNECTION, e.g. Host=localhost;Port=5432;Database=fundo;Username=postgres;Password=postgres
dotnet run
```

Frontend (dev server on http://localhost:4200, calls the API at http://localhost:8080):

```sh
cd frontend
npm install
npm start
```

Component-level notes: [backend/src/README.md](./backend/src/README.md) ·
[frontend/README.md](./frontend/README.md).

## Continuous integration

`.github/workflows/ci.yml` builds and tests the backend and builds the frontend on
every push / pull request to `main`.

## Notes & possible improvements

- **Authentication** is not implemented (bonus). The pipeline is ready for it —
  auditing already reads the current user.
- **Structured logging** could be added (Serilog) for richer request/error logs.
- The frontend covers the required loans table plus a create form; a **payment** action
  per row would be a natural next step (the API already supports it).
