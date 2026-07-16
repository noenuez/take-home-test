# Frontend — Loan Management (Angular)

Lightweight Angular 19 app that displays the loans served by the backend API in a
Material table.

Generated with [Angular CLI](https://github.com/angular/angular-cli) 19.1.6.

## Prerequisites

The backend must be running and reachable. By default the app calls
`http://localhost:8080` (see `src/environments/environment.ts`). Start the API first —
for example, from `backend/src`:

```sh
docker compose up --build
```

The backend allows the `http://localhost:4200` origin via CORS (configurable with the
`FRONTEND_URL` env var).

## Running the Frontend

Install dependencies:

```sh
npm install
```

Start the development server:

```sh
npm start
```

Open `http://localhost:4200/`. The table loads the loans from `GET /loans`, with
loading, error (retry) and empty states, and a **Refresh** button.

## Configuration

| File | Purpose |
| --- | --- |
| `src/environments/environment.ts` | Dev API base URL (`http://localhost:8080`). |
| `src/environments/environment.production.ts` | Prod API base URL (`/api`). |

## Structure

```
src/app/
  models/loan.model.ts      # Loan interface
  services/loan.service.ts  # HttpClient wrapper for /loans
  app.component.*           # Loans table (standalone component)
```

## Build

```sh
npm run build
```
