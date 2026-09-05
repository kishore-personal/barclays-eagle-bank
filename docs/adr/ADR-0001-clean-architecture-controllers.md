# ADR-0001: Clean architecture with controller HTTP boundary

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** Pending user approval of design
- **Related requirements:** REQ-NFR-001, REQ-NFR-003, REQ-API-001

## Context

The API must be explainable in interview and easy to extend in a pairing session. .NET 10 allows controllers or Minimal APIs, and a single project or several.

## Decision

Use four projects — Api, Application, Domain, Infrastructure — plus a test project. Expose HTTP with ASP.NET Core controllers, not Minimal APIs.

## Alternatives considered

### Option A — Controllers + clean architecture (recommended)

- Advantages: Familiar in enterprise .NET; clear place for JWT and error mapping; pairing can add an endpoint without touching domain invariants.
- Disadvantages: More projects and files than a single-folder app.

### Option B — Minimal APIs in one project

- Advantages: Less ceremony; modern ASP.NET style.
- Disadvantages: Business rules tend to leak into endpoint lambdas; less familiar for many Barclays-style codebases.

### Option C — Full CQRS / MediatR as the only structure

- Advantages: Very explicit use-case types.
- Disadvantages: Dual-store CQRS does not pay off at this size. Application-level command/query handlers are decided separately in ADR-0008.

## Consequences

### Positive

- Ownership, money, and persistence can be tested without HTTP.
- Stretch endpoints slot in as new handlers (ADR-0008), not as edits to a god service.

### Negative / trade-offs

- Slightly more solution plumbing on day one.

## Notes

HTTP boundary remains controllers. How Application is sliced (CQRS handlers) is ADR-0008.
