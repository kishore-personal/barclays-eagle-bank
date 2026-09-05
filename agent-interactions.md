# Agent Interaction Audit Trail

This is a concise, factual engineering audit trail for meaningful AI-assisted actions and user decisions.

Because the repository may be public, do **not** store secrets, credentials, personal data, or an unnecessary verbatim transcript of chat.

## Entry template

### YYYY-MM-DD HH:MM — <skill or activity>

- **User intent:** <sanitised summary>
- **Agent actions actually performed:**
  - <action>
- **Decisions / outcomes:**
  - <decision or none>
- **Files changed:**
  - `<path>`
- **Verification actually run:** <commands/results or "None">
- **Workflow transition:** <from -> to>
- **Approval state:** <not required / awaiting / explicitly approved>

---

### 2026-09-05 11:55 — barclays-init

- **User intent:** Initialise the gated Barclays take-home workflow (audit trail, lessons, ADR index, and stage state) without implementing application features.
- **Agent actions actually performed:**
  - Confirmed Git was already initialised on branch `feature/eagle-bank-user`.
  - Created missing workflow directories: `.barclays/`, `requirements/`, `design/`, `tasks/`, `docs/adr/`.
  - Created workflow documents from `.cursor/templates`: `.barclays/workflow-state.md`, `agent-interactions.md`, `lessons.md`, `adrs.md`.
  - Created `.gitignore` (none existed) with local/generated/secret patterns only.
  - Did not create application architecture, controllers, entities, database code, tests, or feature implementation.
- **Decisions / outcomes:**
  - Init marked `COMPLETE`; current stage set to `plan`.
  - Remaining stages left `NOT_STARTED`; no plan/design/tasks approvals recorded.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
  - `lessons.md`
  - `adrs.md`
  - `.gitignore`
  - `requirements/.gitkeep`
  - `design/.gitkeep`
  - `tasks/.gitkeep`
  - `docs/adr/.gitkeep`
- **Verification actually run:** Inspected repository root, Git status, and absence of `.barclays/` and `.gitignore` before creating files.
- **Workflow transition:** uninitialised -> init `COMPLETE`; current stage `plan`
- **Approval state:** not required for init

---

### 2026-09-05 12:01 — barclays-plan

- **User intent:** Produce technical requirements for the Eagle Bank REST API from the exercise scenarios and the attached OpenAPI specification.
- **Agent actions actually performed:**
  - Confirmed init was `COMPLETE` and plan was `NOT_STARTED`.
  - Read the OpenAPI file attached in chat and copied it to `openapi.yaml`.
  - Compared scenario prose with the contract, recorded contradictions (accountId vs accountNumber; create-user Given clause; transaction id regex vs example; missing login path).
  - Wrote `requirements/technical-requirements.md` with stable requirement IDs, MVP vs stretch, assumptions, and open questions.
  - Recorded LESSON-001 in `lessons.md`.
  - Did not generate production implementation code, choose a framework, or start design.
- **Decisions / outcomes:**
  - OpenAPI treated as authoritative for API shape; scenarios treated as authoritative for ownership and error outcomes.
  - Proposed (pending user approval): email/password signup, `POST /v1/auth/login`, and treating `^tan-[A-Za-z0-9]$` as a spec defect.
  - Plan set to `AWAITING_APPROVAL`; design not started.
- **Files changed:**
  - `openapi.yaml`
  - `requirements/technical-requirements.md`
  - `.barclays/workflow-state.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** Read attached OpenAPI and repository workflow state. No tests or builds.
- **Workflow transition:** plan `NOT_STARTED` -> plan `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the technical requirements

---

### 2026-09-05 12:10 — barclays-plan (Q-007)

- **User intent:** Specify preferred implementation language as .NET 10.
- **Agent actions actually performed:**
  - Updated `REQ-NFR-001`, `A-008`, and `Q-007` in `requirements/technical-requirements.md`.
  - Recorded the answer in workflow notes.
  - Did not treat this message as plan approval.
  - Did not start design or implementation.
- **Decisions / outcomes:**
  - Language constraint set to .NET 10.
  - Q-001–Q-006 remain proposed defaults pending plan approval.
  - Plan remains `AWAITING_APPROVAL`.
