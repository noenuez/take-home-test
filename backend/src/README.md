# Fundo — Loan Management API

RESTful API for managing loan applications, built with **.NET 10**, **PostgreSQL**,
**Entity Framework Core** and **FluentMigrator**. The application code is organized
around **CQRS** (MediatR) with the **repository pattern** and **FluentValidation**.

> The original brief mentioned SQL Server; this implementation uses **PostgreSQL** as agreed.

## Architecture

The solution is split into three projects plus a test project:

| Project | Responsibility |
| --- | --- |
| `Fundo.Applications.WebApi` | HTTP host: controllers, DI wiring, exception handling, migration runner. |
| `Fundo.Applications.Domain` | Application layer: CQRS commands/queries, handlers, validators, DTOs. |
| `Fundo.Applications.Data` | Persistence: `FundoDbContext`, entities, EF configurations, FluentMigrator migrations, repositories. |
| `Fundo.Services.Tests` | xUnit unit + integration tests. |

**Request flow**

```
Controller → MediatR (ISender.Send)
           → ValidationBehavior (FluentValidation)
           → Command/Query Handler
           → IRepository / IWritableRepository + IUnitOfWork
           → FundoDbContext (PostgreSQL)
```

Cross-cutting concerns:

- **Validation** runs as a MediatR pipeline behavior; failures become an RFC 9110
  `application/problem+json` **400** response.
- **Domain errors** map to **404** (`NotFoundException`) and **409** (`DomainException`)
  via `IExceptionHandler`s.
- **Auditing** (`CreatedBy/CreatedAt/UpdatedBy/UpdatedAt`) is applied automatically in
  `SaveChanges`.
- **Migrations** (schema + seed) run automatically on startup with FluentMigrator
  (skipped in the `Testing` environment).

## Endpoints

| Method | Route | Description | Success |
| --- | --- | --- | --- |
| `POST` | `/loans` | Create a loan (`currentBalance` starts equal to `amount`, status `Active`). | `201 Created` |
| `GET` | `/loans/{id}` | Retrieve a single loan. | `200 OK` / `404` |
| `GET` | `/loans` | List all loans (newest first). | `200 OK` |
| `POST` | `/loans/{id}/payment` | Deduct an amount from `currentBalance`; status flips to `Paid` at zero. | `200 OK` / `404` / `409` |

**Loan shape**

```json
{
  "id": "6f9619ff-8b86-d011-b42d-00cf4fc964f1",
  "applicantName": "Maria Silva",
  "amount": 1500.00,
  "currentBalance": 500.00,
  "status": "Active",
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": null
}
```

**Create body** — `{ "applicantName": "Maria Silva", "amount": 1500.00 }`
**Payment body** — `{ "amount": 100.00 }`

## Run with Docker (recommended)

The full stack (PostgreSQL + API + frontend) runs from the **repository root** with a
single command:

```sh
docker compose up --build
```

The app is served at `http://localhost:8080` and the API is reachable through it at
`http://localhost:8080/api/loans`. On boot the API applies all migrations and seeds
six sample loans. See the [root README](../../README.md) for details.

## Run locally (without Docker)

Requirements: **.NET 10 SDK** and a reachable PostgreSQL instance.

1. Provide the connection string. Copy the example env file and adjust it:

   ```sh
   cd Fundo.Applications.WebApi
   cp .env.example .env
   ```

   `.env`:

   ```
   ASPNETCORE_ENVIRONMENT=Development
   DB_CONNECTION=Host=localhost;Port=5432;Database=fundo;Username=postgres;Password=postgres
   ```

   The connection string is read from the `DB_CONNECTION` environment variable
   (loaded from `.env` via DotNetEnv).

2. Run the API (migrations + seed run automatically on startup):

   ```sh
   dotnet run
   ```

## Tests

From `backend/src`:

```sh
dotnet test
```

- **Unit tests** cover the command/query handlers (balance math, status transitions,
  not-found / business-rule errors) and the FluentValidation validators, using Moq.
- **Integration tests** exercise the full HTTP + CQRS pipeline through
  `WebApplicationFactory`, swapping PostgreSQL for an in-memory provider — no database
  required.

## Configuration reference

| Variable | Description | Default (compose) |
| --- | --- | --- |
| `DB_CONNECTION` | PostgreSQL connection string. | `Host=postgres;Port=5432;Database=fundo;Username=postgres;Password=postgres` |
| `ASPNETCORE_ENVIRONMENT` | Environment name. `Testing` disables the migration runner. | `Development` |
