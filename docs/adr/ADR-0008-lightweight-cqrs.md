# ADR-0008: Lightweight CQRS (command/query handlers, one store)

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit design approval)
- **Related requirements:** REQ-NFR-003, REQ-TEST-001

## Context

CRUD-style endpoints can be implemented as a few application services (`UserService` with create/get/update/delete). That concentrates change: a PATCH bug can regress CREATE. The user asked whether CQRS is better for regression impact and SOLID.

Full CQRS (separate read model, two databases, events) is disproportionate for this take-home. Application-level CQRS — one command or query object and one handler per use case, same SQLite/EF model — gives isolation without that cost.

## Decision

Use lightweight CQRS in the Application project:

- Writes are commands (`CreateUserCommand`, `CreateTransactionCommand`, …) handled by a matching command handler.
- Reads are queries (`GetUserQuery`, `ListAccountsQuery`, …) handled by a matching query handler. Queries use `AsNoTracking()`.
- Controllers depend on the specific handler interface (or a tiny dispatcher), not a god service.
- One EF Core / SQLite store remains the source of truth (ADR-0002). No read replica, no event store, no MediatR unless a later pairing session wants a mediator.

## Alternatives considered

### Option A — Lightweight CQRS, explicit handlers (recommended)

- Advantages: One reason to change per handler (SRP); adding PATCH does not edit Create; queries can stay simple; easy to unit-test one use case; logging/authz sit in the handler without a fat service.
- Disadvantages: More types than a single `UserService`.

### Option B — Fat application services

- Advantages: Fewer files; faster to type on day one.
- Disadvantages: Higher regression blast radius; ISP violation (callers depend on unused methods).

### Option C — Full CQRS (separate read DB + events)

- Advantages: Independent scale of reads and writes.
- Disadvantages: Eventual consistency, dual schemas, and no requirement that justifies it (REQ-NFR-006).

### Option D — MediatR as the dispatcher

- Advantages: Pipeline behaviours for logging/validation.
- Disadvantages: Controllers depend on `IMediator` (hides real dependencies); extra package to explain in interview. Explicit handler injection keeps SOLID (especially ISP) clearer.

## Consequences

### Positive

- Stretch endpoints are new handler files, not edits to MVP handlers.
- Handler tests target one behaviour.
- ADR-0007 logging can name the command/query type as the use case.

### Negative / trade-offs

- More files. Shared rules (ownership `403`/`404`, email uniqueness) must live in small shared helpers so they are not copy-pasted.

## Notes

This amends the earlier draft that treated “CQRS / MediatR” as out of scope. That rejection applied to full CQRS, not to command/query handler split.
