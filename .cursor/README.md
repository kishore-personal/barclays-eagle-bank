# Barclays Cursor Skills — Gated Take-Home Workflow

This package adds a strict, reviewable Cursor workflow for the Barclays take-home exercise.

## Workflow

```text
/barclays-init
      ↓
/barclays-plan
      ↓
EXPLICIT USER APPROVAL
      ↓
/barclays-design
      ↓
EXPLICIT USER APPROVAL
      ↓
/barclays-tasks
      ↓
EXPLICIT USER APPROVAL
      ↓
/barclays-implement-and-test
```

The always-applied rule in `.cursor/rules/barclays-workflow.mdc` prevents later stages from running until the state file shows the required approvals.

## Included files

```text
.cursor/
├── rules/
│   └── barclays-workflow.mdc
└── skills/
    ├── barclays-init/
    │   └── SKILL.md
    ├── barclays-plan/
    │   └── SKILL.md
    ├── barclays-design/
    │   └── SKILL.md
    ├── barclays-tasks/
    │   └── SKILL.md
    └── barclays-implement-and-test/
        └── SKILL.md

templates/
├── workflow-state.md
├── agent-interactions.md
├── lessons.md
├── adrs.md
├── adr-template.md
├── technical-requirements-template.md
├── system-design-template.md
└── tasks-template.md
```

## Files created in your project by `/barclays-init`

```text
.barclays/workflow-state.md
agent-interactions.md
lessons.md
adrs.md
requirements/
design/
tasks/
docs/adr/
```

## Installation

Copy the contents of this package into the **root of the Barclays repository** so that `.cursor` is directly under the repo root.

Example:

```text
EagleBank/
├── .cursor/
├── templates/
├── README.md
└── ...
```

If your repository already has a README, do not overwrite it blindly. The templates directory can stay in the repo because the skills use it when initialising the workflow documents.

## Usage

### 1. Initialise

In Cursor Agent chat:

```text
/barclays-init
```

This sets up the workflow state, audit trail, lessons file, ADR index, and supporting directories.

### 2. Plan

Make the Barclays exercise requirements and OpenAPI spec available, then run:

```text
/barclays-plan
```

Review `requirements/technical-requirements.md`. Cursor must stop with `plan = AWAITING_APPROVAL`.

When satisfied, explicitly say something like:

```text
I approve the technical requirements. Mark the plan approved.
```

### 3. Design

```text
/barclays-design
```

Review `design/system-design.md` and the ADRs. Explicitly approve when satisfied.

### 4. Tasks

```text
/barclays-tasks
```

Review `tasks/tasks.md`. Explicitly approve when satisfied.

### 5. Implement and test

```text
/barclays-implement-and-test
```

The agent implements approved tasks incrementally, adds tests, runs verifications, and updates task/audit/lesson state factually.

## Audit trail philosophy

The audit trail is intended to show responsible use of an agent and preserve useful engineering context. It should not be used to pretend that AI-assisted work was human-only.

Because a take-home repository may be public, the workflow records **sanitised summaries**, not a full raw chat transcript. Do not commit credentials, tokens, personal data, or confidential Barclays material.

## Approval rules

Approval is intentionally strict:

- silence is not approval;
- the agent cannot approve its own output;
- moving to the next topic is not approval;
- an explicit statement from the user is required for plan, design, and tasks.

## Suggested first prompt

After installation:

```text
/barclays-init
```

Then provide the take-home requirements/OpenAPI and run:

```text
/barclays-plan
```
