# Barclays Code Review

## Review metadata

- Date: 2026-09-05
- Reviewer: Cursor Agent (`barclays-code-review`)
- Commit / branch reviewed: `634993179676fb55d0a70aeae07f356e0d5ad702` on `feature/eagle-bank-user`
- Review outcome: PASS_WITH_OBSERVATIONS

## Scope reviewed

- Workflow state, approved requirements, system design, task list (TASK-001–016), ADR index and `docs/adr/*.md`
- Submitted `openapi.yaml` (including login and password extensions)
- API composition root, controllers, JWT, exception pipeline, request middleware, Swagger hosting
- Application handlers, validators, ownership helper, error mapping
- Domain `User`, `BankAccount`, `Transaction`, `Money`
- Infrastructure EF/SQLite stores, integer-pence schema, password hasher, JWT issuer
- Reqnroll features (`Users`, `Auth`, `Accounts`, `Transactions`) and xUnit units
- README, `appsettings*.json`, `.gitignore`, `lessons.md`, `agent-interactions.md`

This is a review of the completed implement-and-test slice (MUST + TASK-009 withdrawal + TASK-015 Swagger + TASK-016 quality pass). Stretch TASK-010–014 remain TODO and were assessed as documented omissions, not silent failures.

## Commands executed

```bash
dotnet test EagleBank.sln --nologo
git rev-parse --abbrev-ref HEAD
git log -1 --format='%H%n%s'
```

Result: **110 passed, 0 failed, 0 skipped.** Build succeeded as part of the test run. No production code was changed during this review.

## Executive summary

The MVP is implemented against the approved requirements and OpenAPI contract: public create-user, login JWT, authenticated fetch of own user/account/transaction, create account, deposit and withdrawal with `422` for balance cap and insufficient funds. Ownership is enforced server-side (`403` vs `404`). Money is persisted as integer pence. Transaction create and balance update share a database transaction with a conditional `UPDATE`. Passwords are hashed; committed JWT keys are empty. The README is honest about stretch work.

The suite is green. Remaining gaps are stretch endpoints (list/PATCH/DELETE), a missing concurrent-withdrawal test, and a few observability/documentation nits. None of these block submission of the approved MVP slice.

## Findings

### CRITICAL

None.

### HIGH

None.

### MEDIUM

None.

### LOW

#### [LOW] Request log scope never includes `userId`

**Evidence**

- Design §12 and ADR-0007: `BeginScope` should include `RequestId` and, after authentication, `userId` (`usr-…`).
- `Program.cs` registers `RequestContextMiddleware` **before** `UseAuthentication`.
- `RequestContextMiddleware` reads `context.User` (`sub` / `userId`) at the start of the request, when the principal is still empty.

**Issue**

Authenticated request logs get `RequestId` in scope, but not `userId`. Application handlers still emit `userId` via `UseCaseLog.Started(..., userId)`, so use-case traces are not blind.

**Impact**

HTTP completion logs cannot be filtered by `userId` from the request scope. This is an undocumented deviation from ADR-0007, not an authorisation bypass.

**Recommendation**

Move `RequestContextMiddleware` to after `UseAuthentication` (or set scope in a later middleware once the principal is populated), then add a host test that an authenticated call puts `usr-` in the request scope.

#### [LOW] First README run command omits the JWT key

**Evidence**

`README.md` “Run the API” leads with `dotnet run --project src/EagleBank.Api`. The next subsection correctly states that startup throws unless `Jwt:SigningKey` is at least 32 characters, and shows `Jwt__SigningKey` / user-secrets.

**Issue**

A reviewer who copies only the first command hits `InvalidOperationException: JWT signing key is not configured.`

**Impact**

Wasted first-run time; the API is not broken once the documented key is set.

**Recommendation**

Put the export / user-secrets command in the same snippet as `dotnet run`.

#### [LOW] Validation `details.field` casing is inconsistent

**Evidence**

- FluentValidation on DTOs uses property names such as `Name`, `Email`, `Amount` (`CreateUserRequestValidator`, `CreateTransactionRequestValidator`).
- Duplicate-email failures hard-code `"email"` (`CreateUserHandler`, `UserStore`).
- `ExceptionResponseFactory` copies `error.PropertyName` into `ErrorDetail.Field`.
- OpenAPI `BadRequestErrorResponse.details[].field` is an unconstrained string; tests assert the `400` shape, not camelCase field names.

**Issue**

Clients that bind error fields to JSON names may see `Email` on missing-value failures and `email` on duplicate-email.

**Impact**

Cosmetic contract inconsistency. Status codes and `details` presence still match REQ-API-004.

**Recommendation**

Use a camelCase property-name resolver (or explicit `OverridePropertyName`) so all `field` values match the JSON names.

### OBSERVATIONS

#### [OBSERVATION] Stretch OpenAPI operations are documented but not implemented