- **Files changed:**
  - `requirements/technical-requirements.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; plan still `AWAITING_APPROVAL`
- **Approval state:** Q-007 answered; full plan not yet explicitly approved

---

### 2026-09-05 12:13 — barclays-plan (open questions vs design gate)

- **User intent:** Ask whether pending open questions must be cleared with a BA/PO before design can start.
- **Agent actions actually performed:**
  - Classified Q-001–Q-006 as non-blocking if the documented defaults are accepted.
  - Confirmed design remains gated on explicit plan approval, not on external BA answers.
  - Did not mark the plan approved and did not start design.
- **Decisions / outcomes:**
  - Advice: treat the brief and OpenAPI as the product owner for this take-home; proceed on documented assumptions.
  - Q-001 and Q-005 are the only items worth a later BA conversation; they still have workable defaults.
- **Files changed:**
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; plan still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the technical requirements

---

### 2026-09-05 12:14 — barclays-plan (explicit approval)

- **User intent:** Explicitly approve the technical requirements.
- **Agent actions actually performed:**
  - Marked plan `APPROVED` and set current stage to `design`.
  - Recorded Q-001–Q-006 as accepted defaults and Q-007 as .NET 10.
  - Did not start design or implementation.
- **Decisions / outcomes:**
  - `requirements/technical-requirements.md` is the approved plan, including documented defaults.
  - `/barclays-design` is unlocked.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `requirements/technical-requirements.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** plan `AWAITING_APPROVAL` -> plan `APPROVED`; current stage `design`
- **Approval state:** plan explicitly approved by the user

---

### 2026-09-05 12:15 — barclays-design

- **User intent:** Produce a technical design from the approved requirements for a .NET 10 Eagle Bank API.
- **Agent actions actually performed:**
  - Confirmed plan was `APPROVED` and set design `IN_PROGRESS`.
  - Wrote `design/system-design.md` covering architecture, domain, persistence, concurrency, auth, tests, and OpenAPI compliance.
  - Created ADR-0001–ADR-0006 as Proposed (not marked Accepted).
  - Updated `adrs.md` and recorded LESSON-002.
  - Did not implement production endpoints or create the task list.
- **Decisions / outcomes:**
  - Recommended (pending user approval): controllers + clean architecture; EF Core/SQLite; integer pence; conditional balance UPDATE; JWT + PasswordHasher; xUnit + WebApplicationFactory.
  - Design set to `AWAITING_APPROVAL`.
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0001-clean-architecture-controllers.md`
  - `docs/adr/ADR-0002-ef-core-sqlite.md`
  - `docs/adr/ADR-0003-money-integer-pence.md`
  - `docs/adr/ADR-0004-balance-consistency.md`
  - `docs/adr/ADR-0005-jwt-password-hasher.md`
  - `docs/adr/ADR-0006-testing-sqlite.md`
  - `adrs.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** Read approved requirements, OpenAPI location, workflow state, and templates. No tests or builds.
