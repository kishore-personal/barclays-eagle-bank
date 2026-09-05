# Implementation Tasks

## Status legend

- `TODO`
- `IN_PROGRESS`
- `BLOCKED`
- `DONE`

## Minimum viable submission

These **MUST** tasks are the approved MVP (create/fetch user, account, transaction + login):

`TASK-001` → `TASK-008`

Do not implement Redis, a message broker, in-app rate limiting, or a SAST pipeline (ADR-0009, ADR-0010, ADR-0011). Mention those as production follow-ups in the README (`TASK-008`).

## Priority

- `MUST` — required for a valid submission
- `SHOULD` — stretch, preferred order after MVP
- `COULD` — optional if time remains

## HTTP verb and status alignment

Checked against `openapi.yaml` (authoritative shape) and approved requirements. `500` is handled once in TASK-002, not retested per endpoint. Login is a required spec extension (not in the original file).

| Method | Path | Success | Errors in contract / brief | Task | Notes |
|---|---|---|---|---|---|
| POST | `/v1/users` | 201 | 400, 500 | TASK-004 | Public. Duplicate email is 400 (Q-004), not 409. |
| POST | `/v1/auth/login` | 200 `{ token }` | 400, 401 | TASK-004 | Add to submitted OpenAPI. |
| GET | `/v1/users/{userId}` | 200 | 400, 401, 403, 404 | TASK-005 | |
| PATCH | `/v1/users/{userId}` | 200 | 400, 401, 403, 404 | TASK-011 | Partial body. |
| DELETE | `/v1/users/{userId}` | 204 | 400, 401, 403, 404, **409** | TASK-012 | 409 only when accounts exist. |
| POST | `/v1/accounts` | 201 | 400, 401, 403, 500 | TASK-006 | 403 is in the spec; no brief scenario — map it, no forced test. |
| GET | `/v1/accounts` | 200 | **401 only** (+500) | TASK-010 | Not 403/404: list is always the caller's set. |
| GET | `/v1/accounts/{accountNumber}` | 200 | 400, 401, 403, 404 | TASK-006 | Path param is `accountNumber`. |
| PATCH | `/v1/accounts/{accountNumber}` | 200 | 400, 401, 403, 404 | TASK-011 | No 409. Cannot change balance. |
| DELETE | `/v1/accounts/{accountNumber}` | 204 | 400, 401, 403, 404 | TASK-012 | **No 409** (A-010). |
| POST | `/v1/accounts/{accountNumber}/transactions` | 201 | 400, 401, 403, 404, **422** | TASK-007, TASK-009 | 422: insufficient funds or deposit cap (Q-006). |
| GET | `/v1/accounts/{accountNumber}/transactions` | 200 | 400, 401, 403, 404 | TASK-010 | |
| GET | `/v1/accounts/{accountNumber}/transactions/{transactionId}` | 200 | 400, 401, 403, 404 | TASK-007 | Wrong-account txn → 404, not 403. |

There are no PUT or transaction PATCH/DELETE operations (REQ-TXN-014). Production `429` is not a take-home task (ADR-0011).

## Logging at every layer (no PII) — applies to TASK-002–TASK-012

Required by ADR-0007. TASK-002 installs the hooks; each later feature task must **use** them, not only the API layer.

| Layer | Must log | Must never log |
|---|---|---|
| Api | Method, **route template**, status, elapsed ms, `RequestId`, exception type | Raw URL, query string, `Authorization`, bodies |
| Application | Handler/command or query name, result code (`Created`, `Forbidden`, `NotFound`, …), `userId` (`usr-…`) when authenticated | Email, name, phone, address, password, account display name, `reference` |
| Domain | Invariant **reason code** only (`InsufficientFunds`, `BalanceCap`, `InvalidAmountScale`) | Amounts, full `accountNumber`, personal fields |
| Infrastructure | Entity type, persist committed/rolled back, rows affected, JWT issued (no token string) | SQL parameter values, email, password hash, EF sensitive data |

**Allow-list:** `RequestId`, `usr-…`, `tan-…`, enums, HTTP status, result codes.  
**Denylist:** name, email, phone, address, password/hash, JWT/signing key, bodies, transaction `reference`, account `name`, full `accountNumber`.

`EnableSensitiveDataLogging` stays off. Feature-task tests that hit create-user, login, or account/transaction writes must not find denylist strings in the test log sink.

## Exception handling framework — applies to TASK-002 onward

Required by ADR-0012. One maintainable pipeline, injected via DI:

