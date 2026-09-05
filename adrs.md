# Architecture Decision Record Index

| ADR | Title | Status | Date | Related requirements |
|---|---|---|---|---|
| ADR-0001 | Clean architecture with controller HTTP boundary | Accepted | 2026-09-05 | REQ-NFR-001, REQ-NFR-003, REQ-API-001 |
| ADR-0002 | EF Core 10 with SQLite | Accepted | 2026-09-05 | REQ-DATA-001, REQ-DATA-002, REQ-NFR-002, REQ-NFR-006, A-009 |
| ADR-0003 | Money value object persisted as integer pence | Accepted | 2026-09-05 | REQ-NFR-004, REQ-ACCOUNT-013, REQ-TXN-004, A-007 |
| ADR-0004 | Atomic balance update with a conditional write | Accepted | 2026-09-05 | REQ-TXN-005–007, REQ-TXN-016, REQ-DATA-003–004, Q-006 |
| ADR-0005 | JWT bearer with PasswordHasher, not ASP.NET Identity | Accepted | 2026-09-05 | REQ-AUTH-001–007, REQ-NFR-005, REQ-USER-004, Q-001, Q-002 |
| ADR-0006 | xUnit unit tests + WebApplicationFactory + isolated SQLite | Accepted | 2026-09-05 | REQ-TEST-001–005 |
| ADR-0007 | Structured logging on every layer without PII | Accepted | 2026-09-05 | REQ-ERR-007, REQ-ERR-008, REQ-NFR-005, REQ-DEL-003 |
| ADR-0008 | Lightweight CQRS (command/query handlers, one store) | Accepted | 2026-09-05 | REQ-NFR-003, REQ-TEST-001 |
| ADR-0009 | Do not implement event-driven architecture | Accepted | 2026-09-05 | REQ-NFR-006, REQ-TXN-016, REQ-DATA-003–004, REQ-API-003 |
| ADR-0010 | No application cache in the submission | Accepted | 2026-09-05 | REQ-NFR-006, REQ-DATA-003, REQ-TXN-005, REQ-TXN-016, REQ-API-003 |
| ADR-0011 | Production rate limiting and SAST across the SDLC | Accepted | 2026-09-05 | REQ-NFR-005, REQ-NFR-006, REQ-ERR-001, REQ-DEL-003 |
| ADR-0012 | Generic injected exception-handling framework | Accepted | 2026-09-05 | REQ-API-004–005, REQ-ERR-001–008, REQ-NFR-003 |
| ADR-0013 | Reqnroll Gherkin for HTTP acceptance tests | Accepted | 2026-09-05 | REQ-TEST-001–005 |
| ADR-0014 | Apply SOLID so the API stays maintainable | Accepted | 2026-09-05 | REQ-NFR-003 |
| ADR-0015 | Permissive, license-key-free dependencies only | Accepted | 2026-09-05 | REQ-DEL-003, REQ-NFR-003, REQ-NFR-006 |

## Status values

- Proposed
- Accepted
- Rejected
- Superseded
- Deprecated