- **Workflow transition:** design `NOT_STARTED` -> design `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the design; ADRs remain Proposed

---

### 2026-09-05 12:21 — barclays-design (logging amendment)

- **User intent:** Add logging at all layers for traceability and ensure no PII is logged.
- **Agent actions actually performed:**
  - Expanded observability, component responsibilities, security, testing, and risks in `design/system-design.md`.
  - Added ADR-0007 (Proposed) and LESSON-003.
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Structured `ILogger` on Api, Application, Domain, and Infrastructure.
  - Allow-list: `RequestId`, opaque `userId`/`transactionId`, route templates, result codes.
  - Denylist includes email, name, phone, address, passwords, tokens, bodies, full account numbers, and transaction references.
  - EF sensitive-data logging stays off.
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0007-structured-logging-no-pii.md`
  - `adrs.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the amended design

---

### 2026-09-05 12:25 — barclays-design (CQRS amendment)

- **User intent:** Ask whether CQRS is better for basic CRUD to reduce regression and follow SOLID.
- **Agent actions actually performed:**
  - Adopted lightweight CQRS (one command/query handler per use case, one store) as ADR-0008.
  - Clarified that full CQRS (separate read database / events / MediatR) remains out of scope.
  - Updated ADR-0001 notes and `design/system-design.md`.
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Controllers dispatch a single handler; stretch work is new files, not edits to MVP handlers.
  - Dispatcher is explicit handler injection, not MediatR (DQ-006).
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0001-clean-architecture-controllers.md`
  - `docs/adr/ADR-0008-lightweight-cqrs.md`
  - `adrs.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the amended design

---

### 2026-09-05 12:27 — barclays-design (EDA / performance)

- **User intent:** Ask whether event-driven architecture is needed for performance.
- **Agent actions actually performed:**
  - Recorded ADR-0009: do not implement a broker, outbox, or async write path.
  - Updated design non-goals, traceability, risks, and DQ-007.
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Recommended no EDA: the contract is synchronous and balances must commit in the same request (ADR-0004).
  - Performance stays in-process (indexes, `AsNoTracking` queries, optional later PostgreSQL).
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0009-no-eda.md`
  - `adrs.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the amended design

---

### 2026-09-05 12:28 — barclays-design (caching / production)

- **User intent:** Ask whether productionising the app requires caching.
- **Agent actions actually performed:**
  - Recorded ADR-0010: no Redis/memory/HTTP cache in the submission.
  - Documented a production follow-up: optional user-profile cache only; never cache balance as source of truth.
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Caching is not required for the take-home.
  - In production, cache is a later read optimisation after indexes/PostgreSQL, not a ledger store.
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0010-no-cache-for-submission.md`
  - `adrs.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the amended design

---

### 2026-09-05 12:32 — barclays-design (rate limiting and SAST)

- **User intent:** Require production rate limiting and SAST through most of the SDLC.
- **Agent actions actually performed:**
  - Added ADR-0011 and updated security, testing, productionisation, and risks in `design/system-design.md`.
  - Did not add rate-limit or SAST implementation to the take-home scope.
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Production: edge rate limits, strictest on login and create-user; `429` + `Retry-After`.
  - SAST in IDE/PR/CI/scheduled scans and a release gate; complement with SCA and secrets scanning.
- **Files changed:**
  - `design/system-design.md`
  - `docs/adr/ADR-0011-production-rate-limiting-and-sast.md`
  - `adrs.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the amended design

---

### 2026-09-05 12:42 — barclays-design (architecture diagram)

- **User intent:** Produce an architecture diagram to review before design approval.
- **Agent actions actually performed:**
  - Added a Mermaid flowchart to `design/system-design.md` section 4.
  - Created a review canvas for the same runtime picture (submission vs production follow-ups).
  - Did not mark the design approved and did not start tasks.
- **Decisions / outcomes:**
  - Diagram matches the proposed design: one process, CQRS-lite, one SQLite store.
- **Files changed:**
  - `design/system-design.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; design still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the design

---

### 2026-09-05 12:51 — barclays-design (explicit approval)

- **User intent:** Explicitly approve the system design.
- **Agent actions actually performed:**
  - Marked design `APPROVED` and set current stage to `tasks`.
  - Set ADR-0001–ADR-0011 to Accepted.
  - Did not create the task list or start implementation.
- **Decisions / outcomes:**
  - `design/system-design.md` and accepted ADRs are the approved design.
  - `/barclays-tasks` is unlocked.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `design/system-design.md`
  - `adrs.md`
  - `docs/adr/ADR-0001` through `ADR-0011`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** design `AWAITING_APPROVAL` -> design `APPROVED`; current stage `tasks`
- **Approval state:** design explicitly approved by the user

---

### 2026-09-05 12:54 — barclays-tasks

- **User intent:** Decompose the approved design into ordered implementation tasks.
- **Agent actions actually performed:**
  - Confirmed init complete and plan/design approved.
  - Wrote `tasks/tasks.md` with TASK-001–TASK-016 (no implementation).
  - Marked TASK-001–TASK-008 and TASK-016 as MUST (MVP); TASK-009–TASK-012 SHOULD; TASK-013–TASK-015 COULD.
- **Decisions / outcomes:**
  - Redis, EDA, in-app rate limiting, and SAST jobs have no tasks (approved out of take-home scope).
  - Tasks set to `AWAITING_APPROVAL`.
- **Files changed:**
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** tasks `NOT_STARTED` -> tasks `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:12 — barclays-tasks (HTTP contract check)

- **User intent:** Check task list against HTTP verbs and status codes.
- **Agent actions actually performed:**
  - Compared `openapi.yaml` and approved requirements with `tasks/tasks.md`.
  - Added a verb/status matrix and tightened TASK-004, TASK-006, TASK-007, TASK-010, TASK-011, TASK-012.
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - Verbs were already correct (no PUT; no transaction PATCH/DELETE).
  - Fixes: list accounts is 200/401 only; delete account has no 409; login 400; missing 401/400 on PATCH/DELETE/fetch txn called out in test scope.