`GET /v1/accounts`, `GET .../transactions` (list), `PATCH` user/account, and `DELETE` user/account remain TASK-010–012 TODO. Controllers have no those actions, so those verbs/paths return `404`/`405`. README “What is implemented” states this. TASK-015 correctly serves the **submitted** spec, which still describes the stretch operations. A reviewer clicking those operations in Swagger will not get the documented success codes.

#### [OBSERVATION] REQ-TXN-016 is implemented, not proven by a concurrency test

`TransactionStore.AddAtomicAsync` uses a DB transaction plus conditional `UPDATE` (`balance_pence >= amount` / `+ amount <= MaxPence`), matching ADR-0004. TASK-013 (xUnit two-at-once withdrawal) is still TODO. Feature tests cover sequential insufficient-funds `422` and unchanged balance, not parallel requests.

#### [OBSERVATION] Insufficient-funds “no new transaction” is inferred, not listed

`Transactions.feature` failed withdrawal then fetches “the created transaction” (the earlier deposit) and checks balance `10.50`. There is no list endpoint to assert the withdrawal row was never inserted. Combined with the atomic store, this is a reasonable proxy.

#### [OBSERVATION] Login short-circuits password verify when the email is unknown

`LoginHandler` does `user is null || !_passwordHasher.Verify(...)`. Unknown email and wrong password both return `401` / `UnauthorizedException` (“Invalid credentials.”), which matches the spec and Auth.feature. Hash verify is skipped for unknown email, so timing may differ slightly. Acceptable for this take-home.

#### [OBSERVATION] Test JWT signing key is a labelled fixture constant

`ApiWebApplicationFactory.TestJwtSigningKey` is committed and clearly `TEST-ONLY-…`. Production `appsettings.json` / `appsettings.Development.json` leave `SigningKey` empty. This matches LESSON-011 and REQ-NFR-005.

#### [OBSERVATION] Extra `/health` is not in OpenAPI

`GET /health` returns `{ "status": "ok" }` and is covered by `HostSmokeTests`. Fine as an operational extra; not a contract operation.

#### [OBSERVATION] Unused stretch types and leftover step text

`ConflictException` is mapped (`409`) but unused until DELETE user. `HttpSteps` still has an unused Then for a combined payload type. Neither affects runtime behaviour.

## Requirements traceability

| Requirement / capability | Evidence | Status | Notes |
|---|---|---|---|
| REQ-USER-001–005 create user + password omitted from response | `UsersController.CreateUser`, `CreateUserHandler`, `UserResponse`, `Users.feature` | PASS | Public `201`; password write-only; id `usr-…` |
| REQ-USER-006–008, 016 fetch user 200/403/404/400 | `GetUserHandler`, `ResourceOwnership`, `Users.feature` | PASS | Bad `userId` → `400` |
| REQ-USER-009–013 PATCH/DELETE user | No controller actions; TASK-011/012 TODO | PARTIAL | Stretch; README honest |
| REQ-USER-014–015 unique email, E.164, email format | Validators + unique index + `400` duplicate | PASS | Duplicate is `400` per Q-004 |
| REQ-AUTH-001–005 login + JWT `sub` | `AuthController`, `LoginHandler`, `JwtTokenIssuer`, `Auth.feature` | PASS | `{ "token" }`; 60 min; key not committed |
| REQ-AUTH-003, 006–007 bearer + ownership | `[Authorize]`, JWT challenge `401`, `ResourceOwnership` | PASS | Other owner `403`; missing `404` |
| REQ-ACCOUNT-001–005, 007–009, 012–013 create/fetch | `AccountsController`, `Accounts.feature` | PASS | `01` + 6 digits; sort `10-10-10`; GBP; `0.00` |
| REQ-ACCOUNT-006, 010–011 list/PATCH/DELETE | TASK-010–012 TODO | PARTIAL | Stretch |
| REQ-TXN-001–005, 008–009 deposit | `CreateTransactionHandler`, `Transactions.feature` | PASS | `201`; other account `403`; missing `404` |
| REQ-TXN-006–007 withdrawal + `422` | Same handler; feature scenarios | PASS | TASK-009 DONE |
| REQ-TXN-011–013 fetch txn | `GetTransactionHandler`, feature | PASS | Wrong-account txn → `404` |
| REQ-TXN-010 list txns | No list action | PARTIAL | Stretch |
| REQ-TXN-014 immutability | No update/delete txn APIs | PASS | |
| REQ-TXN-015 response fields | `TransactionResponseMapper` + feature `TransactionResponse` | PASS | |
| REQ-TXN-016 atomic + no overdraft | `TransactionStore` conditional UPDATE | PARTIAL | Mechanism present; TASK-013 test missing |
| REQ-API-001–008 contract + login in spec | Controllers + `openapi.yaml` + HostSmokeTests | PARTIAL | Implemented ops match; stretch ops in spec only |
| REQ-DATA-001–004 persistence + failed writes | SQLite + atomic store + unique email | PASS | |
| REQ-DATA-005–006 delete side effects | Not implemented | PARTIAL | Stretch |
| REQ-ERR-001–008 error mapping | `ExceptionResponseFactory`, JWT `OnChallenge`, handler | PASS | `500` uses fixed message; no stack leak |
| REQ-NFR-001–005 .NET 10, runbook, structure, money, secrets | csproj `net10.0`, README, layers, `Money`, empty key | PASS | First-run JWT order is LOW above |
| REQ-TEST-001–002, 004–005 | Features + units; 110 passed | PASS | REQ-TEST-003 N/A (no delete) |
| REQ-DEL-002–004 README, spec, AI disclosure | README + `openapi.yaml` | PASS | REQ-DEL-001 public GitHub not verified in this review |
| TASK-015 Swagger | `/swagger`, `/openapi.yaml` | PASS | Serves submitted file, not generated JSON |