- Base `EagleBankException` with safe message + reason code.
- Typed subtypes (`NotFound`, `Forbidden`, `Conflict`, `InsufficientFunds`, `BalanceCap`).
- Injected `IExceptionResponseFactory` maps type → HTTP status + OpenAPI body.
- Single `IExceptionHandler` registered in the ASP.NET pipeline (not try/catch in controllers).
- FluentValidation failures use the same factory (`400` + `details`).
- Unknown exceptions → `500` fixed safe message; log exception type only.
- Command/query handlers **throw**; they do not set `Results`/`StatusCode` for business errors.
- Adding a new failure is a new type + mapper convention — no controller edits.

JWT `401` stays in auth middleware.

## Unit tests and Gherkin acceptance tests — applies to TASK-001 onward

Required by ADR-0006 and ADR-0013. Two complementary suites, both run by `dotnet test`:

| Kind | Tool | Lives in | Use for |
|---|---|---|---|
| Unit | xUnit | `tests/EagleBank.Tests/Unit/` | `Money`, exception factory/handler, optional handler fakes |
| HTTP acceptance | Reqnroll (Gherkin) on xUnit | `tests/EagleBank.Tests/Features/` + `StepDefinitions/` + `Support/` | Brief Given/When/Then scenarios and REQ-TEST-001–003 |

**Do not** use SpecFlow (unmaintained). Use **Reqnroll** + `Reqnroll.xUnit`.

**Host:** shared `WebApplicationFactory` + isolated SQLite per scenario (ADR-0006). Step definitions issue HTTP calls; they do not re-implement domain rules.

**Wording:** copy the brief’s scenario intent; correct identifiers and gates already approved (`accountNumber` not `accountId`; create-user is public; PATCH is partial; duplicate email is `400`; account delete has no `409`; transaction ids `tan-` + one-or-more alphanumeric).

**Shared steps** (generic; add as needed):

- `Given I am not authenticated`
- `Given I am authenticated as a registered user`
- `Given another registered user exists`
- `Given I have a personal bank account`
- `Given that account has a balance of {decimal}`
- `When I create a user with all the required data`
- `When I create a user with missing required data`
- `When I send a {word} request to {string}` (or named When steps per resource)
- `Then I receive a {int} response`
- `Then the response is a UserResponse / BankAccountResponse / TransactionResponse / token payload`
- `Then the response is an ErrorResponse / BadRequestErrorResponse`
- `Then the account balance is {decimal}`
- `Then the test log sink contains no denylist PII`

**Feature files to add incrementally** (do not implement stretch scenarios before their task):

| File | Task | Scenarios (MUST unless marked stretch) |
|---|---|---|
| `Features/Users.feature` | 004, 005, 011, 012 | Create user 201; create 400 missing/invalid; create 400 duplicate email; fetch own 200; fetch 401; fetch other 403; fetch missing 404; fetch bad id 400. Stretch: PATCH 200/400/401/403/404; DELETE 204/409/401/403/404 |
| `Features/Auth.feature` | 004 | Login 200 `{ token }`; login 400 malformed; login 401 unknown email or bad password (same outcome) |
| `Features/Accounts.feature` | 006, 010, 011, 012 | Create 201; create 400; create 401; fetch own 200; fetch 401/403/404/400. Stretch: list 200 (caller only) / 401; PATCH 200/400/401/403/404; DELETE 204 then GET 404 / 401/403/404 |
| `Features/Transactions.feature` | 007, 009, 010 | Deposit 201 + balance increased; create 400; create 401/403/404; fetch txn 200; fetch 401/400/403; fetch unknown or wrong-account txn 404; deposit over cap 422 + unchanged balance. Stretch: withdrawal 201; withdrawal 422 + no row + same balance; list txns 200/400/401/403/404 |

PII log-sink checks may be a Gherkin Then or an xUnit unit/integration assertion; they must run for create-user and login.

TASK-013 concurrent withdrawal remains **xUnit**, not Gherkin.

## SOLID — applies to TASK-001 onward

Required by ADR-0014 (and ADR-0001, ADR-0008, ADR-0012). Design already uses these ideas; this section makes them **implementation rules**. New features must extend by adding types, not by growing god classes.

