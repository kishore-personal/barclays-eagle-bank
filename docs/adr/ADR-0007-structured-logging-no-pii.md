# ADR-0007: Structured logging on every layer without PII

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit design approval)
- **Related requirements:** REQ-ERR-007, REQ-ERR-008, REQ-NFR-005, REQ-DEL-003

## Context

The user asked for logging at all layers for traceability, with no PII in logs. ASP.NET and EF Core defaults can emit raw URLs, request bodies, and SQL parameter values (email, account numbers, hashes).

## Decision

Use `ILogger<T>` in Api, Application, Domain, and Infrastructure. Push a request-scoped correlation id and, when authenticated, the opaque `userId` (`usr-…`) via `BeginScope`. Log use-case start/success/failure and invariant/persistence outcomes using allow-listed fields only.

Never log the denylist in section 12 of `design/system-design.md`. Log route templates (`/v1/accounts/{accountNumber}`), not raw paths. Keep `EnableSensitiveDataLogging` off. Do not log request or response bodies.

## Alternatives considered

### Option A — Layered structured logs + allow-list (recommended)

- Advantages: Traceable across layers; reviewable in interview; reduces accidental PII.
- Disadvantages: Slightly more logger calls than controller-only logging.

### Option B — Framework request logging only

- Advantages: Almost no code.
- Disadvantages: Raw paths include account numbers; easy to log bodies in Development; no domain/application trace.

### Option C — Full audit of every field, then redact

- Advantages: Maximum forensic detail.
- Disadvantages: Easy to miss a field; bodies and SQL params are the usual leak.

## Consequences

### Positive

- A reviewer can follow `RequestId` + `userId` + use case without seeing email or address.
- EF will not dump parameter values.

### Negative / trade-offs

- Account numbers are not logged in full, so ops must use `userId` + transaction id + result codes to investigate.

## Notes

This ADR was a user-requested amendment and was accepted with design approval on 2026-09-05.