## Architecture / ADR alignment

| ADR | Alignment |
|---|---|
| ADR-0001 Clean architecture / thin controllers | Controllers map HTTP only; handlers own use cases |
| ADR-0002 EF Core + SQLite | `EagleBankDbContext`, migrate in Development, tests use isolated files |
| ADR-0003 Integer pence | `Money` + `SqliteSchemaTests` (`balance_pence` / `amount_pence` INTEGER) |
| ADR-0004 Conditional atomic UPDATE | `TransactionStore.TryDepositAsync` / `TryWithdrawAsync` |
| ADR-0005 JWT + `PasswordHasher<T>` | `AspNetPasswordHasher`, HMAC-SHA256, no ASP.NET Identity user store |
| ADR-0006 xUnit + WAF + isolated SQLite | `ApiWebApplicationFactory` |
| ADR-0007 Structured logs, no PII | Route templates, denylist feature steps, `EnableSensitiveDataLogging` off; **scope `userId` miss** (LOW) |
| ADR-0008 Lightweight CQRS | One handler per use case; no MediatR |
| ADR-0009 No EDA | No events/bus |
| ADR-0010 No app cache | Live row is source of truth |
| ADR-0011 Rate limit / SAST production-only | Documented in README, not implemented |
| ADR-0012 Exception framework | `IExceptionHandler` + `IExceptionResponseFactory` |
| ADR-0013 Reqnroll for HTTP | Four feature files |
| ADR-0014 SOLID | No fat `*Service`; Application has no ASP.NET/EF package refs |
| ADR-0015 Permissive packages | FluentValidation, Swashbuckle, EF, JWT; `Microsoft.OpenApi` pinned 2.7.5 |

`CreateTransactionHandler` applies domain invariants on an `AsNoTracking` account, then persists via SQL. The in-memory `Apply` does not write balance; the conditional `UPDATE` does. Zero-row UPDATE → `422` and rollback. That matches ADR-0004.

## Test assessment

**Suites**

- Reqnroll: `Users.feature`, `Auth.feature`, `Accounts.feature`, `Transactions.feature`
- xUnit: `Money`, `BankAccount.Apply`, `ResourceOwnership`, `ExceptionResponseFactory`, exception handler, SQLite schema, layer logging, account-number factory, host smoke (health, request-id, OpenAPI, Swagger)

**Command actually run:** `dotnet test EagleBank.sln --nologo` — **110 passed, 0 failed.**

**Coverage that is present:** create/fetch happy paths; `400` validation; `401` missing token; `403`/`404` ownership; deposit scale and cap; withdrawal success and insufficient funds; PII denylist on create/login/deposit/withdraw; login unknown email vs wrong password share `401`.

**Gaps:** no TASK-013 concurrency test; list/PATCH/DELETE untested (unimplemented); several `401`/`403`/`404` feature steps assert status only, not `ErrorResponse` shape (mapping is unit-tested); no expired-JWT scenario; “malformed” login body is missing `password`, not invalid JSON.

## Submission readiness

The repository is ready to submit for the **approved MVP plus withdrawal and Swagger**. Do not treat remaining SHOULD/COULD tasks as incomplete MUST work: TASK-010–014 are still TODO and the README says so.

Optional before submit (not required for this review outcome):

1. Reorder request-context middleware so authenticated `userId` appears in log scope.
2. Fold the JWT env export into the first README run snippet.
3. Add TASK-013 if you want evidence for concurrent withdrawals.
4. Tell reviewers in README/Swagger which OpenAPI operations are actually wired.

These four items were implemented after the review, on user request (2026-09-05). The original review outcome is unchanged. Verification after the follow-up: `dotnet test EagleBank.sln` — 112 passed, 0 failed.

No CRITICAL or HIGH defects were found. Implementation approval states were not changed.

## Final outcome

**PASS_WITH_OBSERVATIONS**