| Principle | Must | Must not |
|---|---|---|
| **S** Single responsibility | Controller = bind + dispatch. One command or query handler per use case. Domain = money/invariants. Infrastructure = EF/JWT/hash. Exception factory = HTTP mapping only. | `UserService` / `AccountService` with create+get+update+delete. Business rules or SQL in controllers. |
| **O** Open/closed | Stretch work is a new handler (and validator) file. New failure = new `EagleBankException` subtype + mapper convention. | Edit Create/Get handlers to add PATCH/DELETE/list. Per-action `try/catch` that sets status. |
| **L** Liskov substitution | Infrastructure implementations honour Application ports (same success and typed exceptions). Exception subtypes remain safely mappable as `EagleBankException`. | Production types that break the port (swallow errors, return null where the contract throws). |
| **I** Interface segregation | Action injects only the handler it calls. Ports are narrow (`IUserStore`, `IJwtTokenService`, `IPasswordHasher` — or equivalent). Ownership is a small shared helper. | `IMediator` as the controller dependency. One interface with unused CRUD methods. |
| **D** Dependency inversion | Application defines ports; Infrastructure implements; Api is the composition root. | Application or Domain reference `Microsoft.AspNetCore.*` or `Microsoft.EntityFrameworkCore`. Domain referencing Infrastructure. |

**Reference direction (DIP):** Domain ← Application ← Infrastructure; Api → Application + Infrastructure (registration only). Domain has **no** project references outward.

**Allowed exception to “one class per verb”:** one `CreateTransaction` handler for deposit and withdrawal (one use case). Deposit vs withdraw **invariants** stay on the domain account.

Shared ownership / email-uniqueness helpers are required so SRP does not become copy-paste (already in TASK-005).

TASK-016 includes a SOLID review: no fat services, no controller rules, no Application→EF/ASP.NET references.

## Dependency licences — applies to TASK-001 onward

Required by ADR-0015. This repo is a public POC / take-home. **No commercial, RPL, or licence-key packages.**

| Rule | Detail |
|---|---|
| Allow | MIT, Apache-2.0, BSD, or first-party Microsoft/.NET (ASP.NET, EF Core, SQLite provider, JWT, `PasswordHasher<T>`, xUnit, `Mvc.Testing`, FluentValidation, Reqnroll) |
| Map DTOs | Hand-written mappers next to the use case. **No AutoMapper** (15+ is RPL/commercial; do not pin 14.x as a workaround) |
| Dispatch | Explicit handler injection. **No MediatR** (13+ is RPL/commercial; already rejected in ADR-0008) |
| Do not add | Mapster (unneeded), Duende IdentityServer, paid UI kits, or any package that needs `LicenseKey` / NuGet licence acceptance |
| Do not commit | Licence keys, `*.lic`, or accept-license artefacts |

`dotnet restore` must succeed for a reviewer with no extra agreement. TASK-016 reviews `.csproj` package IDs against this list.

---

## TASK-001 — Bootstrap .NET 10 solution

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Create the solution and empty projects so `dotnet build` and `dotnet test` succeed.
- **Requirement references:** REQ-NFR-001, REQ-NFR-002, REQ-TEST-004
- **Design / ADR references:** ADR-0001, ADR-0006, ADR-0013, ADR-0014, ADR-0015; `design/system-design.md` sections 4 and 13
- **Dependencies:** None
- **Implementation scope:**
  - `EagleBank.sln` with `src/EagleBank.Api`, `src/EagleBank.Application`, `src/EagleBank.Domain`, `src/EagleBank.Infrastructure`, `tests/EagleBank.Tests`
  - Target `net10.0`; wire project references for DIP: Application → Domain only; Infrastructure → Application + Domain; Api → Application + Infrastructure; Domain → nothing; Tests → Api
  - Api is the only composition root (DI registration). Do not add a `UserService` / facade in Application.
  - Test project folders: `Features/`, `StepDefinitions/`, `Support/`, `Unit/`
  - Add Reqnroll + `Reqnroll.xUnit` (not SpecFlow) and `Microsoft.AspNetCore.Mvc.Testing`. Do **not** add AutoMapper or MediatR. Every NuGet ID must be permissive (ADR-0015).
  - Api is an ASP.NET Core Web API that starts and returns a non-business health or root response if useful
  - JWT signing key from configuration / user-secrets / env; do not commit a real secret
  - Update `.gitignore` for `bin/`, `obj/`, SQLite files, user-secrets leftovers
- **Test scope:** xUnit smoke that the test host starts if practical; otherwise `dotnet build` plus the test project compiling with Reqnroll packages. Feature files may be empty until TASK-002/004.
- **Verification / definition of done:** `dotnet build` succeeds. `dotnet test` runs the test project (even if only a placeholder).
- **Verification evidence:** 2026-09-05 — `dotnet build EagleBank.sln` succeeded (0 warnings, 0 errors). `dotnet test EagleBank.sln --no-build` — 1 passed (`HostSmokeTests.Get_health_returns_ok`).

