# Barclays Code Review Checklist

Use this checklist as guidance. Not every item must produce a finding.

## 1. Requirements

- Are all implemented features traceable to approved requirements?
- Are any required operations missing?
- Is anything implemented that contradicts the approved scope?
- Are acceptance criteria demonstrably satisfied?

## 2. OpenAPI contract

- Do routes, verbs, request bodies, parameters, status codes, and response schemas match the specification?
- Are required fields and validation constraints respected?
- Are identifiers and resource relationships handled as specified?
- Are error responses consistent with the contract?

## 3. Architecture and ADRs

- Does the implementation match the approved design?
- Are ADR decisions followed?
- Have material architectural deviations been documented?
- Is layering clear without unnecessary abstraction?

## 4. Domain correctness

### Users
- User creation/fetch/update/delete behaviour is correct.
- Identity and ownership boundaries are respected.

### Accounts
- Accounts belong to the correct user.
- Account identifiers are handled consistently.
- Account state cannot be accessed or modified by another user where ownership rules apply.

### Transactions
- Deposits and withdrawals are immutable after creation where required.
- Amount validation is correct.
- Withdrawals cannot result in invalid balances if the requirements forbid it.
- Balance changes and transaction creation are consistent and atomic where needed.
- Transactions are associated with the correct account.

## 5. Monetary handling

- `decimal` or another appropriate fixed-precision representation is used for money.
- Floating-point arithmetic is not used for monetary values.
- Rounding behaviour is explicit if applicable.
- Negative/zero amount rules are enforced consistently.

## 6. Persistence

- Entity relationships are correct.
- Persistence concerns do not leak unnecessarily into API models.
- Database constraints support important invariants where appropriate.
- Multi-step balance/transaction updates use suitable transaction boundaries.
- Concurrency risks are considered.

## 7. Validation and errors

- Invalid input returns appropriate status codes.
- Missing resources are handled consistently.
- Validation is not duplicated unnecessarily.
- Internal exceptions are not leaked to clients.
- Error messages do not expose sensitive implementation details.

## 8. Security

- Ownership checks are enforced server-side.
- Sensitive information is not logged.
- Secrets are not committed to source control.
- Configuration is environment-appropriate.
- Input is treated as untrusted.

## 9. Code quality

- Naming is clear and consistent.
- Methods/classes have focused responsibilities.
- No obvious dead code or copy/paste duplication.
- Dependencies point in sensible directions.
- Async APIs are used correctly where applicable.
- Cancellation tokens are propagated where appropriate.
- No unnecessary patterns or abstractions are introduced merely for appearance.

## 10. Tests

- Core happy paths are tested.
- Important negative paths are tested.
- Ownership/authorization boundaries are tested where relevant.
- Deposit/withdrawal rules are tested.
- Tests are deterministic and independent.
- Tests assert meaningful behaviour rather than implementation detail.
- Integration tests cover contract/persistence behaviour where appropriate.

## 11. Build and repository quality

- Solution builds cleanly.
- Tests pass.
- Warnings are understood.
- README explains how to build/run/test the application.
- Repository contains no secrets, generated clutter, or irrelevant files.
- Git ignore rules are sensible.

## 12. Take-home quality

- The solution is understandable by another engineer.
- Trade-offs are documented.
- The solution demonstrates judgement rather than over-engineering.
- Known limitations are stated honestly.
