# ADR-0012: Generic injected exception-handling framework

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit request during task review)
- **Related requirements:** REQ-API-004, REQ-API-005, REQ-ERR-001–008, REQ-NFR-003

## Context

Controllers and handlers that each `try/catch` and build HTTP bodies will drift and become hard to change. The user asked for a generic, injected exception-handling process so new use cases stay maintainable.

## Decision

Use one ASP.NET Core `IExceptionHandler` (or equivalent middleware) registered in DI. Application and domain code throw typed exceptions that inherit a single base (`EagleBankException`) carrying a safe message and a reason code. An injected mapper (`IExceptionResponseFactory`) turns those types into OpenAPI `ErrorResponse` / `BadRequestErrorResponse` and the HTTP status.

FluentValidation failures enter the same pipeline (400 + `details`). Unhandled exceptions become 500 with a fixed safe message. Controllers and command/query handlers do not catch business exceptions or set status codes.

Adding a new failure mode means: one exception type + one mapper registration (or convention on the base type). No controller edits.

## Alternatives considered

### Option A — Injected handler + typed exceptions (recommended)

- Advantages: Single place to change status/body; testable mapper; handlers stay thin; SOLID (OCP).
- Disadvantages: Small amount of framework code on day one.

### Option B — try/catch in every controller

- Advantages: Obvious in a single file.
- Disadvantages: Copy-paste; easy to return the wrong body shape; unmaintainable as endpoints grow.

### Option C — Filter attribute per action

- Advantages: Explicit per route.
- Disadvantages: Easy to forget on a new action; still scattered.

## Consequences

### Positive

- Stretch endpoints reuse the same process.
- Logging of exception type stays in one handler (ADR-0007).

### Negative / trade-offs

- Developers must throw the typed exceptions, not return magic integers from handlers.

## Notes

JWT 401 remains the auth middleware. This framework covers application/domain/validation failures after the request is in the pipeline.
