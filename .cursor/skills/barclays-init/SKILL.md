---
name: barclays-init
description: Initialise the Barclays take-home repository workflow, audit trail, lessons log, ADR index, and workflow state. This must be the first Barclays skill used.
---

# Barclays Init

Use this skill first for the Barclays take-home repository.

## Goal

Prepare the repository for a traceable, gated engineering workflow without implementing application features.

## Preconditions

None.

If `.barclays/workflow-state.md` already exists, inspect it before changing anything. Do not destroy existing history. Make this skill idempotent.

## Actions

1. Inspect the repository root and determine whether Git is already initialised.
2. If Git is not initialised, initialise it with `git init`.
3. Create these directories if missing:
   - `.barclays/`
   - `requirements/`
   - `design/`
   - `tasks/`
   - `docs/adr/`
4. Create these files if missing, using the matching templates in `/templates` when available:
   - `.barclays/workflow-state.md`
   - `agent-interactions.md`
   - `lessons.md`
   - `adrs.md`
5. Ensure the state file records:
   - init: `COMPLETE`
   - plan: `NOT_STARTED`
   - design: `NOT_STARTED`
   - tasks: `NOT_STARTED`
   - implement-and-test: `NOT_STARTED`
   - current stage: `plan`
6. Append a factual init entry to `agent-interactions.md` describing what was created or already present.
7. Inspect `.gitignore`. Add only obvious local/generated entries if useful, but do not blindly replace the user's file.
8. Do not create application architecture, controllers, entities, database code, tests, or feature implementation.

## Audit entry requirements

Record:
- skill name;
- user intent summary;
- actions actually performed;
- files created/changed;
- decisions made;
- stage transition.

Do not store secrets or copy the entire chat into a public repository.

## Completion response

Summarise what was initialised, show the current workflow state, and instruct the user to run `/barclays-plan` with the exercise requirements/OpenAPI available in the chat or repository.
