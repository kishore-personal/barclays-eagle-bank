---
name: barclays-implement-and-test
description: Implement the explicitly approved Barclays task list one task at a time, run tests/builds, record evidence, and update lessons. Requires init complete plus explicit plan, design, and task approvals.
---

# Barclays Implement and Test

## Goal

Implement the approved solution incrementally, keeping code, tests, documentation, task status, and audit history aligned with what was actually done.

## Hard gate

Read `.barclays/workflow-state.md` before changing production code.

Proceed only when all are true:
- `init = COMPLETE`
- `plan = APPROVED`
- `design = APPROVED`
- `tasks = APPROVED`

If any gate is missing, stop and report the missing stage. Do not workaround or infer approval.

## Required context

Before implementation read:
- `requirements/technical-requirements.md`
- `design/system-design.md`
- `tasks/tasks.md`
- `adrs.md`
- relevant accepted ADRs
- `lessons.md`
- existing code/tests
- OpenAPI specification where available

## Implementation loop

Set implement-and-test status to `IN_PROGRESS`, then repeat:

1. Select the next eligible `MUST`/`SHOULD` task whose dependencies are done.
2. Mark only that task `IN_PROGRESS`.
3. Inspect relevant existing code before editing.
4. Implement the smallest coherent change satisfying that task.
5. Add/update automated tests for the task.
6. Run the narrowest useful tests first.
7. Fix failures caused by the change.
8. Run broader regression tests/build as appropriate.
9. Verify API contract behaviour against the approved requirements/OpenAPI.
10. Mark the task `DONE` only if its definition of done is actually met.
11. Record commands/results factually in the task notes or audit entry.
12. Append a concise entry to `agent-interactions.md`.
13. Add a reusable lesson to `lessons.md` if one was genuinely learned.
14. If implementation reveals a material design problem, do not silently rewrite architecture. Record the issue, propose an ADR/design amendment, and obtain user approval if it changes an approved decision.
15. Continue to the next approved task.

## Testing integrity

Never write "tests pass", "build passes", "OpenAPI validated", or similar unless the corresponding command/check actually ran successfully.

When a command cannot be run because of environment limitations:
- say exactly what could not be verified;
- do not mark verification complete;
- give the user the exact command to run if appropriate.

## Code quality expectations

Keep the solution proportionate to a take-home test:
- clear naming and small cohesive units;
- explicit domain invariants;
- async APIs where appropriate;
- cancellation tokens where useful;
- decimal or approved money representation;
- safe persistence/concurrency behaviour per ADRs;
- useful validation and problem responses;
- deterministic tests;
- no unnecessary framework complexity;
- no dead code or speculative abstractions.

## Scope control

Implement only the approved task list unless:
- a tiny supporting change is necessary to complete an approved task; or
- the user explicitly expands scope.

Record scope changes in the audit trail and update requirements/design/tasks if necessary before proceeding.

## Completion

When all approved `MUST` tasks are `DONE` and their required verification has passed:
- run the broadest reasonable build/test suite;
- review requirement-to-task coverage;
- review OpenAPI compliance for implemented scope;
- update README/run instructions if part of approved tasks;
- set `implement-and-test = COMPLETE` only if the approved implementation scope is complete;
- append a final factual audit summary.

Report:
- completed tasks;
- tests/builds actually run and their results;
- remaining `SHOULD`/`COULD` tasks;
- known limitations;
- any unverified items.
