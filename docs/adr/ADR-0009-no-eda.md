# ADR-0009: Do not implement event-driven architecture

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit design approval)
- **Related requirements:** REQ-NFR-006, REQ-TXN-016, REQ-DATA-003, REQ-DATA-004, REQ-API-003

## Context

The user asked whether EDA is needed for performance. The API is a synchronous REST contract: create/fetch must return the persisted resource and the new balance in the same response. Approved NFRs are take-home scale (single service, no throughput or HA target).

## Decision

Do not introduce a message broker, domain-event bus, outbox, or async consumers. Keep writes in the HTTP request’s database transaction (ADR-0004). Performance work, if any, stays in-process: indexed lookups, `AsNoTracking()` on queries, and a later PostgreSQL swap.

## Alternatives considered

### Option A — Request/response + one database (recommended)

- Advantages: Matches `201`/`200` with the new state; atomic balance + transaction; no broker to run; latency is one local commit.
- Disadvantages: No fan-out to other systems (none are in scope).

### Option B — EDA for writes (publish TransactionCreated, update balance in a consumer)

- Advantages: Useful when many downstream systems must react.
- Disadvantages: The HTTP caller would wait for eventual consistency or a complex saga; overdraft protection becomes harder; extra infrastructure does not raise throughput for a single SQLite writer; worse p95 for the REST path.

### Option C — In-process domain events after commit

- Advantages: Decouples logging or future notifications without a broker.
- Disadvantages: Still not a performance win; easy to grow into hidden side effects. Structured `ILogger` already covers traceability (ADR-0007).

## Consequences

### Positive

- Balance invariants stay in one transaction.
- Reviewer runs one process.
- Interview story: EDA when you have many consumers or absorb spikes; not when one request must return a consistent ledger line.

### Negative / trade-offs

- Notifications, fraud scoring, or statement engines would need a later outbox. That is pairing-session work, not this submission.

## Notes

CQRS here means command/query handlers (ADR-0008), not an event-driven write side.
