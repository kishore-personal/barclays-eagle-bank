# Review Severity Guide

## CRITICAL

Use when the implementation has a fundamental flaw that makes the submission unsafe, invalid, or materially broken.

Examples:
- secrets committed to the repository
- core API cannot build or start
- transaction logic can corrupt balances
- severe authorization/ownership bypass
- implementation fundamentally violates the required API contract

## HIGH

Use for serious correctness, security, or requirement failures that should be fixed before submission.

Examples:
- required create/fetch endpoint missing
- withdrawals allow invalid balances
- user can access another user's account
- transaction creation and balance update are non-atomic in a way that risks inconsistency
- OpenAPI response/status mismatch on core flows

## MEDIUM

Use for meaningful maintainability, test, validation, or design problems that do not invalidate the whole submission.

Examples:
- important negative-path tests missing
- inconsistent validation
- design/ADR deviation without documentation
- poor exception mapping
- unnecessary coupling likely to make changes harder

## LOW

Use for small quality improvements that are worth addressing but have limited risk.

Examples:
- naming inconsistency
- minor duplication
- weak README wording
- small test readability issue

## OBSERVATION

Use for non-blocking commentary, trade-offs, or future improvements.

Examples:
- reasonable alternative design exists
- production system might need stronger observability
- a simplification could be considered later

## Outcome guidance

### PASS
No CRITICAL/HIGH/MEDIUM issues and only trivial LOW/OBSERVATION items.

### PASS_WITH_OBSERVATIONS
No blocking issues, but there are LOW findings or meaningful observations worth noting.

### CHANGES_REQUESTED
Any CRITICAL or HIGH issue, or a set of MEDIUM issues that materially reduce confidence in the submission.
