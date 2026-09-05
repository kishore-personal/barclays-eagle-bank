# ADR-0004: Atomic balance update with a conditional write

- **Status:** Accepted
- **Date:** 2026-09-05
- **Decision owners:** User (explicit design approval)
- **Related requirements:** REQ-TXN-005, REQ-TXN-006, REQ-TXN-007, REQ-TXN-016, REQ-DATA-003, REQ-DATA-004, Q-006

## Context

A deposit or withdrawal must insert an immutable transaction and change `balance` together. Concurrent withdrawals must not produce a negative balance. A deposit must not push the balance above `10000.00`.

## Decision

Run each create-transaction use case in one database transaction. Apply the balance change with a conditional `UPDATE`:

- Withdrawal succeeds only if `balance_pence >= amount_pence`.
- Deposit succeeds only if `balance_pence + amount_pence <= 1000000`.

If the update affects zero rows, roll back and return `422`. Do not insert the transaction.

## Alternatives considered

### Option A — Conditional UPDATE (recommended)

- Advantages: Atomic; works on SQLite; no overdraft window; easy to test.
- Disadvantages: Slightly more explicit SQL/EF execute than a blind in-memory add.

### Option B — Optimistic concurrency token (`rowversion` / version column)

- Advantages: Standard EF pattern.
- Disadvantages: Callers would need a retry; insufficient funds vs stale write must be distinguished.

### Option C — `SELECT FOR UPDATE` / serializable isolation

- Advantages: Familiar pessimistic locking on PostgreSQL.
- Disadvantages: SQLite does not offer row-level `FOR UPDATE` in the same way; overkill if Option A holds.

## Consequences

### Positive

- Failed withdrawals leave no residue.
- Stretch concurrent-withdrawal test is feasible.

### Negative / trade-offs

- Switching to PostgreSQL later should keep the same application-level contract.

## Notes

Read-your-own-writes inside the transaction so the returned `BankAccount`/`Transaction` matches the committed row.
