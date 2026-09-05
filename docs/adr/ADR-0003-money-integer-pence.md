# ADR-0003: Money value object persisted as integer pence

- **Status:** Proposed
- **Date:** 2026-09-05
- **Decision owners:** Pending user approval of design
- **Related requirements:** REQ-NFR-004, REQ-ACCOUNT-013, REQ-TXN-004, A-007

## Context

Balances and amounts are GBP with two decimal places. Binary floating point is forbidden. SQLite has no native decimal type, so storing `double` or naive `decimal` mappings is risky.

## Decision

Represent money in the domain as a `Money` value object (GBP + `decimal` major units, two-decimal scale). Persist `balance` and `amount` as integer pence (`long`). Convert only at the persistence boundary.

## Alternatives considered

### Option A — Integer pence in storage (recommended)

- Advantages: Exact integer arithmetic; no SQLite type surprises; easy to explain.
- Disadvantages: Conversion at the repository/EF boundary.

### Option B — `decimal` columns / EF default mapping

- Advantages: Matches the OpenAPI `number` mentally.
- Disadvantages: SQLite affinity can coerce values; easier to get scale wrong.

### Option C — Store pence in the domain too

- Advantages: One representation.
- Disadvantages: API mapping and OpenAPI examples are in major units (`10.99`); more error-prone at the HTTP edge.

## Consequences

### Positive

- Invariants compare integers or scaled decimals safely.
- Interview story is clear: never use `double` for money.

### Negative / trade-offs

- Mappers must multiply/divide by 100 in one place only.

## Notes

JSON responses still emit major-unit numbers as required by the OpenAPI schemas.