---

## TASK-002 — API foundations: errors, validation, logging

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Shared HTTP error shapes via a **generic injected exception framework**, FluentValidation hook, and PII-safe `ILogger` on **Api, Application, Domain, and Infrastructure**.
- **Requirement references:** REQ-API-004, REQ-API-005, REQ-ERR-007, REQ-ERR-008, REQ-NFR-003, REQ-TEST-001
- **Design / ADR references:** ADR-0006, ADR-0007, ADR-0012, ADR-0013, ADR-0014, ADR-0015; design sections 10, 12, and 13
- **Dependencies:** TASK-001
- **Implementation scope:**
  - `EagleBankException` base + typed subtypes (`NotFound`, `Forbidden`, `Conflict`, `InsufficientFunds`, `BalanceCap`)
  - `IExceptionResponseFactory` (injected) maps exception → status + `ErrorResponse` / `BadRequestErrorResponse`
  - One `IExceptionHandler` registered in DI / the ASP.NET pipeline; **no** per-controller try/catch
  - FluentValidation (Apache-2.0) failures go through the same factory (`400` + `details`). Do not add AutoMapper.
  - Unhandled → `500` safe message; log exception type + `RequestId` only
  - `RequestId` scope (honor `X-Request-Id` or generate); after auth, add `userId` (`usr-…`) to the same scope
  - Api request log: method, route template, status, elapsed — not raw path
  - Application: every command/query handler logs start + result code
  - Domain: invariant failures log reason code only
  - Infrastructure: persist/JWT issue logs entity type and outcome; `EnableSensitiveDataLogging` off
  - Reqnroll `Support/`: `WebApplicationFactory` fixture, isolated SQLite per scenario, Before/After hooks
  - Shared step definitions for auth state, HTTP status, OpenAPI error/success shapes, and log-sink denylist
  - SOLID: exception mapping is OCP (new type + factory, no controller catch); factory is injected (DIP); Application must not reference ASP.NET types to throw HTTP results
- **Test scope:**
  - **Unit (xUnit):** factory/handler — each mapped type → correct status and JSON shape; unknown exception → 500 without stack/secrets
  - **Gherkin:** no business features yet; shared steps must compile. PII sink assertion may wait for TASK-004 create-user/login
- **Verification / definition of done:** New failure modes do not require controller changes. All four layers have logger usage points. Reqnroll host is ready. `dotnet test` passes.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 14 passed, 0 failed (factory mappings, exception handler JSON, layer log helpers, health + RequestId/route-template log).

---

## TASK-003 — Domain Money and EF Core SQLite schema

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Persist users, accounts, and transactions with integer pence and migrations.
- **Requirement references:** REQ-DATA-001, REQ-DATA-002, REQ-NFR-004, REQ-ACCOUNT-013
- **Design / ADR references:** ADR-0002, ADR-0003, ADR-0006; design sections 6, 8, and 13
- **Dependencies:** TASK-001
- **Implementation scope:**
  - `Money` value object (GBP, two decimal places)
  - EF entities / configuration: `users` (unique email, password hash, address columns), `accounts` (`balance_pence`, FK user), `transactions` (immutable, cascade delete)
  - SQLite file path gitignored; apply migrations in Development or documented `dotnet ef` command
  - No application cache (ADR-0010)
  - Infrastructure persist logging: entity type + outcome only; never SQL parameter values (ADR-0007)
- **Test scope:** **Unit (xUnit):** Money scale and invalid amounts; EF can create the schema against an isolated SQLite file. No Gherkin in this task.
- **Verification / definition of done:** `dotnet test` passes. Schema creates without storing amounts as REAL/`double`.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 25 passed, 0 failed. `SqliteSchemaTests` confirmed `accounts.balance_pence` and `transactions.amount_pence` are INTEGER; Money unit tests cover scale/currency/range.

---

