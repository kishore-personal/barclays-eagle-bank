# ADR-0010: No application cache in the submission; production cache only non-ledger reads

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** Pending user approval of design
- **Related requirements:** REQ-NFR-006, REQ-DATA-003, REQ-TXN-005, REQ-TXN-016, REQ-API-003

## Context

The user asked whether productionising the app requires caching. The take-home has no throughput target. A bank-style API returns balances and transactions that must match the last committed write.

## Decision

Do not add Redis, `IMemoryCache`, or HTTP response caching in this submission. Query handlers read SQLite/EF directly (`AsNoTracking()`).

If the service is productionised later, add a cache only after measurement, and only for data that is not the ledger source of truth:

- Reasonable: user profile reads, invalidated on user PATCH/DELETE; static reference data.
- Not reasonable as source of truth: `balance`, create-transaction results, or transaction lists used to decide a withdrawal.

Production HTTP responses for authenticated banking data should use `Cache-Control: no-store` (or equivalent), not public CDN caching.

## Alternatives considered

### Option A — No cache now (recommended)

- Advantages: No stale balance; one less moving part; matches take-home scale.
- Disadvantages: Every GET hits the database (acceptable here).

### Option B — Cache balances and account GETs in Redis

- Advantages: Lower read latency under heavy GET traffic.
- Disadvantages: Easy to serve a stale balance after a deposit; invalidation bugs become overdrafts or customer-trust issues.

### Option C — Short-TTL cache on all GETs

- Advantages: Simple.
- Disadvantages: Still stale within the TTL; does not help writes; hides whether the database is actually the problem.

## Consequences

### Positive

- Ledger reads stay consistent with ADR-0004.
- Production conversation is explicit: cache profile, never treat cache as the balance.

### Negative / trade-offs

- A pairing session that wants Redis must add it behind a port (for example `IUserProfileCache`) without touching command handlers.

## Notes

Before adding a cache in production, prefer indexes, connection pooling, and PostgreSQL. Cache is not the first performance lever.