- **Files changed:**
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** Spec/requirements/task comparison only. No tests or builds.
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:16 — barclays-tasks (all-layer logging)

- **User intent:** Check whether tasks require no-PII logging at every layer.
- **Agent actions actually performed:**
  - Confirmed the first draft only covered logging in TASK-002/004/016.
  - Added a cross-cutting allow/deny table and per-task logging/test bullets for TASK-002–TASK-012 and TASK-016.
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - Feature tasks must use Api, Application, Domain, and Infrastructure logs with the ADR-0007 denylist.
- **Files changed:**
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:18 — barclays-tasks (exception framework)

- **User intent:** Require a generic, injected, maintainable exception-handling framework.
- **Agent actions actually performed:**
  - Added ADR-0012 and updated design section 10 and component responsibilities.
  - Expanded TASK-002 and added a cross-cutting exception-handling rule to `tasks/tasks.md`.
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - One `IExceptionHandler` + injected `IExceptionResponseFactory`; handlers throw typed `EagleBankException` subtypes and do not set HTTP status.
- **Files changed:**
  - `docs/adr/ADR-0012-injected-exception-framework.md`
  - `design/system-design.md`
  - `adrs.md`
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:25 — barclays-tasks (licence-safe dependencies)

- **User intent:** Avoid licence implications on this public POC (examples: AutoMapper, MediatR).
- **Agent actions actually performed:**
  - Recorded ADR-0015: permissive MIT/Apache/BSD/Microsoft packages only; hand-written mapping; no AutoMapper or MediatR at any version.
  - Updated tasks (allow/deny list, TASK-001/002/004/008/016) and ADR-0008 (licence as extra reason to skip MediatR).
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - Do not pin last-MIT AutoMapper 14 / MediatR 12 as a workaround.
  - FluentValidation and Reqnroll remain allowed (permissive).
- **Files changed:**
  - `docs/adr/ADR-0015-permissive-dependencies.md`
  - `docs/adr/ADR-0008-lightweight-cqrs.md`
  - `design/system-design.md`
  - `adrs.md`
  - `tasks/tasks.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:23 — barclays-tasks (SOLID maintainability)

- **User intent:** Ensure SOLID is used so the application stays maintainable; update tasks if that was missing.
- **Agent actions actually performed:**
  - Confirmed design already implied SOLID (handlers, DI, exception OCP) but tasks had no explicit rule.
  - Added ADR-0014 and a cross-cutting SOLID table on `tasks/tasks.md`.
  - Bound TASK-001 project references (DIP), feature tasks (one handler, no fat services), and TASK-016 review.
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - SOLID is an implementation constraint: S/O/L/I/D mapped to concrete do/don't; create-transaction remains one use case for deposit and withdrawal.
- **Files changed:**
  - `docs/adr/ADR-0014-solid.md`
  - `design/system-design.md`
  - `adrs.md`
  - `tasks/tasks.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:21 — barclays-tasks (Gherkin acceptance tests)

- **User intent:** Confirm unit and integration tests exist, and require Gherkin-style coverage from the brief scenarios.
- **Agent actions actually performed:**
  - Recorded ADR-0013 (Reqnroll + `.feature` files on the existing WebApplicationFactory/SQLite host).
  - Extended ADR-0006 and design section 13: xUnit for unit/concurrency; Reqnroll for HTTP acceptance.
  - Updated `tasks/tasks.md` with a feature-file catalogue, shared steps, and per-task Gherkin scopes.
  - Did not implement code or mark tasks approved.
- **Decisions / outcomes:**
  - HTTP Given/When/Then from the requirements become `Features/Users.feature`, `Auth.feature`, `Accounts.feature`, `Transactions.feature`.
  - Concurrent withdrawal stays xUnit.