## TASK-004 — Create user and login

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Public signup with password and `POST /v1/auth/login` returning `{ "token" }`.
- **Requirement references:** REQ-USER-001–005, REQ-USER-014, REQ-USER-015, REQ-AUTH-001–005, REQ-ERR-001
- **Design / ADR references:** ADR-0005, ADR-0008, ADR-0013, ADR-0014, DQ-001, DQ-002; design sections 7 and 13
- **Dependencies:** TASK-002, TASK-003
- **Implementation scope:**
  - `CreateUserCommand` / handler; `LoginCommand` / handler
  - `POST /v1/users` (201 `UserResponse`, no password); required `password` (min 8); no JWT
  - Duplicate email → `400` with details (not 409)
  - `POST /v1/auth/login` → 200 `{ "token" }`; malformed body → 400; unknown email and bad password share one `401`
  - `PasswordHasher<T>`; JWT `sub` = `userId`, 60-minute expiry
  - Logging: Api + CreateUser/Login handlers (result codes); Infrastructure logs user persist and JWT issued without token/email; do not log password or hash
  - Extend submitted `openapi.yaml` with `password` and login (also TASK-008 if you batch spec edits)
  - SOLID: two handlers (create vs login); controller injects those handlers only; password hash and JWT via Application ports implemented in Infrastructure
  - Licence: hand-written UserResponse / login mapping; no AutoMapper or MediatR
- **Test scope:**
  - **Gherkin `Features/Users.feature`:** create user with all required data → 201 UserResponse (no password); missing required data → 400; invalid phone or email → 400; duplicate email → 400
  - **Gherkin `Features/Auth.feature`:** login with valid credentials → 200 `{ token }`; malformed body → 400; unknown email → 401; wrong password → 401
  - **Then** (Gherkin or xUnit): user response has no password; log sink has no email/password/phone/address/token
- **Verification / definition of done:** `dotnet test` runs those scenarios and they pass.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 44 passed, 0 failed (Users.feature + Auth.feature plus prior unit/schema tests). Reqnroll `/` in unused shared Then steps had to be escaped as `\/` (Cucumber alternatives).

---

## TASK-005 — Fetch user

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Authenticated `GET /v1/users/{userId}` with 401/403/404.
- **Requirement references:** REQ-USER-006–008, REQ-USER-016, REQ-AUTH-003, REQ-AUTH-006–007
- **Design / ADR references:** ADR-0005, ADR-0008, ADR-0013, ADR-0014; ownership helper
- **Dependencies:** TASK-004
- **Implementation scope:**
  - JWT bearer on this route; `GetUserQuery` / handler (do not add Get to the create-user handler)
  - Missing token → 401; other existing user → 403; unknown id → 404; bad `userId` pattern → 400
  - Shared ownership helper (do not copy-paste 403/404)
  - Logging: query start/result (`Forbidden`/`NotFound`); no email or name
  - SOLID: new query type only; inject `GetUserQuery` handler; reuse the ownership helper (ISP/SRP)
- **Test scope:** **Gherkin `Features/Users.feature`:** authenticated owner fetches own user → 200; unauthenticated → 401; other existing user → 403; unknown id → 404; bad `userId` pattern → 400; sink has no email/name
- **Verification / definition of done:** `dotnet test` passes for those scenarios.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 52 passed, 0 failed (owner 200, unauthenticated 401, other user 403, unknown 404, bad `userId` 400, plus prior tests).

---

## TASK-006 — Create and fetch account

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Owner can create an account and fetch it by `accountNumber`.
- **Requirement references:** REQ-ACCOUNT-001–005, REQ-ACCOUNT-007–009, REQ-ACCOUNT-012, REQ-API-008
- **Design / ADR references:** ADR-0002, ADR-0008, ADR-0013, ADR-0014; path is `{accountNumber}` not `{accountId}`
- **Dependencies:** TASK-005
- **Implementation scope:**
  - `CreateAccountCommand`, `GetAccountQuery`
  - `POST /v1/accounts` 201; `name` + `accountType=personal`; owner from JWT; `sortCode` `10-10-10`; `currency` GBP; `balance` 0.00; `accountNumber` `01` + 6 digits
  - POST without/invalid JWT → 401 (spec also lists 403; no brief create-forbidden case — keep the mapper)
  - `GET /v1/accounts/{accountNumber}` 200 / 401 / 403 / 404 / 400 (bad pattern)
  - Clients cannot set owner or balance
  - Logging: command/query result codes; never log full `accountNumber` or account `name`
  - SOLID: create and get are separate handlers; controller actions inject only the one they dispatch
- **Test scope:** **Gherkin `Features/Accounts.feature`:** create with required data → 201 + fetch own by `accountNumber` → 200; missing fields or bad `accountType` → 400; create without JWT → 401; fetch without JWT → 401; fetch another user's account → 403; fetch unknown number → 404; bad `accountNumber` pattern → 400; sink has no account number or name
- **Verification / definition of done:** `dotnet test` passes for those scenarios.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 66 passed, 0 failed (create+fetch 201/200, missing/bad type 400, create/fetch 401, other owner 403, unknown 404, bad `accountNumber` 400, plus prior tests).

