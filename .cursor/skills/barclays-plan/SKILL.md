---
name: barclays-plan
description: Convert the Barclays exercise requirements and OpenAPI contract into reviewable technical requirements. Requires barclays-init to be complete and must stop for explicit user approval.
---

# Barclays Plan

## Goal

Turn the supplied Barclays take-home requirements into a precise, testable technical requirements document before making architectural decisions.

## Gate

Read `.barclays/workflow-state.md`.

Proceed only when:
- `init = COMPLETE`; and
- `plan` is `NOT_STARTED`, `IN_PROGRESS`, or `AWAITING_APPROVAL`.

If init is not complete, stop and tell the user to run `/barclays-init`.

## Inputs

Read all relevant sources that are actually available:
- requirements supplied by the user in chat;
- provided OpenAPI specification;
- repository README or instructions;
- `lessons.md`;
- `adrs.md` if already populated;
- any existing `requirements/technical-requirements.md`.

Treat the OpenAPI contract as authoritative for API shape unless the exercise instructions explicitly say otherwise. Flag contradictions instead of silently choosing one.

## Process

1. Set plan status to `IN_PROGRESS`.
2. Extract the business capabilities and constraints.
3. Identify ambiguous or missing details that materially affect scope or acceptance criteria.
4. Discuss material ambiguities/trade-offs with the user in chat when needed. Do not over-question minor implementation details that belong in design.
5. Produce or update `requirements/technical-requirements.md`.
6. Make requirements independently testable where practical and give stable IDs, for example:
   - `REQ-USER-001`
   - `REQ-ACCOUNT-001`
   - `REQ-TXN-001`
   - `REQ-NFR-001`
7. Separate minimum submission scope from stretch scope.
8. Capture assumptions explicitly.
9. Define acceptance criteria and expected error behaviours based on the supplied contract.
10. Append a concise planning entry to `agent-interactions.md`.
11. Set plan status to `AWAITING_APPROVAL`.
12. Stop. Do not start design.

## Required document structure

`requirements/technical-requirements.md` should contain:

1. Purpose and context
2. Source material
3. In scope
4. Out of scope
5. Domain terminology
6. Functional requirements
   - User
   - Account
   - Transaction / deposit / withdrawal
7. API contract and validation requirements
8. Authentication/authorisation assumptions, if required by the exercise
9. Persistence and consistency requirements stated at requirement level
10. Error handling requirements
11. Non-functional requirements
12. Testing and quality requirements
13. Delivery/repository requirements
14. Minimum viable submission scope
15. Stretch scope
16. Assumptions and open questions
17. Acceptance criteria / traceability

## Important constraints

During planning:
- do not generate production implementation code;
- do not prematurely choose frameworks/patterns beyond constraints already supplied;
- do not mark design decisions as accepted ADRs;
- do not mark plan `APPROVED` yourself.

## Approval handling

If the user explicitly approves the plan, update:
- `plan = APPROVED`
- `current stage = design`

Append the explicit approval as a sanitised factual entry to `agent-interactions.md`.

Then tell the user `/barclays-design` is unlocked.
