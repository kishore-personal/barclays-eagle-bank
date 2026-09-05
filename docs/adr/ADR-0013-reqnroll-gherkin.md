# ADR-0013: Reqnroll Gherkin for HTTP acceptance tests

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit request during task review)
- **Related requirements:** REQ-TEST-001–005

## Context

The exercise brief is written as Given/When/Then scenarios. The first task list covered those outcomes as xUnit `HttpClient` tests only. The user asked for unit tests **and** integration tests in Gherkin style taken from those requirements.

## Decision

Keep **xUnit unit tests** for domain invariants and the exception mapper (ADR-0006 still applies for the host and SQLite fixture).

Express **HTTP acceptance / integration** cases as Gherkin `.feature` files run by **Reqnroll** (the maintained SpecFlow successor) on xUnit. Step definitions call the same `WebApplicationFactory` + isolated SQLite host as ADR-0006.

Feature files live under `tests/EagleBank.Tests/Features/`. Wording follows the brief’s scenarios, corrected for approved contract rules (`accountNumber`, create-user is public, PATCH is partial, duplicate email is `400`, account delete has no `409`, `tan-` + one-or-more alphanumeric).

Do not use abandoned SpecFlow packages.

## Alternatives considered

### Option A — Reqnroll + xUnit unit tests (recommended)

- Advantages: Reviewer can read the brief next to `.feature` files; `dotnet test` still runs everything; unit tests stay fast and precise.
- Disadvantages: Extra NuGet packages and step-definition glue.

### Option B — xUnit-only HTTP tests

- Advantages: Less tooling.
- Disadvantages: Rejected by the user; scenarios are harder to trace to the brief.

### Option C — SpecFlow

- Advantages: Familiar name.
- Disadvantages: SpecFlow is unmaintained; Reqnroll is the .NET successor.

## Consequences

### Positive

- REQ-TEST-001–003 map to named scenarios, not only method names.
- Stretch features add scenarios to the same files.

### Negative / trade-offs

- Shared steps must stay generic (`I receive a 403 response`) so they do not encode business rules.
- Concurrent withdrawal (TASK-013) stays an xUnit test; Gherkin is a poor fit for two parallel HTTP calls.

## Notes

This extends ADR-0006; it does not replace isolated SQLite or `WebApplicationFactory`.
