# Eagle Bank

ASP.NET Core 10 REST API for the Barclays Eagle Bank take-home. Callers register with email and password, obtain a JWT, and manage only their own user, accounts, and transactions.

The submitted contract is [`openapi.yaml`](openapi.yaml). It keeps the original paths and schemas and adds `password` on create-user plus `POST /v1/auth/login` returning `{ "token" }`. Transaction ids use `^tan-[A-Za-z0-9]+$` (the original one-character pattern did not match the spec examples).

AI assistance was used on this submission. Design choices are recorded in `design/system-design.md` and `docs/adr/`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (this repo targets `net10.0`)
- `curl` or any HTTP client (optional, for the examples below)

No commercial NuGet packages, AutoMapper, or MediatR. Restore uses MIT / Apache / BSD / first-party Microsoft packages only. A reviewer does not need a licence key.

## Run tests

From the repository root:

```bash
dotnet test EagleBank.sln
```

That runs both suites in `tests/EagleBank.Tests`:

- **xUnit** units (`Money`, exception mapping, ownership, account-number format, domain apply, concurrent withdrawal)
- **Reqnroll** HTTP acceptance (`Features/*.feature`) against `WebApplicationFactory` with an isolated SQLite file per scenario

## Run the API

Committed `appsettings.json` and `appsettings.Development.json` leave `Jwt:SigningKey` empty. Startup fails unless the key is at least 32 characters. Do not commit a real secret.

```bash
export Jwt__SigningKey='replace-with-a-long-random-secret-key'
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project src/EagleBank.Api
```

Alternatively: `dotnet user-secrets set "Jwt:SigningKey" "replace-with-a-long-random-secret-key" --project src/EagleBank.Api`.

The Development profile listens on `http://localhost:5080`. Migrations apply on startup. Check `GET /health`.

Swagger UI is at [http://localhost:5080/swagger](http://localhost:5080/swagger). It loads the submitted [`openapi.yaml`](openapi.yaml) (including login), not a generated spec. The raw file is also at `GET /openapi.yaml`. The UI banner and the spec description list which operations are wired in this repository.

Tokens last 60 minutes. `sub` is the `userId`.

### SQLite

The API uses `Data Source=data/eagle-bank.db` under the API content root. The `data/` directory is gitignored.

To reset local data, stop the process and delete the database files:

```bash
rm -f src/EagleBank.Api/data/eagle-bank.db src/EagleBank.Api/data/eagle-bank.db-*
```

The next `dotnet run` recreates an empty schema.

### Docker

JWT must be supplied. There is no default secret in the image or Compose file.

```bash
export Jwt__SigningKey='replace-with-a-long-random-secret-key'
docker compose up --build
```

Then `GET http://localhost:5080/health`. Stop with `Ctrl+C` or `docker compose down`.

## Authenticate

Create-user and login are public. Every other `/v1` route requires `Authorization: Bearer <jwt>`.

### 1. Create a user

```bash
curl -s -X POST http://localhost:5080/v1/users \
  -H 'Content-Type: application/json' \
  -d '{
    "name": "Ada Lovelace",
    "address": {
      "line1": "Stoney Street",
      "town": "London",
      "county": "Greater London",
      "postcode": "SE1 9TG"
    },
    "phoneNumber": "+441234567890",
    "email": "ada@example.com",
    "password": "S3cretPass!"
  }'
```

`201` returns a `UserResponse` (no password). Duplicate email is `400`.

### 2. Login

```bash
curl -s -X POST http://localhost:5080/v1/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"ada@example.com","password":"S3cretPass!"}'
```

`200` body: `{ "token": "<jwt>" }`. Unknown email and wrong password share one `401`.

### 3. Call a protected route

```bash
TOKEN='<paste token>'

curl -s http://localhost:5080/v1/users/<userId> \
  -H "Authorization: Bearer $TOKEN"
```

Then `POST /v1/accounts` with `{ "name", "accountType": "personal" }`, and deposit with `POST /v1/accounts/{accountNumber}/transactions` (`amount`, `currency`: `GBP`, `type`: `deposit`).

## What is implemented

The submitted `openapi.yaml` operations are wired, including list, PATCH, and DELETE.

| Method | Path | Behaviour |
|---|---|---|
| POST | `/v1/users` | Public create |
| GET | `/v1/users/{userId}` | Own user (`403` / `404`) |
| PATCH | `/v1/users/{userId}` | Partial update; `updatedTimestamp` changes |
| DELETE | `/v1/users/{userId}` | `204` if no accounts; `409` if any remain |
| POST | `/v1/auth/login` | `{ "token" }` |
| POST | `/v1/accounts` | Create personal GBP account |
| GET | `/v1/accounts` | Caller's accounts only (`200` / `401`) |
| GET | `/v1/accounts/{accountNumber}` | Own account (`^01\d{6}$`) |
| PATCH | `/v1/accounts/{accountNumber}` | `name` and/or `accountType` |
| DELETE | `/v1/accounts/{accountNumber}` | `204`; transactions are removed |
| POST | `/v1/accounts/{accountNumber}/transactions` | Deposit and withdrawal |
| GET | `/v1/accounts/{accountNumber}/transactions` | Owner's transactions |
| GET | `/v1/accounts/{accountNumber}/transactions/{transactionId}` | Fetch one |

Deposit over `10000.00` or a withdrawal with insufficient funds returns `422`. Money is GBP, two decimals, persisted as integer pence. Errors use the OpenAPI bodies (`400` includes `details`).

## Logging

Logs are structured across API, application, domain, and persistence. Correlate with `RequestId` (`X-Request-Id` header or generated). After authentication, the request log scope includes `userId` (`usr-…`) only.

Logs must not contain email, name, phone, address, password, JWT, request bodies, transaction `reference`, account display name, or the full `accountNumber`. EF sensitive-data logging is off.

## Production follow-ups (not in this repo)

These are documented only — they are not implemented:

- Rate limiting (especially login and signup) and `429`
- SAST / SCA in CI
- PostgreSQL instead of a SQLite file
- No application cache of `balance` (the live row is the source of truth)

## Layout

```
src/EagleBank.Api             HTTP, JWT, exception pipeline
src/EagleBank.Application     one handler per use case, ports, validators
src/EagleBank.Domain          User, BankAccount, Transaction, Money
src/EagleBank.Infrastructure  EF Core + SQLite, password hash, JWT issue
tests/EagleBank.Tests         xUnit + Reqnroll
openapi.yaml                  submitted contract
```