- **Files changed:**
  - `docs/adr/ADR-0013-reqnroll-gherkin.md`
  - `docs/adr/ADR-0006-testing-sqlite.md`
  - `design/system-design.md`
  - `adrs.md`
  - `tasks/tasks.md`
  - `lessons.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; tasks still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the task list

---

### 2026-09-05 13:27 — barclays-tasks (user approval)

- **User intent:** Explicitly approve the implementation task list.
- **Agent actions actually performed:**
  - Marked `tasks` as `APPROVED` in `.barclays/workflow-state.md`.
  - Set current stage to `implement-and-test`.
  - Did not implement production code.
- **Decisions / outcomes:**
  - `tasks/tasks.md` is the approved work breakdown (TASK-001–TASK-016).
  - `/barclays-implement-and-test` is unlocked. Implementation must follow approved tasks only, one coherent task at a time.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** tasks `AWAITING_APPROVAL` → `APPROVED`; current stage `tasks` → `implement-and-test`
- **Approval state:** tasks explicitly approved by the user

---

### 2026-09-05 13:30 — barclays-implement-and-test (TASK-001)

- **User intent:** Implement and test one approved task at a time, starting with the first MUST item.
- **Agent actions actually performed:**
  - Confirmed plan/design/tasks approvals and set implement-and-test to `IN_PROGRESS`.
  - Created `EagleBank.sln` (classic format; SDK 10 defaulted to `.slnx`) and four src projects plus tests.
  - Wired DIP project references; added Reqnroll, Reqnroll.xUnit, and Mvc.Testing (no AutoMapper/MediatR).
  - Replaced template WeatherForecast with `/health`; JWT signing key is empty in committed `appsettings.json` and a development-only placeholder in `appsettings.Development.json`.
  - Added `HostSmokeTests` and test folders `Features/`, `StepDefinitions/`, `Support/`, `Unit/`.
  - Updated `.gitignore` for `bin/`, `obj/`, SQLite, and user-secrets leftovers.
- **Decisions / outcomes:**
  - TASK-001 marked `DONE`. TASK-002 not started (one-task increment).
- **Files changed:**
  - `EagleBank.sln`
  - `src/EagleBank.*`
  - `tests/EagleBank.Tests`
  - `.gitignore`
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet build EagleBank.sln` succeeded (0 warnings). `dotnet test EagleBank.sln --no-build` — 1 passed, 0 failed.
- **Workflow transition:** implement-and-test `NOT_STARTED` → `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 13:38 — barclays-implement-and-test (TASK-002)

- **User intent:** Continue one task at a time with TASK-002 (exception framework, validation, logging).
- **Agent actions actually performed:**
  - Added Domain `EagleBankException` subtypes and `InvariantLog`.
  - Added Application `IExceptionResponseFactory` / `ExceptionResponseFactory` (FluentValidation → 400 + details; unknown → safe 500).
  - Registered one Api `IExceptionHandler`; request `RequestId` scope; route-template request log; FluentValidation action filter.
  - Added Infrastructure persist/JWT log helpers; `EfLogging.SensitiveDataLoggingEnabled` is false.
  - Added Reqnroll `ApiWebApplicationFactory`, hooks, shared HTTP steps, and xUnit factory/handler/layer tests.
- **Decisions / outcomes:**
  - TASK-002 marked `DONE`. TASK-003 not started.
- **Files changed:**
  - `src/EagleBank.Domain`
  - `src/EagleBank.Application`
  - `src/EagleBank.Infrastructure`
  - `src/EagleBank.Api`
  - `tests/EagleBank.Tests`
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 14 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 13:45 — barclays-implement-and-test (TASK-003)

- **User intent:** Implement TASK-003 only (Money + EF Core SQLite schema).
- **Agent actions actually performed:**
  - Added Domain `Money` (GBP, two decimals, integer pence) and User/Account/Transaction aggregates.
  - Added `EagleBankDbContext`, Fluent configurations (`balance_pence` / `amount_pence` INTEGER), unique email, cascade transactions.
  - Authored `InitialCreate` migration; Development applies it unless `SkipMigrations=true` (test host).
  - Persist `SaveChangesAsync` logs entity type + outcome; `EnableSensitiveDataLogging` stays false.
  - Added Money and isolated SQLite schema tests.
- **Decisions / outcomes:**
  - TASK-003 marked `DONE`. TASK-004 not started.
- **Files changed:**
  - `src/EagleBank.Domain`
  - `src/EagleBank.Infrastructure`
  - `src/EagleBank.Api`
  - `tests/EagleBank.Tests`
  - `tasks/tasks.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 25 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 13:53 — barclays-implement-and-test (TASK-004)

