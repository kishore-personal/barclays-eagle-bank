# Engineering Lessons

Reusable lessons discovered while planning, designing, implementing, reviewing, or testing this repository.

Do not use this file as a diary. Capture lessons that could help another engineer or agent avoid repeating a mistake or understand a useful local convention.

## Entry template

### LESSON-001 — <short title>

- **Date:** YYYY-MM-DD
- **Context:** <what happened>
- **Lesson:** <reusable insight>
- **Action / convention:** <what future work should do>
- **Related files/tasks/ADRs:** <references>

---

### LESSON-001 — OpenAPI wins over scenario prose for API shape

- **Date:** 2026-09-05
- **Context:** The brief uses `/v1/accounts/{accountId}` while the attached OpenAPI uses `/v1/accounts/{accountNumber}` and pattern `^01\d{6}$`.
- **Lesson:** For this exercise, the OpenAPI document is authoritative for paths, methods, schemas, and status codes. Scenario text supplies behaviour (ownership, 403 vs 404, insufficient funds) but must not silently rename contract identifiers.
- **Action / convention:** When brief and spec disagree, record the contradiction, follow the spec for shape, and follow the brief for behaviour only where the spec is silent.
- **Related files/tasks/ADRs:** `openapi.yaml`, `requirements/technical-requirements.md` (A-001, REQ-API-008)

---

### LESSON-002 — Do not store GBP amounts as SQLite REAL

- **Date:** 2026-09-05
- **Context:** Designing persistence for two-decimal GBP balances on SQLite, which has no native decimal type.
- **Lesson:** Domain `decimal` is not enough if the store can coerce to floating point. Integer minor units (pence) keep scale exact.
- **Action / convention:** Persist `balance` and transaction `amount` as integer pence; convert only at the EF boundary; emit major units in JSON.
- **Related files/tasks/ADRs:** ADR-0003, REQ-NFR-004

---

### LESSON-003 — Default framework logs leak identifiers

- **Date:** 2026-09-05
- **Context:** Design amended to log at every layer without PII.
- **Lesson:** ASP.NET request logs print raw URLs (`/v1/accounts/01234567`) and EF `EnableSensitiveDataLogging` prints SQL parameter values such as email. Layered logging is unsafe unless those defaults are turned off and only an allow-list is logged.
- **Action / convention:** Log route templates, not raw paths; never enable EF sensitive-data logging; never log request bodies; correlate with `RequestId` and `usr-` / `tan-` ids only.
- **Related files/tasks/ADRs:** ADR-0007, `design/system-design.md` section 12

---

### LESSON-004 — CQRS for CRUD means handlers, not two databases

- **Date:** 2026-09-05
- **Context:** Design discussion on whether CQRS reduces regression on basic CRUD.
- **Lesson:** Separate command and query handlers isolate change (SRP/ISP) and keep stretch work off the MVP files. Separate read stores, events, and MediatR are not required for that benefit.
- **Action / convention:** One handler per use case, shared ownership helpers, one EF model; reject dual-database CQRS unless a scaling requirement appears.
- **Related files/tasks/ADRs:** ADR-0008, REQ-NFR-003

---

### LESSON-005 — Keep Gherkin for HTTP, xUnit for units and concurrency

- **Date:** 2026-09-05
- **Context:** The brief is Given/When/Then; domain money and parallel withdrawals are not narrative HTTP stories.
- **Lesson:** Feature files trace acceptance outcomes to the brief. Value-object rules and two-at-once requests stay clearer as xUnit.
- **Action / convention:** Reqnroll for `/v1` status and body scenarios; xUnit under `Unit/` for Money, exception mapping, and TASK-013 concurrency.
- **Related files/tasks/ADRs:** ADR-0006, ADR-0013, `tasks/tasks.md`

---

### LESSON-006 — SOLID needs a checklist, not only folder names

- **Date:** 2026-09-05
- **Context:** Clean architecture + CQRS handlers already implied SOLID, but the task list did not say so explicitly.
- **Lesson:** A `UserService` can still sit in Application and violate SRP/ISP. Maintainability comes from one handler per use case, narrow ports, and composition root only in Api.
- **Action / convention:** Follow ADR-0014 during implementation; TASK-016 reviews project references and rejects fat services.
- **Related files/tasks/ADRs:** ADR-0014, ADR-0008, REQ-NFR-003

---

### LESSON-007 — Do not take AutoMapper or MediatR for a public POC

- **Date:** 2026-09-05
- **Context:** AutoMapper 15+ and MediatR 13+ are dual RPL-1.5 / commercial (Lucky Penny). Older MIT/Apache builds remain, but they are unsupported.
- **Lesson:** A take-home restore must not require a licence key or RPL obligations. Pinning last-MIT versions still looks like a licence risk and skips security patches.
- **Action / convention:** Manual DTO mapping; explicit handlers; NuGet limited to MIT/Apache/BSD/Microsoft. TASK-016 audits package IDs.
- **Related files/tasks/ADRs:** ADR-0015, ADR-0008

---

### LESSON-008 — .NET 10 `dotnet new sln` defaults to `.slnx`

- **Date:** 2026-09-05
- **Context:** TASK-001 required `EagleBank.sln`. SDK 10.0.400 created `EagleBank.slnx` instead, so `dotnet sln EagleBank.sln add` failed.
- **Lesson:** The new XML solution format is the SDK default; reviewers and scripts that look for `.sln` will miss it.
- **Action / convention:** Create the classic format with `dotnet new sln --format sln` when the task or README names a `.sln` file.
- **Related files/tasks/ADRs:** TASK-001, REQ-NFR-001

---