---

## TASK-007 — Deposit and fetch transaction

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Deposit updates balance atomically; fetch transaction by id on the owning account.
- **Requirement references:** REQ-TXN-001–005, REQ-TXN-008–009, REQ-TXN-011–015, REQ-TXN-016, REQ-DATA-003–004, Q-003, Q-006
- **Design / ADR references:** ADR-0003, ADR-0004, ADR-0008, ADR-0013, ADR-0014
- **Dependencies:** TASK-006
- **Implementation scope:**
  - `CreateTransactionCommand` (deposit path required); `GetTransactionQuery`
  - `POST .../transactions` 201; amount > 0, two decimals, ≤ 10000.00; currency GBP
  - Deposit increases balance; reject with 422 if resulting balance would exceed 10000.00
  - One DB transaction + conditional UPDATE; no leftover row on failure
  - `GET .../transactions/{transactionId}` 200; 400 bad id/account pattern; 401; 403 if account exists and is not owned; 404 if account missing, txn missing, or txn belongs to another account
  - Transaction ids `tan-` + multiple alphanumeric characters
  - Logging: Application result codes; Domain `BalanceCap` reason code on 422; Infrastructure persist commit/rollback without amounts or `reference`
  - SOLID: create vs get are separate handlers; deposit cap is a domain invariant (not a controller `if`); withdrawal in TASK-009 extends this handler’s type switch + domain, not a new god service
- **Test scope:** **Gherkin `Features/Transactions.feature`:** deposit with required data → 201 and balance increased; invalid amount → 400; unauthenticated → 401; other user's account → 403; missing account → 404; fetch own transaction → 200; fetch 401; fetch bad id/account pattern → 400; fetch other user's account → 403; unknown or wrong-account `transactionId` → 404; deposit that would exceed `10000.00` → 422 and unchanged balance; sink has no amount, `reference`, or full account number
- **Verification / definition of done:** `dotnet test` passes those scenarios. Withdrawal may still be unimplemented.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 90 passed, 0 failed (deposit 201 + balance, invalid amount 400, 401/403/404 create, fetch 200/401/400/403/404 including wrong-account txn, deposit over cap 422 + unchanged 10000.00). Withdrawal domain/SQL path exists but is not Gherkin-covered (TASK-009).

---

## TASK-008 — README and submitted OpenAPI

- **Priority:** MUST
- **Status:** DONE
- **Objective:** Reviewer can run, test, and authenticate; contract includes login.
- **Requirement references:** REQ-DEL-001–004, REQ-API-007, REQ-NFR-002, REQ-TEST-004
- **Design / ADR references:** ADR-0005, ADR-0011, ADR-0013 (README: how to run `dotnet test`; production follow-ups stay documentation-only)
- **Dependencies:** TASK-007
- **Implementation scope:**
  - README: prerequisites (.NET 10), `dotnet test` (xUnit unit + Reqnroll features), `dotnet run`, create user, login, Bearer example
  - State SQLite reset, JWT secret via env/user-secrets, AI assistance allowed
  - Note that dependencies are permissive OSS only (no AutoMapper/MediatR licence keys)
  - Note production: rate limiting, SAST/SCA, PostgreSQL, no cache of balance (do not implement them)
  - Note that logs are structured, correlatable by RequestId, and must not contain PII
  - `openapi.yaml` updated: `password`, `POST /v1/auth/login` `{ "token" }`, transaction id pattern `^tan-[A-Za-z0-9]+$`
- **Test scope:** None beyond existing suite still green
- **Verification / definition of done:** README commands are accurate. Spec matches implemented auth. `dotnet test` still passes.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 103 passed, 0 failed. README documents `dotnet test`, `dotnet run`, create-user/login/Bearer, SQLite reset, JWT via env/user-secrets. `openapi.yaml` already had password, login `{ token }`, and `^tan-[A-Za-z0-9]+$`.

---

## TASK-009 — Withdrawal