- **User intent:** Continue one approved task at a time; finish create-user and login.
- **Agent actions actually performed:**
  - Completed `POST /v1/users` (public, password required) and `POST /v1/auth/login` returning `{ token }`.
  - Duplicate email maps to 400 with details; unknown email and wrong password share 401.
  - Password hashing and JWT issue stay behind Application ports; OpenAPI includes password and login.
  - Added `Features/Users.feature` and `Features/Auth.feature`; escaped unused shared Then `/` so Reqnroll bindings load.
- **Decisions / outcomes:**
  - TASK-004 marked `DONE`. TASK-005 not started (one-task increment).
- **Files changed:**
  - `src/EagleBank.Api` (Users/Auth controllers)
  - `src/EagleBank.Application` (CreateUser, Login, ports)
  - `src/EagleBank.Infrastructure` (UserStore, hasher, JWT issuer)
  - `tests/EagleBank.Tests` (features, steps, factory)
  - `openapi.yaml`
  - `tasks/tasks.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 44 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 14:00 — barclays-implement-and-test (TASK-005)

- **User intent:** Continue one approved task at a time; implement authenticated fetch user.
- **Agent actions actually performed:**
  - Added JWT bearer authentication; create-user and login remain anonymous.
  - Added `GetUserQuery` / handler and shared `ResourceOwnership` helper (missing → 404, other owner → 403).
  - `GET /v1/users/{userId}`: missing/invalid token → 401; bad id pattern → 400.
  - Extended `Users.feature` with owner 200, 401, 403, 404, and 400 scenarios.
- **Decisions / outcomes:**
  - TASK-005 marked `DONE`. TASK-006 not started (one-task increment).
- **Files changed:**
  - `src/EagleBank.Api` (JWT auth, GET user)
  - `src/EagleBank.Application` (GetUser, ResourceOwnership)
  - `src/EagleBank.Infrastructure` (`IUserStore.GetByIdAsync`)
  - `tests/EagleBank.Tests` (Users.feature, steps, ownership unit tests)
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 52 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 14:34 — barclays-implement-and-test (TASK-006)

- **User intent:** Continue one approved task at a time; implement create and fetch account.
- **Agent actions actually performed:**
  - Added `CreateAccountCommand` / `GetAccountQuery` handlers and `POST /v1/accounts`, `GET /v1/accounts/{accountNumber}`.
  - Account numbers are `01` + 6 digits with unique-index retry; owner comes from JWT; `sortCode` `10-10-10`, GBP, balance `0.00`.
  - Reused `ResourceOwnership` for 403/404; added `Features/Accounts.feature`.
- **Decisions / outcomes:**
  - TASK-006 marked `DONE`. TASK-007 not started (one-task increment).
- **Files changed:**
  - `src/EagleBank.Api` (AccountsController, CallerUser)
  - `src/EagleBank.Application` (CreateAccount, GetAccount)
  - `src/EagleBank.Infrastructure` (AccountStore, AccountNumberFactory)
  - `tests/EagleBank.Tests` (Accounts.feature, steps)
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 66 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 14:53 — barclays-implement-and-test (TASK-007)

- **User intent:** Continue one approved task at a time; implement deposit and fetch transaction.
- **Agent actions actually performed:**
  - Added `CreateTransaction` / `GetTransaction` handlers and `POST/GET .../transactions`.
  - Deposit applies `BankAccount.Apply` then a conditional `UPDATE` in one DB transaction (ADR-0004); cap breach is 422 with no leftover row.
  - Fetch authorises the account first; unknown or wrong-account `transactionId` is 404.
  - OpenAPI `transactionId` pattern updated to `^tan-[A-Za-z0-9]+$` (Q-003).
- **Decisions / outcomes:**
  - TASK-007 marked `DONE`. Withdrawal SQL/domain branch exists but is not Gherkin-covered (TASK-009). TASK-008 not started.
- **Files changed:**
  - `src/EagleBank.Domain` (`BankAccount.Apply`)
  - `src/EagleBank.Application` (CreateTransaction, GetTransaction)
  - `src/EagleBank.Infrastructure` (TransactionStore, TransactionIdFactory)
  - `src/EagleBank.Api` (TransactionsController)
  - `tests/EagleBank.Tests` (Transactions.feature, apply unit tests)
  - `openapi.yaml`
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 90 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 14:58 — barclays-implement-and-test (TASK-008)

- **User intent:** Continue one approved task at a time; write README and confirm the submitted OpenAPI.
- **Agent actions actually performed:**
  - Wrote `README.md` with .NET 10 prerequisites, `dotnet test` / `dotnet run`, create-user, login, Bearer example, SQLite reset, JWT via env or user-secrets.
  - Documented AI assistance, permissive-only NuGet, no-PII logging, and production follow-ups (rate limiting, SAST/SCA, PostgreSQL, no balance cache) as documentation only.
  - Confirmed `openapi.yaml` already includes `password`, `POST /v1/auth/login` `{ token }`, and `^tan-[A-Za-z0-9]+$`; tightened the spec info description.
- **Decisions / outcomes:**
  - TASK-008 marked `DONE`. MVP MUST remaining is TASK-016 (quality pass). SHOULD next is TASK-009 (withdrawal Gherkin).
- **Files changed:**
  - `README.md`
  - `openapi.yaml` (info description)
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 103 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 16:50 — barclays-implement-and-test (TASK-009)

- **User intent:** Continue one approved task at a time; next SHOULD item is withdrawal.
- **Agent actions actually performed:**
  - Reused the existing create-transaction handler (`type=withdrawal`); no new service and no GetTransaction edits.
  - Added Gherkin for sufficient-funds 201 (balance 7.50) and insufficient-funds 422 (balance unchanged; original deposit still fetchable).
  - Domain already logged `InsufficientFunds` without amounts; denylist extended with withdrawal amounts.
- **Decisions / outcomes:**
  - TASK-009 marked `DONE`. TASK-010 not started (one-task increment).
- **Files changed:**
  - `tests/EagleBank.Tests` (Transactions.feature, steps, fixtures, apply unit test)
  - `README.md` (withdrawal now listed as implemented)
  - `tasks/tasks.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 106 passed, 0 failed.
