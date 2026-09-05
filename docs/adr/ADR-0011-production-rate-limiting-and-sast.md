# ADR-0011: Production rate limiting and SAST across the SDLC

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** User requested; pending design approval
- **Related requirements:** REQ-NFR-005, REQ-NFR-006, REQ-ERR-001, REQ-DEL-003

## Context

Anonymous `POST /v1/users` and `POST /v1/auth/login` are guessable and can be abused (credential stuffing, signup flooding). The take-home has no throughput or abuse target. The user asked that production include rate limiting, and that SAST exist through most of the SDLC to catch vulnerabilities early.

## Decision

**This submission:** do not implement rate limiting or a SAST pipeline. Keep the security controls already designed (JWT, hashed passwords, no PII in logs, no committed secrets).

**When productionising:**

1. Rate-limit at the edge (API gateway or ASP.NET `RateLimiter`) before handlers run. Tightest limits on login and create-user (by IP). Authenticated routes may also limit by `userId`. Exceeded limits return `429` with `Retry-After` and an `ErrorResponse`; do not log email or token. Document `429` in the production OpenAPI/gateway contract.
2. Run SAST at multiple SDLC points, not only at release: IDE/pre-commit where practical, every pull request / CI build, scheduled full-branch scans, and a release gate that fails the pipeline on new high/critical findings. Pair SAST with dependency vulnerability scanning (SCA). Do not treat a single late scan as sufficient.

## Alternatives considered

### Option A — Document for production, omit from take-home (recommended)

- Advantages: Honest scope; interview-ready; no extra infrastructure for reviewers.
- Disadvantages: The submitted app can be brute-forced locally (acceptable for the exercise).

### Option B — In-process rate limiting in the take-home

- Advantages: Demonstrates `429`.
- Disadvantages: Needs a shared store to be correct across instances; not asked by the brief; easy to get wrong.

### Option C — SAST only at final release

- Advantages: One pipeline job.
- Disadvantages: Findings arrive too late; expensive to fix; misses the point of shift-left.

## Consequences

### Positive

- Production abuse and code-flaw controls are explicit.
- SAST is framed as continuous, not a one-off audit.

### Negative / trade-offs

- Reviewers will not see live rate-limit or SAST jobs unless a pairing session adds them.

## Notes

Rate limiting does not replace lockout, MFA, or WAF rules in a real bank. SAST does not replace DAST, code review, or secrets scanning.