- **Priority:** SHOULD
- **Status:** DONE
- **Objective:** Withdrawal with insufficient-funds `422` and unchanged balance.
- **Requirement references:** REQ-TXN-006, REQ-TXN-007, REQ-TEST-002
- **Design / ADR references:** ADR-0004, ADR-0013, ADR-0014
- **Dependencies:** TASK-007
- **Implementation scope:** Same create-transaction handler; `type=withdrawal`; conditional UPDATE `balance_pence >= amount_pence`; Domain logs `InsufficientFunds` reason code only (no amount). SOLID: insufficient-funds lives on the domain account; do not add a `WithdrawalService` or edit GetTransaction.
- **Test scope:** **Gherkin `Features/Transactions.feature`:** withdrawal with sufficient funds → 201 and lower balance; insufficient funds → 422, no new transaction, same balance; sink has no monetary amounts
- **Verification / definition of done:** `dotnet test` includes those scenarios.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 106 passed, 0 failed (withdrawal 201 + balance 7.50; insufficient funds 422 + balance still 10.50 and original deposit still fetchable). Same create-transaction handler; no WithdrawalService.

---

## TASK-010 — List accounts and list transactions

- **Priority:** SHOULD
- **Status:** TODO
- **Objective:** Collection GETs return only the caller's data.
- **Requirement references:** REQ-ACCOUNT-006, REQ-TXN-010
- **Design / ADR references:** ADR-0008, ADR-0013, ADR-0014
- **Dependencies:** TASK-006, TASK-007
- **Implementation scope:**
  - `GET /v1/accounts` → 200 own list only, or 401. Do **not** return 403/404 for this collection (spec has 200/401/500 only).
  - `GET /v1/accounts/{accountNumber}/transactions` → 200 / 400 (bad accountNumber) / 401 / 403 / 404
  - Logging: query result codes only; no account numbers in the list log
  - SOLID: `ListAccountsQuery` and `ListTransactionsQuery` are new handlers; do not add list methods onto create/get handlers
- **Test scope:** **Gherkin:** `Features/Accounts.feature` list accounts → 200 (only caller) and 401; `Features/Transactions.feature` list transactions → 200 / 400 / 401 / 403 / 404; sink has no account numbers
- **Verification / definition of done:** `dotnet test` passes for those scenarios.
- **Verification evidence:** Not run

---

## TASK-011 — PATCH user and account

- **Priority:** SHOULD
- **Status:** TODO
- **Objective:** Partial updates for owner only.
- **Requirement references:** REQ-USER-009, REQ-USER-010, REQ-ACCOUNT-010
- **Design / ADR references:** A-003 (PATCH is partial), ADR-0013, ADR-0014
- **Dependencies:** TASK-005, TASK-006
- **Implementation scope:** `UpdateUserCommand`, `UpdateAccountCommand` as **new** handlers (do not edit Create/Get). Cannot change ids, balance, sortCode, ownership; `updatedTimestamp` changes; 401 if no/invalid JWT; log result codes only (no email, name, or account name). SOLID: OCP — PATCH is additive files.
- **Test scope:** **Gherkin:** `Features/Users.feature` PATCH own → 200; 401; 403 other; 404 missing; 400 bad path or invalid body; 400 duplicate email. `Features/Accounts.feature` PATCH own → 200; 401; 403; 404; 400. Sink has no email/name
- **Verification / definition of done:** `dotnet test` passes for those scenarios.
- **Verification evidence:** Not run

---

## TASK-012 — DELETE user and account

- **Priority:** SHOULD
- **Status:** TODO
- **Objective:** Delete user only with no accounts; delete account and hide its transactions.
- **Requirement references:** REQ-USER-011–013, REQ-ACCOUNT-011, REQ-DATA-005, REQ-DATA-006, REQ-TEST-003, A-010
- **Design / ADR references:** Cascade transactions on account delete; ADR-0013, ADR-0014
- **Dependencies:** TASK-006
- **Implementation scope:** `DeleteUserCommand` and `DeleteAccountCommand` as **new** handlers (do not edit Create/Get/Update). 204 / 409 if user still has accounts / 400 bad id / 401 / 403 / 404; account delete 204 (even with balance or transactions) / 400 / 401 / 403 / 404 — **no account-delete 409**; log `Conflict`/`NotFound` result codes only. SOLID: OCP — DELETE is additive files.
- **Test scope:** **Gherkin:** `Features/Users.feature` DELETE own with no accounts → 204; own with accounts → 409; 401; 403; 404. `Features/Accounts.feature` DELETE own → 204 then GET 404; 401; 403; 404; optional 400 bad `accountNumber`. Sink has no email or account number
- **Verification / definition of done:** `dotnet test` passes for those scenarios.
- **Verification evidence:** Not run

---

## TASK-013 — Concurrent withdrawal safety test