- **Workflow transition:** none; implement-and-test remains `IN_PROGRESS`
- **Approval state:** implementing approved tasks only

---

### 2026-09-05 16:57 — barclays-implement-and-test (TASK-016)

- **User intent:** Run the final quality pass (TASK-016) rather than continuing lists/PATCH/DELETE.
- **Agent actions actually performed:**
  - Re-ran the full suite; reviewed SOLID (one handler per use case; no fat services; Application/Domain have no ASP.NET/EF references), licences (no AutoMapper/MediatR/`*.lic`), and four-layer logging plus PII denylist checks.
  - Removed the committed Development JWT placeholder; startup now fails if `Jwt:SigningKey` is under 32 characters. Added `*.lic` to `.gitignore`.
- **Decisions / outcomes:**
  - TASK-016 marked `DONE`. All MUST tasks are done. Remaining SHOULD/COULD: TASK-010–TASK-015.
  - implement-and-test set to `COMPLETE` for the chosen slice (MVP + withdrawal + quality pass).
- **Files changed:**
  - `src/EagleBank.Api/appsettings.Development.json`
  - `src/EagleBank.Api/Authentication/JwtAuthenticationExtensions.cs`
  - `.gitignore`
  - `README.md`
  - `lessons.md`
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 108 passed, 0 failed. `git ls-files '*.lic'` — none.
- **Workflow transition:** implement-and-test `IN_PROGRESS` → `COMPLETE`
- **Approval state:** not required; quality pass of approved tasks

---

### 2026-09-05 17:24 — barclays-implement-and-test (TASK-015)

- **User intent:** Implement Swagger UI for the submitted OpenAPI contract.
- **Agent actions actually performed:**
  - Served repo `openapi.yaml` at `GET /openapi.yaml` (resolved from content root / output, not a generated document).
  - Added Swashbuckle UI at `/swagger` pointing at that file; pinned `Microsoft.OpenApi` 2.7.5 to avoid GHSA-v5pm-xwqc-g5wc.
  - Documented the URL in the README; added host smoke tests for the spec and UI.
- **Decisions / outcomes:**
  - TASK-015 marked `DONE`. Remaining SHOULD/COULD: TASK-010–TASK-014.
- **Files changed:**
  - `src/EagleBank.Api` (Program, SubmittedOpenApi, csproj)
  - `tests/EagleBank.Tests/Unit/HostSmokeTests.cs`
  - `README.md`
  - `tasks/tasks.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln` — 110 passed, 0 failed.
- **Workflow transition:** implement-and-test briefly `IN_PROGRESS` → `COMPLETE`
- **Approval state:** implementing an approved COULD task on request

---

### 2026-09-05 17:50 — barclays-code-review

