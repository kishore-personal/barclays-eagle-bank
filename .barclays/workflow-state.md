# Barclays Workflow State

> This file is the source of truth for workflow gates. Do not mark a stage approved without explicit user approval in chat.

| Stage | Status |
|---|---|
| init | COMPLETE |
| plan | APPROVED |
| design | NOT_STARTED |
| tasks | NOT_STARTED |
| implement-and-test | NOT_STARTED |

**Current stage:** design

## Approval evidence

| Stage | Approval | Evidence summary |
|---|---|---|
| plan | Explicitly approved | User approved the technical requirements in chat on 2026-09-05, including Q-007 (.NET 10) and the documented defaults for Q-001–Q-006. |
| design | Pending | |
| tasks | Pending | |

## Notes

- Valid statuses: `NOT_STARTED`, `IN_PROGRESS`, `AWAITING_APPROVAL`, `APPROVED`, `COMPLETE`, `BLOCKED`.
- Only `plan`, `design`, and `tasks` require explicit user approval before the next stage.
