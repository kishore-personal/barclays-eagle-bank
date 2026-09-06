# Barclays Code Review Workflow

This workflow is mandatory for `/barclays-code-review`.

## Stage 0 — Verify review eligibility

Read `.barclays/workflow-state.md`.

Confirm:

- init = COMPLETE
- plan = APPROVED
- design = APPROVED
- tasks = APPROVED
- implement-and-test = COMPLETE

If any prerequisite is not satisfied:

1. Do not review.
2. Explain which prerequisite is missing.
3. Do not change workflow state.

---

## Stage 1 — Establish review baseline

Read the approved artefacts:

1. `requirements/technical-requirements.md`
2. `design/system-design.md`
3. `tasks/tasks.md`
4. `adrs.md`
5. `docs/adr/*.md`
6. OpenAPI specification
7. `lessons.md`

Build a mental baseline of what the implementation was supposed to achieve.

Do not begin by reviewing code in isolation.

---

## Stage 2 — Inspect implementation

Inspect:

- solution/project structure
- API endpoints
- controllers/endpoints
- application/domain services
- entities/models
- persistence
- validation
- error handling
- configuration
- tests
- README/documentation

Use `references/review-checklist.md`.

Trace implementation back to requirements, design, ADRs, and tasks.

---

## Stage 3 — Validate contract compliance

Compare implementation against the OpenAPI specification.

Check:

- route paths
- HTTP verbs
- path/query parameters
- request models
- response models
- required fields
- validation
- status codes
- error responses

Record mismatches as findings.

---

## Stage 4 — Review domain correctness

Pay particular attention to:

- user ownership
- account ownership
- deposit behaviour
- withdrawal behaviour
- insufficient funds handling
- transaction immutability
- money representation
- balance consistency
- transaction boundaries
- concurrency risks

Do not assume correctness from happy-path tests alone.

---

## Stage 5 — Review architecture

Compare the implementation with:

- `design/system-design.md`
- accepted ADRs

For every material deviation determine whether it is:

- justified and harmless
- undocumented
- risky
- contrary to an accepted decision

Record meaningful deviations.

---

## Stage 6 — Review tests

Inspect existing tests before running them.

Assess:

- happy paths
- negative paths
- validation
- ownership boundaries
- deposit/withdrawal rules
- persistence behaviour
- OpenAPI-facing behaviour
- independence/determinism

Identify meaningful missing tests.

---

## Stage 7 — Execute verification

Run appropriate repository commands when available.

For a typical .NET solution this may include:

```bash
dotnet restore
dotnet build
dotnet test
```

Use the actual repository setup rather than blindly assuming these commands.

Record:

- commands executed
- pass/fail status
- warnings/errors
- tests discovered
- tests passed/failed/skipped

Never invent execution results.

---

## Stage 8 — Classify findings

Use:

`references/severity-guide.md`

Allowed severities:

- CRITICAL
- HIGH
- MEDIUM
- LOW
- OBSERVATION

Each finding must contain:

1. title
2. evidence
3. issue
4. impact
5. recommendation

Avoid vague comments such as "improve code quality".

---

## Stage 9 — Produce review report

Create:

`reviews/code-review.md`

Use:

`references/review-output-template.md`

The report must include:

- executive summary
- commands executed
- findings
- requirements traceability
- architecture/ADR alignment
- test assessment
- submission readiness
- final outcome

---

## Stage 10 — Determine review outcome

Use one of:

### PASS

No material issues remain.

### PASS_WITH_OBSERVATIONS

Implementation is submission-ready but non-blocking improvements or observations exist.

### CHANGES_REQUESTED

Material correctness, contract, security, architecture, or test issues should be addressed before submission.

Do not use PASS when CRITICAL or HIGH findings exist.

---

## Stage 11 — Present findings to user

Summarise the review in chat.

Do not automatically fix the findings.

Ask the user which findings they want addressed, unless they have already instructed the agent to fix review findings.

---

## Stage 12 — Update repository memory

Append to:

`agent-interactions.md`

Include:

- skill invoked
- review scope
- commands run
- findings summary
- outcome

If a genuinely reusable lesson was discovered, update:

`lessons.md`

---

## Stage 13 — Update workflow state

Update `.barclays/workflow-state.md` with:

- code-review = COMPLETE
- review outcome
- date/time where appropriate

Do not alter prior approval states.

The completed flow should read conceptually:

```text
barclays-init
    ↓
barclays-plan
    ↓ APPROVED
barclays-design
    ↓ APPROVED
barclays-tasks
    ↓ APPROVED
barclays-implement-and-test
    ↓ COMPLETE
barclays-code-review
    ↓
PASS / PASS_WITH_OBSERVATIONS / CHANGES_REQUESTED
```
