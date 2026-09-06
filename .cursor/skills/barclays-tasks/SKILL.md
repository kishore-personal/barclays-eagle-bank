---
name: barclays-tasks
description: Decompose the approved Barclays requirements and design into ordered, testable implementation tasks. Requires explicit design approval and must stop for task approval.
---

# Barclays Tasks

## Goal

Create a sequenced implementation plan made of small, independently verifiable tasks.

## Gate

Proceed only when `.barclays/workflow-state.md` shows:
- `init = COMPLETE`
- `plan = APPROVED`
- `design = APPROVED`

Otherwise stop and identify the missing approval.

## Inputs

Read:
- `requirements/technical-requirements.md`
- `design/system-design.md`
- `adrs.md`
- accepted ADRs in `docs/adr/`
- `lessons.md`
- existing `tasks/tasks.md`, if any

## Process

1. Set tasks status to `IN_PROGRESS`.
2. Create or update `tasks/tasks.md`.
3. Decompose the design into ordered tasks with stable IDs, e.g. `TASK-001`.
4. Prefer vertical, demonstrable increments where sensible.
5. Include test work in each feature task rather than leaving all testing until the end.
6. Mark dependencies explicitly.
7. Map each task to requirement IDs and relevant ADRs.
8. Add verification commands/criteria where known.
9. Identify which tasks form the minimum viable submission.
10. Do not implement tasks.
11. Append a concise task-planning entry to `agent-interactions.md`.
12. Set tasks status to `AWAITING_APPROVAL`.
13. Stop for user review.

## Each task must contain

- ID and title
- Objective
- Requirement references
- Design/ADR references
- Dependencies
- Implementation scope
- Test scope
- Verification / definition of done
- Priority: `MUST`, `SHOULD`, or `COULD`
- Status: initially `TODO`

## Recommended ordering pattern

Adapt to the approved design, but typically consider:

1. Solution/bootstrap and baseline build
2. Shared API/error/validation foundations
3. Persistence/migrations
4. User create/fetch vertical slice
5. Account create/fetch vertical slice
6. Deposit transaction vertical slice
7. Withdrawal transaction vertical slice
8. Transaction fetch/list behaviour
9. Remaining update/delete endpoints required by scope
10. Contract/integration tests
11. Documentation/readme/run instructions
12. Final quality pass

Do not blindly create tasks for endpoints that are explicitly out of approved scope.

## Approval handling

When the user explicitly approves `tasks/tasks.md`:
- set `tasks = APPROVED`;
- set `current stage = implement-and-test`;
- append the approval to `agent-interactions.md`.

Then tell the user `/barclays-implement-and-test` is unlocked.
