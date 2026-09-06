# Barclays Workflow State

> This file is the source of truth for workflow gates. Do not mark a stage approved without explicit user approval in chat.

| Stage | Status |
|---|---|
| init | COMPLETE |
| plan | NOT_STARTED |
| design | NOT_STARTED |
| tasks | NOT_STARTED |
| implement-and-test | NOT_STARTED |

**Current stage:** plan

## Approval evidence

| Stage | Approval | Evidence summary |
|---|---|---|
| plan | Pending | |
| design | Pending | |
| tasks | Pending | |

## Notes

- Valid statuses: `NOT_STARTED`, `IN_PROGRESS`, `AWAITING_APPROVAL`, `APPROVED`, `COMPLETE`, `BLOCKED`.
- Only `plan`, `design`, and `tasks` require explicit user approval before the next stage.