- **Priority:** COULD
- **Status:** DONE
- **Objective:** Prove two parallel withdrawals cannot overdraw.
- **Requirement references:** REQ-TXN-016
- **Design / ADR references:** ADR-0004, ADR-0013
- **Dependencies:** TASK-009
- **Implementation scope:** No product change unless the test reveals a bug
- **Test scope:** **Unit/integration xUnit (not Gherkin):** two concurrent withdrawals of the full balance; one succeeds, one 422; final balance ≥ 0 and at most one debit of that amount
- **Verification / definition of done:** That test is in the suite and `dotnet test` passes.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 112 passed, 0 failed. `ConcurrentWithdrawalTests` two parallel full-balance withdrawals → one `201`, one `422`, remaining balance `0.00`.

---

## TASK-014 — Docker Compose

- **Priority:** COULD
- **Status:** TODO
- **Objective:** One-command run for reviewers with Docker.
- **Requirement references:** REQ-NFR-002 (stretch design item 6)
- **Design / ADR references:** Design section 15
- **Dependencies:** TASK-008
- **Implementation scope:** Dockerfile + compose; JWT secret required via env; no hidden default secret
- **Test scope:** Document how to verify; do not claim Docker works unless the image was built and run
- **Verification / definition of done:** Image builds and serves create-user or `/health` if that endpoint exists.
- **Verification evidence:** Not run

---

## TASK-015 — Swagger UI for submitted spec

- **Priority:** COULD
- **Status:** DONE
- **Objective:** Serve the submitted `openapi.yaml`, not a drifting generated-only spec.
- **Requirement references:** REQ-API-007 (stretch design item 7)
- **Design / ADR references:** Design section 14
- **Dependencies:** TASK-008
- **Implementation scope:** Swagger UI / static file for repo `openapi.yaml`
- **Test scope:** Optional HTTP assertion that the spec document is reachable
- **Verification / definition of done:** Documented URL shows the submitted contract including login.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 110 passed, 0 failed. `GET /openapi.yaml` returns the repo file (includes `/v1/auth/login`). `GET /swagger/index.html` returns Swagger UI. README documents `http://localhost:5080/swagger`.

---

## TASK-016 — Final quality pass

- **Priority:** MUST
- **Status:** DONE
- **Objective:** MVP suite is green; no secrets in git; logs stay PII-free; structure still matches SOLID.
- **Requirement references:** REQ-TEST-001, REQ-TEST-004, REQ-TEST-005, REQ-DEL-003, REQ-NFR-003
- **Design / ADR references:** ADR-0006, ADR-0007, ADR-0013, ADR-0014, ADR-0015
- **Dependencies:** TASK-008
- **Implementation scope:** Fix gaps found by running the full test suite; confirm `.gitignore` and no committed JWT secret; confirm Api/Application/Domain/Infrastructure all log and that denylist strings do not appear in test sinks; SOLID review — no fat `*Service` with multiple use cases, no business rules in controllers, no Application/Domain reference to ASP.NET or EF, each implemented use case is its own handler, stretch did not edit MVP handlers except TASK-009’s allowed create-transaction extension; **licence review** — no AutoMapper, MediatR, or other RPL/commercial/key packages in any `.csproj`; no licence key files committed
- **Test scope:** Re-run the whole suite: xUnit unit tests, Reqnroll features for the implemented slice, and PII log-sink checks; no new product features
- **Verification / definition of done:** `dotnet test` executed successfully in this environment. Do not claim pass unless that run succeeded.
- **Verification evidence:** 2026-09-05 — `dotnet test EagleBank.sln` — 108 passed, 0 failed. Removed committed Development JWT placeholder; startup fails if the key is under 32 characters. `.gitignore` covers `data/`, `*.db`, `secrets.json`, `*.lic`. No AutoMapper/MediatR/`*.lic` in the tree. SOLID: one handler per use case; Application/Domain have no ASP.NET or EF references; controllers dispatch only. All four layers have log helpers; Gherkin PII denylist checks remain green.

---

## Out of scope (no tasks)

| Topic | Why |
|---|---|
| Redis / HTTP cache | ADR-0010 |
| Message broker / EDA | ADR-0009 |
| In-app rate limiting | ADR-0011 — production only |
| SAST/SCA pipeline jobs | ADR-0011 — production only |
| MediatR, AutoMapper, second database, event sourcing | ADR-0008, ADR-0015 |

---

## Suggested implementation order

1. TASK-001 … TASK-008 (MVP, then stop if timeboxed)
2. TASK-009 (withdrawal)
3. TASK-010 (lists)
4. TASK-011 (PATCH)
5. TASK-012 (DELETE)
6. TASK-013, TASK-014, TASK-015 if time remains
7. TASK-016 after the chosen slice
