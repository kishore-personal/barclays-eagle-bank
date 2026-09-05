# ADR-0005: JWT bearer with PasswordHasher, not ASP.NET Identity

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** Pending user approval of design
- **Related requirements:** REQ-AUTH-001–007, REQ-NFR-005, REQ-USER-004, Q-001, Q-002

## Context

The approved plan adds `password` on create user and `POST /v1/auth/login` returning `{ "token" }`. Protected routes use bearer JWT. ASP.NET Identity is available but heavy for a three-resource API.

## Decision

Store a password hash on `User` using `PasswordHasher<T>`. Issue HMAC-SHA256 JWTs with `sub` = `userId`. Configure `JwtBearer` for all `/v1` endpoints except create user and login. Do not use ASP.NET Identity, cookies, or refresh tokens.

## Alternatives considered

### Option A — Custom user store + JWT (recommended)

- Advantages: Small surface; hash and claims are obvious in a walkthrough; matches the OpenAPI extension.
- Disadvantages: We implement login ourselves (acceptable at this size).

### Option B — ASP.NET Identity + JWT

- Advantages: Battle-tested user/lockout plumbing.
- Disadvantages: Extra tables and conventions that the OpenAPI does not need; harder to walk through quickly.

### Option C — Unsigned or static tokens

- Advantages: Fastest.
- Disadvantages: Unacceptable for REQ-NFR-005 and interview credibility.

## Consequences

### Positive

- Owner checks use `sub` only; clients cannot spoof `userId`.
- Signing key stays in configuration, not source control.

### Negative / trade-offs

- No built-in lockout or refresh; document as a production follow-up.

## Notes

Proposed token lifetime is 60 minutes (DQ-002). Login failures use one `401` message for unknown email and bad password.
