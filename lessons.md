# Engineering Lessons

Reusable lessons discovered while planning, designing, implementing, reviewing, or testing this repository.

Do not use this file as a diary. Capture lessons that could help another engineer or agent avoid repeating a mistake or understand a useful local convention.

## Entry template

### LESSON-001 — <short title>

- **Date:** YYYY-MM-DD
- **Context:** <what happened>
- **Lesson:** <reusable insight>
- **Action / convention:** <what future work should do>
- **Related files/tasks/ADRs:** <references>

---

### LESSON-001 — OpenAPI wins over scenario prose for API shape

- **Date:** 2026-09-05
- **Context:** The brief uses `/v1/accounts/{accountId}` while the attached OpenAPI uses `/v1/accounts/{accountNumber}` and pattern `^01\d{6}$`.
- **Lesson:** For this exercise, the OpenAPI document is authoritative for paths, methods, schemas, and status codes. Scenario text supplies behaviour (ownership, 403 vs 404, insufficient funds) but must not silently rename contract identifiers.
- **Action / convention:** When brief and spec disagree, record the contradiction, follow the spec for shape, and follow the brief for behaviour only where the spec is silent.
- **Related files/tasks/ADRs:** `openapi.yaml`, `requirements/technical-requirements.md` (A-001, REQ-API-008)

---
