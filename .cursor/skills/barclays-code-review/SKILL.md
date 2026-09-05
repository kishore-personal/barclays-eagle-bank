---
name: barclays-code-review
description: Review the Barclays take-home implementation against approved requirements, design, ADRs, tasks, OpenAPI contract, tests, and engineering quality standards.
---

# Barclays Code Review

## Purpose

Perform an independent review of the completed implementation after `barclays-implement-and-test`.

This skill is a review stage, not an implementation stage.

Do not silently modify production code while performing the review. Record findings first and let the user decide which changes should be made.

## Preconditions

Before starting, verify that:

- `.barclays/workflow-state.md` exists.
- `plan` is APPROVED.
- `design` is APPROVED.
- `tasks` is APPROVED.
- `implement-and-test` is COMPLETE.
- Approved requirements and design artefacts are available.

If these conditions are not met, explain what is missing and stop.

## Inputs to read

Read, where present:

- `.barclays/workflow-state.md`
- `requirements/technical-requirements.md`
- `design/system-design.md`
- `tasks/tasks.md`
- `adrs.md`
- `docs/adr/*.md`
- `lessons.md`
- `agent-interactions.md`
- the OpenAPI specification
- source code
- tests
- configuration
- README/documentation

Also read the reference material in this skill:

- `references/review-checklist.md`
- `references/severity-guide.md`
- `references/review-output-template.md`

## Review objectives

Review the implementation for:

1. Requirement compliance
2. OpenAPI contract compliance
3. Architectural alignment
4. ADR compliance
5. Domain correctness
6. User/account ownership rules
7. Transaction and balance correctness
8. Validation and error handling
9. Security and privacy considerations
10. Persistence and transaction boundaries
11. Concurrency and consistency risks
12. Code readability and maintainability
13. Test quality and coverage
14. Build/test health
15. Documentation quality
16. Unnecessary complexity or over-engineering
17. Incomplete tasks or misleading completion claims

## Review behaviour

- Prefer evidence over assumptions.
- Reference concrete files, symbols, endpoints, tests, or requirements in findings.
- Do not invent test results.
- Run available build and test commands when practical.
- If tests cannot be run, say so explicitly.
- Do not mark a requirement compliant unless there is evidence.
- Do not rewrite implementation code during the review unless the user explicitly asks for fixes after reviewing findings.

## Severity

Use the severity definitions in `references/severity-guide.md`.

Allowed severities:

- CRITICAL
- HIGH
- MEDIUM
- LOW
- OBSERVATION

## Output

Create or update:

`reviews/code-review.md`

Use `references/review-output-template.md`.

The review outcome must be one of:

- PASS
- PASS_WITH_OBSERVATIONS
- CHANGES_REQUESTED

## Workflow state

After completing the review, update `.barclays/workflow-state.md` with the review stage and outcome.

Do not change implementation approval states retrospectively.

## Audit trail

Append a concise entry to `agent-interactions.md` describing:

- review scope
- artefacts inspected
- commands/tests run
- key findings
- final review outcome

Do not fabricate interaction history or approval.

## Lessons

If the review exposes a reusable engineering lesson, append it to `lessons.md`.

Only add lessons that would genuinely help a future developer or agent.
