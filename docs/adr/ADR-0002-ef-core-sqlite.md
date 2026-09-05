# ADR-0002: EF Core 10 with SQLite

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit design approval)
- **Related requirements:** REQ-DATA-001, REQ-DATA-002, REQ-NFR-002, REQ-NFR-006, A-009

## Context

Data must survive across HTTP requests. The take-home should run locally without a mandatory external database. Persistence technology was left to design.

## Decision

Use EF Core 10 with a SQLite file for local runs. Use the same model in tests with an isolated SQLite database per fixture. Keep provider configuration in Infrastructure so PostgreSQL can replace SQLite later.

## Alternatives considered

### Option A — EF Core + SQLite (recommended)

- Advantages: Real SQL, migrations, no Docker required, easy reviewer setup.
- Disadvantages: Weaker concurrency and types than PostgreSQL; decimal must be stored carefully (see ADR-0003).

### Option B — EF Core + PostgreSQL + Docker

- Advantages: Closer to production banking stores; native `numeric`.
- Disadvantages: Reviewer must run Docker; more failure modes in a 7-day take-home.

### Option C — In-memory dictionary or EF InMemory as the app store

- Advantages: Fastest to write.
- Disadvantages: Weak concurrency story; data vanishes on restart unless extra work is added; less impressive in interview.

## Consequences

### Positive

- `dotnet run` is enough to review.
- Schema and unique constraints are explicit.

### Negative / trade-offs

- Must document that production would use a managed relational database.

## Notes

In-memory EF remains acceptable only as a last-resort unit-test aid, not as the application store.
