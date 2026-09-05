# Agent Interaction Audit Trail

This is a concise, factual engineering audit trail for meaningful AI-assisted actions and user decisions.

Because the repository may be public, do **not** store secrets, credentials, personal data, or an unnecessary verbatim transcript of chat.

## Entry template

### YYYY-MM-DD HH:MM — <skill or activity>

- **User intent:** <sanitised summary>
- **Agent actions actually performed:**
  - <action>
- **Decisions / outcomes:**
  - <decision or none>
- **Files changed:**
  - `<path>`
- **Verification actually run:** <commands/results or "None">
- **Workflow transition:** <from -> to>
- **Approval state:** <not required / awaiting / explicitly approved>

---

### 2026-09-05 11:55 — barclays-init

- **User intent:** Initialise the gated Barclays take-home workflow (audit trail, lessons, ADR index, and stage state) without implementing application features.
- **Agent actions actually performed:**
  - Confirmed Git was already initialised on branch `feature/eagle-bank-user`.
  - Created missing workflow directories: `.barclays/`, `requirements/`, `design/`, `tasks/`, `docs/adr/`.
  - Created workflow documents from `.cursor/templates`: `.barclays/workflow-state.md`, `agent-interactions.md`, `lessons.md`, `adrs.md`.
  - Created `.gitignore` (none existed) with local/generated/secret patterns only.
  - Did not create application architecture, controllers, entities, database code, tests, or feature implementation.
- **Decisions / outcomes:**
  - Init marked `COMPLETE`; current stage set to `plan`.
  - Remaining stages left `NOT_STARTED`; no plan/design/tasks approvals recorded.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
  - `lessons.md`
  - `adrs.md`
  - `.gitignore`
  - `requirements/.gitkeep`
  - `design/.gitkeep`
  - `tasks/.gitkeep`
  - `docs/adr/.gitkeep`
- **Verification actually run:** Inspected repository root, Git status, and absence of `.barclays/` and `.gitignore` before creating files.
- **Workflow transition:** uninitialised -> init `COMPLETE`; current stage `plan`
- **Approval state:** not required for init

---

### 2026-09-05 12:01 — barclays-plan

- **User intent:** Produce technical requirements for the Eagle Bank REST API from the exercise scenarios and the attached OpenAPI specification.
- **Agent actions actually performed:**
  - Confirmed init was `COMPLETE` and plan was `NOT_STARTED`.
  - Read the OpenAPI file attached in chat and copied it to `openapi.yaml`.
  - Compared scenario prose with the contract, recorded contradictions (accountId vs accountNumber; create-user Given clause; transaction id regex vs example; missing login path).
  - Wrote `requirements/technical-requirements.md` with stable requirement IDs, MVP vs stretch, assumptions, and open questions.
  - Recorded LESSON-001 in `lessons.md`.
  - Did not generate production implementation code, choose a framework, or start design.
- **Decisions / outcomes:**
  - OpenAPI treated as authoritative for API shape; scenarios treated as authoritative for ownership and error outcomes.
  - Proposed (pending user approval): email/password signup, `POST /v1/auth/login`, and treating `^tan-[A-Za-z0-9]$` as a spec defect.
  - Plan set to `AWAITING_APPROVAL`; design not started.
- **Files changed:**
  - `openapi.yaml`
  - `requirements/technical-requirements.md`
  - `.barclays/workflow-state.md`
  - `lessons.md`
  - `agent-interactions.md`
- **Verification actually run:** Read attached OpenAPI and repository workflow state. No tests or builds.
- **Workflow transition:** plan `NOT_STARTED` -> plan `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the technical requirements

---

### 2026-09-05 12:10 — barclays-plan (Q-007)

- **User intent:** Specify preferred implementation language as .NET 10.
- **Agent actions actually performed:**
  - Updated `REQ-NFR-001`, `A-008`, and `Q-007` in `requirements/technical-requirements.md`.
  - Recorded the answer in workflow notes.
  - Did not treat this message as plan approval.
  - Did not start design or implementation.
- **Decisions / outcomes:**
  - Language constraint set to .NET 10.
  - Q-001–Q-006 remain proposed defaults pending plan approval.
  - Plan remains `AWAITING_APPROVAL`.
- **Files changed:**
  - `requirements/technical-requirements.md`
  - `.barclays/workflow-state.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; plan still `AWAITING_APPROVAL`
- **Approval state:** Q-007 answered; full plan not yet explicitly approved

---

### 2026-09-05 12:13 — barclays-plan (open questions vs design gate)

- **User intent:** Ask whether pending open questions must be cleared with a BA/PO before design can start.
- **Agent actions actually performed:**
  - Classified Q-001–Q-006 as non-blocking if the documented defaults are accepted.
  - Confirmed design remains gated on explicit plan approval, not on external BA answers.
  - Did not mark the plan approved and did not start design.
- **Decisions / outcomes:**
  - Advice: treat the brief and OpenAPI as the product owner for this take-home; proceed on documented assumptions.
  - Q-001 and Q-005 are the only items worth a later BA conversation; they still have workable defaults.
- **Files changed:**
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** none; plan still `AWAITING_APPROVAL`
- **Approval state:** awaiting explicit user approval of the technical requirements

---

### 2026-09-05 12:14 — barclays-plan (explicit approval)

- **User intent:** Explicitly approve the technical requirements.
- **Agent actions actually performed:**
  - Marked plan `APPROVED` and set current stage to `design`.
  - Recorded Q-001–Q-006 as accepted defaults and Q-007 as .NET 10.
  - Did not start design or implementation.
- **Decisions / outcomes:**
  - `requirements/technical-requirements.md` is the approved plan, including documented defaults.
  - `/barclays-design` is unlocked.
- **Files changed:**
  - `.barclays/workflow-state.md`
  - `requirements/technical-requirements.md`
  - `agent-interactions.md`
- **Verification actually run:** None
- **Workflow transition:** plan `AWAITING_APPROVAL` -> plan `APPROVED`; current stage `design`
- **Approval state:** plan explicitly approved by the user

---
