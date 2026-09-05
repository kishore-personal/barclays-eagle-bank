# ADR-0015: Permissive, license-key-free dependencies only

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit request during task review)
- **Related requirements:** REQ-DEL-003, REQ-NFR-003, REQ-NFR-006

## Context

This repository is a public take-home / POC. The user asked that there be no licence implications — for example AutoMapper or MediatR. Those two libraries (AutoMapper 15+ and MediatR 13+) are dual-licensed Reciprocal Public License 1.5 / Lucky Penny commercial. A Community tier exists, but it still means license acceptance, possible keys, and reviewer/legal ambiguity. Older MIT/Apache builds exist but are unsupported and are a poor look on a public submission.

The design already rejected MediatR for SOLID (ADR-0008). This ADR rejects it for licensing as well, and rejects AutoMapper and any similar package.

## Decision

Use only **permissive, no-key** dependencies: MIT, Apache-2.0, BSD, or equivalent first-party Microsoft/.NET libraries. Map DTOs **by hand** (small explicit mappers). Do not add AutoMapper, MediatR, Mapster (unnecessary), or any package that requires a paid licence, a licence key, RPL/copyleft that would bind this repo, or a NuGet licence-acceptance prompt.

**Allowed (examples):** ASP.NET Core, EF Core, SQLite provider, `PasswordHasher<T>`, `System.IdentityModel.Tokens.Jwt` / Microsoft JWT, FluentValidation (Apache-2.0), xUnit, `Microsoft.AspNetCore.Mvc.Testing`, Reqnroll (permissive).

**Forbidden (examples):** AutoMapper (any version, including pre-15 “still MIT”), MediatR (any version), Duende IdentityServer, commercial UI/component suites, packages that need `LicenseKey` configuration.

Do not commit licence keys or accept-license files.

## Alternatives considered

### Option A — Manual mapping + permissive packages (recommended)

- Advantages: No commercial or RPL exposure; no key in config; matches a small DTO surface; reviewers can `dotnet restore` with no extra agreement.
- Disadvantages: A few mapping methods to write.

### Option B — Pin AutoMapper 14 / MediatR 12 (last MIT/Apache)

- Advantages: Familiar APIs without a new licence.
- Disadvantages: Unsupported line; known AutoMapper issues without a free patch path; the package names still look like a licence risk to reviewers. Rejected.

### Option C — Current AutoMapper/MediatR under Community / non-production terms

- Advantages: Latest features.
- Disadvantages: Dual RPL/commercial; key/acceptance ceremony; unclear for a public Barclays-branded POC. Rejected.

## Consequences

### Positive

- Restore and review need no licence paperwork.
- Aligns with explicit handler injection (ADR-0008) and SOLID (ADR-0014).

### Negative / trade-offs

- Mapping and dispatch are written in-repo. That is acceptable at this size.

## Notes

If a later pairing session wants AutoMapper or MediatR, treat it as a licensed dependency decision, not a default.