- **User intent:** Independent review of the completed implement-and-test slice.
- **Agent actions actually performed:**
  - Confirmed plan/design/tasks APPROVED and implement-and-test COMPLETE.
  - Inspected requirements, design, tasks, ADRs, OpenAPI, source, tests, README, and committed JWT config.
  - Did not change production code.
- **Decisions / outcomes:**
  - Review outcome **PASS_WITH_OBSERVATIONS**.
  - No CRITICAL/HIGH/MEDIUM findings. LOW: request-scope `userId` never set (middleware before auth); README first run command omits JWT env; validation `details.field` casing mixed.
  - Observations: stretch list/PATCH/DELETE advertised in Swagger but not implemented; TASK-013 concurrency test missing though conditional UPDATE exists.
- **Files changed:**
  - `reviews/code-review.md`
  - `.barclays/workflow-state.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln --nologo` — 110 passed, 0 failed.
- **Workflow transition:** implement-and-test remains `COMPLETE`; review `COMPLETE`; current stage `review`
- **Approval state:** review does not require user approval; implementation approvals unchanged

---

### 2026-09-05 17:56 — review follow-ups (items 1–4)

- **User intent:** Implement the four optional items listed in `reviews/code-review.md` (middleware `userId` scope, README JWT snippet, TASK-013, document wired OpenAPI operations).
- **Agent actions actually performed:**
  - Added `AuthenticatedUserScopeMiddleware` after `UseAuthentication`; `RequestContextMiddleware` now sets `RequestId` only.
  - Folded `Jwt__SigningKey` into the first README run snippet; added a wired-vs-not operations table.
  - Noted implemented vs unwired operations in `openapi.yaml` and a Swagger UI banner.
  - Added `ConcurrentWithdrawalTests` (TASK-013). Test log sink records only `RequestId` / `userId` scopes so ASP.NET `RequestPath` is not treated as a log line.
- **Decisions / outcomes:**
  - TASK-013 marked `DONE`. Remaining SHOULD/COULD: TASK-010–012, TASK-014.
  - Review outcome left as PASS_WITH_OBSERVATIONS; follow-up noted on the review file.
- **Files changed:**
  - `src/EagleBank.Api` (Program, middleware, SubmittedOpenApi)
  - `openapi.yaml`, `README.md`
  - `tests/EagleBank.Tests` (TestLogSink, HostSmokeTests, ConcurrentWithdrawalTests)
  - `tasks/tasks.md`, `reviews/code-review.md`, `lessons.md`
  - `.barclays/workflow-state.md`, `agent-interactions.md`
- **Verification actually run:** `dotnet test EagleBank.sln --nologo` — 112 passed, 0 failed.
- **Workflow transition:** implement-and-test briefly `IN_PROGRESS` → `COMPLETE`; current stage `review`
- **Approval state:** implementing approved TASK-013 plus user-requested review follow-ups

---

### 2026-09-05 18:08 — barclays-implement-and-test (stretch TASK-010–012, TASK-014)

- **User intent:** Implement remaining stretch: list, PATCH, DELETE, and Docker.
- **Agent actions actually performed:**
  - Added new list/update/delete handlers (create/get handlers left as-is). Account delete removes transactions first; user delete is `409` while accounts remain.
  - Added Reqnroll scenarios for the new operations.
  - Added `Dockerfile` + `compose.yaml`; JWT required via env (no default secret).
  - Documented Docker and marked all OpenAPI operations as wired.
- **Decisions / outcomes:**
  - TASK-010, TASK-011, TASK-012, TASK-014 marked `DONE`. All approved tasks are `DONE`.
  - SQLite LINQ `OrderBy(DateTimeOffset)` is not supported; lists sort in memory (LESSON-014).
- **Files changed:**
  - Application/Infrastructure/Api list, PATCH, DELETE files and controllers
  - `tests/EagleBank.Tests` features and steps
  - `Dockerfile`, `compose.yaml`, `.dockerignore`
  - `README.md`, `openapi.yaml`, `tasks/tasks.md`, `lessons.md`
- **Verification actually run:** `dotnet test EagleBank.sln --nologo` — 170 passed, 0 failed. `docker compose up --build -d` then `GET /health` and `GET /swagger/index.html` both `200`. `docker compose down`.
- **Workflow transition:** implement-and-test `IN_PROGRESS` → `COMPLETE`
- **Approval state:** implementing approved stretch tasks on request

---

