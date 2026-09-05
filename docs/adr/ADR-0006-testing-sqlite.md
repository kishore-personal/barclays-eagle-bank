# ADR-0006: xUnit integration tests with isolated SQLite

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** Pending user approval of design
- **Related requirements:** REQ-TEST-001–005

## Context

MVP paths and error codes must be proven with automated tests. The test store should exercise the same EF model as the app.

## Decision

Use xUnit. Cover domain invariants with unit tests. Cover HTTP behaviour with `WebApplicationFactory` against a unique SQLite database per fixture (file or SQLite in-memory with a shared relational cache as needed for EF). Assert status codes and OpenAPI JSON shapes.

## Alternatives considered

### Option A — WebApplicationFactory + SQLite (recommended)

- Advantages: Same provider as the app; real unique constraints; no Docker.
- Disadvantages: Slightly slower than pure unit tests.

### Option B — Testcontainers PostgreSQL

- Advantages: Closer to a production engine.
- Disadvantages: Docker required in CI and on a reviewer laptop.

### Option C — HTTP tests against EF InMemory

- Advantages: Fast.
- Disadvantages: Different behaviour for transactions, constraints, and concurrency.

## Consequences

### Positive

- REQ-TEST-001 can be demonstrated with `dotnet test`.
- Concurrency/balance tests can use the same fixture style.

### Negative / trade-offs

- Tests depend on EF/SQLite, so they are not pure unit tests. That is acceptable for API contract tests.

## Notes

Do not report a green build unless `dotnet test` was actually run.
