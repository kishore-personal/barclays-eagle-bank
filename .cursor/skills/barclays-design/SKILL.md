---
name: barclays-design
description: Create and discuss the technical design from approved Barclays technical requirements, recording significant choices as ADRs. Requires explicit plan approval and must stop for explicit design approval.
---

# Barclays Design

## Goal

Create a pragmatic technical design that satisfies the approved requirements and can be explained confidently in a senior engineering interview.

## Gate

Read `.barclays/workflow-state.md`.

Proceed only when:
- `init = COMPLETE`; and
- `plan = APPROVED`.

If plan is not approved, stop. Do not design around the gate.

## Required inputs

Read:
- `requirements/technical-requirements.md`
- `lessons.md`
- `adrs.md`
- any ADRs in `docs/adr/`
- the OpenAPI specification when available
- existing repository structure, if any

## Process

1. Set design status to `IN_PROGRESS`.
2. Trace each important design element back to approved requirements.
3. Identify architecture choices that have meaningful trade-offs.
4. Discuss those choices with the user before treating them as accepted decisions.
5. Produce or update `design/system-design.md`.
6. Create ADRs under `docs/adr/` for significant decisions.
7. Update `adrs.md` as the ADR index.
8. Append a concise design interaction entry to `agent-interactions.md`.
9. Set design status to `AWAITING_APPROVAL`.
10. Stop. Do not create implementation tasks yet.

## Design topics to cover where applicable

- solution/project structure;
- API/controller boundary;
- application/use-case layer;
- domain model and invariants;
- DTOs and mapping;
- persistence strategy and schema shape;
- account balance and transaction consistency;
- concurrency strategy for simultaneous deposits/withdrawals;
- money representation and precision;
- validation strategy;
- API error model;
- authentication/authorisation approach if required;
- OpenAPI compliance strategy;
- logging/observability appropriate for a take-home test;
- test strategy and test pyramid;
- migrations/test database strategy;
- dependency injection;
- performance and security considerations proportionate to scope;
- deployment/run instructions if required.

## ADR guidance

Create an ADR only for a decision worth preserving, such as:
- architectural layering;
- persistence technology;
- money representation;
- transaction/balance consistency strategy;
- concurrency mechanism;
- authentication approach;
- test database choice.

Use sequential IDs: `ADR-0001`, `ADR-0002`, etc.

Each ADR should contain:
- Title
- Status
- Context
- Decision
- Alternatives considered
- Consequences
- Requirement references
- Date

Do not fabricate a user decision. If the user has not chosen between material alternatives, keep the issue proposed/open.

## Required design document structure

1. Design summary
2. Goals and non-goals
3. Requirement traceability
4. Proposed architecture
5. Component responsibilities
6. Domain model
7. API request flow
8. Persistence/data model
9. Transaction and concurrency behaviour
10. Validation and errors
11. Security
12. Observability
13. Testing strategy
14. OpenAPI compliance
15. Operational/run considerations
16. Risks and mitigations
17. ADR summary
18. Open design questions

## Constraints

- Do not implement production endpoints in this stage.
- Small pseudo-code or interface sketches are acceptable only when they clarify a design decision.
- Do not mark design approved yourself.

## Approval handling

When the user explicitly approves the design:
- set `design = APPROVED`;
- set `current stage = tasks`;
- append the approval to `agent-interactions.md`.

Then tell the user `/barclays-tasks` is unlocked.
