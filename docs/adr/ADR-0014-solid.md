# ADR-0014: Apply SOLID so the API stays maintainable

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit request during task review)
- **Related requirements:** REQ-NFR-003

## Context

The pairing session will extend this API. Fat services, controllers that own rules, and hidden dependencies (`IMediator`, god interfaces) make that hard. The user asked that SOLID be used so the application stays maintainable. Several accepted ADRs already point that way (ADR-0001, ADR-0008, ADR-0012); this record makes the five principles binding for implementation.

## Decision

Implement against these conventions. They constrain structure; they do not add product features.

| Principle | Convention |
|---|---|
| **S** Single responsibility | One reason to change per type. Controllers bind HTTP only. One command or query handler per use case. Domain owns money and balance invariants. Persistence stays in Infrastructure. Exception-to-HTTP mapping stays in the injected factory. |
| **O** Open/closed | Add a use case as a new handler (and validator) without editing existing handlers. Add a failure as a new `EagleBankException` subtype + mapper convention (ADR-0012). Stretch PATCH/DELETE/list must not rewrite Create/Get. |
| **L** Liskov substitution | Ports and exception subtypes remain substitutable. An Infrastructure implementation of an Application port must honour the same outcomes (success, typed exceptions). Do not weaken contracts in “test doubles that succeed always” used as production types. |
| **I** Interface segregation | Controllers inject the specific handler they call, not `IUserService` / `IAccountService` with unused methods, and not `IMediator`. Application ports are narrow (persist user, issue JWT, hash password). Shared ownership is a small helper, not a kitchen-sink service. |
| **D** Dependency inversion | Application defines ports. Infrastructure implements them. Domain depends on nothing above it. Api is the composition root (registers implementations). Application and Domain must not reference ASP.NET, EF Core, or SQLite types. |

Forbidden for maintainability: multi-method application services; business rules in controllers; `try/catch` that sets status in actions; copy-pasted 403/404 blocks; Application project referencing `Microsoft.EntityFrameworkCore` or `Microsoft.AspNetCore.*`.

Allowed: one `CreateTransaction` handler for deposit and withdrawal (one use case); deposit vs withdraw invariants live on the domain account, not as a second god service.

## Alternatives considered

### Option A — Explicit SOLID conventions (recommended)

- Advantages: Reviewable in TASK-016; matches interview walkthrough; pairing adds files instead of editing fat classes.
- Disadvantages: More types than a single-project CRUD demo.

### Option B — Rely on clean architecture alone

- Advantages: Fewer words in the task list.
- Disadvantages: Easy to put a `UserService` in Application and still call it “clean architecture”.

## Consequences

### Positive

- REQ-NFR-003 (explainable, extensible) has a checklist, not only folder names.

### Negative / trade-offs

- More files. Shared rules must stay in small helpers so SRP does not become copy-paste.

## Notes

Does not change HTTP contracts or persistence choices. Complements ADR-0001, ADR-0008, and ADR-0012.
