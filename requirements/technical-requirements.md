# Technical Requirements

## 1. Purpose and context

Build a REST API for a fictional bank, Eagle Bank, that lets a person register, authenticate, manage their own user profile, create and manage their own bank accounts, and deposit or withdraw money. Deposits and withdrawals are stored as transactions against an account. Transactions can be retrieved but must not be modified or deleted.

This is a take-home exercise. The hiring team will review the public repository and may later extend the solution in a pairing session. The work should therefore be correct for the minimum scope, easy to explain, and straightforward to extend.

The supplied OpenAPI document is authoritative for HTTP paths, methods, status codes, and schemas unless this document explicitly records a contradiction and a proposed resolution. Scenario prose that disagrees with the OpenAPI (for example `{accountId}` versus `{accountNumber}`) does not override the contract.

## 2. Source material

| Source | Location | Role |
|---|---|---|
| Exercise brief and Given/When/Then scenarios | User-supplied in chat, 2026-09-05 | Functional behaviour, authorisation outcomes, delivery constraints, minimum scope |
| OpenAPI 3.1.0 specification | `openapi.yaml` (copied from the file attached in chat) | Authoritative API shape |
| Repository README | `README.md` | No additional exercise instructions |

No prior `requirements/technical-requirements.md`, ADRs, or lessons existed when planning started.

## 3. In scope

- REST API under `/v1` matching the supplied OpenAPI for user, account, and transaction resources.
- One or more authentication endpoints that return a JWT, to be added to the submitted OpenAPI.
- Bearer JWT on every endpoint except `POST /v1/users` and the authentication endpoint(s).
- Ownership checks so a caller can only read or change their own user, accounts, and transactions.
- Validation and error responses for missing/invalid credentials and for the scenario status codes (`400`, `401`, `403`, `404`, `409`, `422`, `500`).
- Persistence of users, accounts, and transactions across requests for a running instance.
- Automated tests covering the minimum viable submission, plus a public GitHub repository, README, and an updated OpenAPI file.

## 4. Out of scope

- A customer-facing UI, mobile app, or banking portal.
- Transfers between accounts, interest, overdrafts, multi-currency, or account types other than `personal`.
- Transaction update or delete APIs.
- Real payment rails, KYC/AML, or production identity providers.
- Pagination, filtering, or sorting beyond what the OpenAPI defines (list endpoints return the caller's matching collection).
- Completing every endpoint is not required for submission; update/delete/list beyond the minimum are stretch unless time allows.

## 5. Domain terminology

| Term | Meaning |
|---|---|
| User | A registered person. Identifier format `usr-` followed by one or more alphanumeric characters. |
| Bank account | A personal GBP account owned by exactly one user. Identifier in the API is `accountNumber`, pattern `^01\d{6}$` (for example `01234567`). |
| Sort code | Fixed value `10-10-10` for all Eagle Bank accounts. |
| Transaction | An immutable deposit or withdrawal against one account. Identifier format in the spec is `tan-` plus alphanumeric characters (see Q-003). |
| Deposit | Transaction `type` `deposit`; increases account `balance`. |
| Withdrawal | Transaction `type` `withdrawal`; decreases account `balance` only when funds are sufficient. |
| Owner | The authenticated user whose `userId` is bound to the resource. |
| JWT bearer token | Access token sent as `Authorization: Bearer <token>` on protected endpoints. |

The brief's `{accountId}` and `{userId}` path names: user path parameter is `userId`; account path parameter is `accountNumber`.

## 6. Functional requirements

### User

| ID | Requirement |
|---|---|
| REQ-USER-001 | A caller can create a user with `POST /v1/users` and no authentication. On success the API returns `201` and a `UserResponse`. |
| REQ-USER-002 | Create-user required body fields are `name`, `address`, `phoneNumber`, and `email`. Address required fields are `line1`, `town`, `county`, and `postcode`. Optional address fields are `line2` and `line3`. |
| REQ-USER-003 | If required create-user data is missing or invalid, the API returns `400`. Create user remains public even though one scenario's Given clause says the user is authenticated. |
| REQ-USER-004 | Create-user must also accept a credential secret sufficient to authenticate later (see REQ-AUTH-002). That secret must not appear in `UserResponse`. |
| REQ-USER-005 | The API generates `id` matching `^usr-[A-Za-z0-9]+$`, and sets `createdTimestamp` and `updatedTimestamp` (date-time). |
| REQ-USER-006 | An authenticated owner can fetch their details with `GET /v1/users/{userId}` and receive `200` plus `UserResponse`. |
| REQ-USER-007 | An authenticated caller fetching a different existing `userId` receives `403` and an `ErrorResponse`. |
| REQ-USER-008 | An authenticated caller fetching a `userId` that does not exist receives `404` and an `ErrorResponse`. |
| REQ-USER-009 | An authenticated owner can partially update their details with `PATCH /v1/users/{userId}` using `UpdateUserRequest` (no fields are required). Success is `200` with the full updated `UserResponse`. `updatedTimestamp` changes; `id` and `createdTimestamp` do not. |
| REQ-USER-010 | PATCH of another existing user returns `403`. PATCH of a non-existent `userId` returns `404`. |
| REQ-USER-011 | An authenticated owner with no bank accounts can delete themselves with `DELETE /v1/users/{userId}` and receive `204`. |
| REQ-USER-012 | DELETE of the owner who still has one or more bank accounts returns `409` and an `ErrorResponse`. |
| REQ-USER-013 | DELETE of another existing user returns `403`. DELETE of a non-existent `userId` returns `404`. |
| REQ-USER-014 | Email is unique. A create or update that would duplicate another user's email is rejected (see Q-004). |
| REQ-USER-015 | `phoneNumber` must match E.164 as given in the spec (`^\+[1-9]\d{1,14}$`). `email` must be a valid email. Invalid values return `400`. |
| REQ-USER-016 | A `userId` that does not match `^usr-[A-Za-z0-9]+$` returns `400`. |

### Account

| ID | Requirement |
|---|---|
| REQ-ACCOUNT-001 | An authenticated user can create an account with `POST /v1/accounts`. Required fields: `name`, `accountType`. Success is `201` and `BankAccountResponse`. |
| REQ-ACCOUNT-002 | `accountType` must be `personal`. Other values return `400`. |
| REQ-ACCOUNT-003 | Missing or invalid create-account fields return `400`. |
| REQ-ACCOUNT-004 | The API assigns `accountNumber` matching `^01\d{6}$`, unique among accounts. `sortCode` is always `10-10-10`. `currency` is always `GBP`. Initial `balance` is `0.00`. |
| REQ-ACCOUNT-005 | The new account is owned by the authenticated user. Ownership is not supplied by the client. |
| REQ-ACCOUNT-006 | `GET /v1/accounts` returns `200` and `ListBankAccountsResponse` containing only the authenticated user's accounts. |
| REQ-ACCOUNT-007 | An authenticated owner can fetch an account with `GET /v1/accounts/{accountNumber}` and receive `200` plus `BankAccountResponse`. |
| REQ-ACCOUNT-008 | Fetch, update, delete, or transact against an existing account that belongs to another user returns `403`. |
| REQ-ACCOUNT-009 | Fetch, update, delete, or transact against an `accountNumber` that does not exist returns `404`. |
| REQ-ACCOUNT-010 | An authenticated owner can partially update `name` and/or `accountType` with `PATCH /v1/accounts/{accountNumber}`. Success is `200` with the full updated `BankAccountResponse`. Clients cannot change `accountNumber`, `sortCode`, `balance`, `currency`, or ownership via PATCH. |
| REQ-ACCOUNT-011 | An authenticated owner can delete their account with `DELETE /v1/accounts/{accountNumber}` and receive `204`. The OpenAPI defines no `409` for account delete (see Q-005). |
| REQ-ACCOUNT-012 | An `accountNumber` that does not match `^01\d{6}$` returns `400`. |
| REQ-ACCOUNT-013 | `balance` is a non-negative amount with up to two decimal places, stored consistently with transactions. Spec range is `0.00`–`10000.00` (see Q-006). |

### Transactions / deposits / withdrawals

| ID | Requirement |
|---|---|
| REQ-TXN-001 | An authenticated owner can create a transaction with `POST /v1/accounts/{accountNumber}/transactions`. Required fields: `amount`, `currency`, `type`. Optional: `reference`. Success is `201` and `TransactionResponse`. |
| REQ-TXN-002 | `type` is `deposit` or `withdrawal`. `currency` must be `GBP`. Other values return `400`. |
| REQ-TXN-003 | Missing or invalid create-transaction fields return `400`. |
| REQ-TXN-004 | `amount` must be greater than `0.00`, have at most two decimal places, and not exceed `10000.00`. Zero, negative, or over-precision amounts return `400`. |
| REQ-TXN-005 | A `deposit` is stored against the account and increases `balance` by `amount`. |
| REQ-TXN-006 | A `withdrawal` with sufficient funds is stored against the account and decreases `balance` by `amount`. |
| REQ-TXN-007 | A `withdrawal` with insufficient funds (`amount` greater than current `balance`) returns `422` and an `ErrorResponse`. No transaction is stored and `balance` is unchanged. |
| REQ-TXN-008 | Create/list/fetch of transactions on another user's existing account returns `403`. |
| REQ-TXN-009 | Create/list/fetch of transactions on a non-existent account returns `404`. |
| REQ-TXN-010 | `GET /v1/accounts/{accountNumber}/transactions` returns `200` and `ListTransactionsResponse` for the owner's account. |
| REQ-TXN-011 | `GET /v1/accounts/{accountNumber}/transactions/{transactionId}` returns `200` and `TransactionResponse` when the caller owns the account and the transaction belongs to that account. |
| REQ-TXN-012 | If the account exists and is owned by the caller but `transactionId` does not exist, return `404`. |
| REQ-TXN-013 | If the account exists and is owned by the caller but the transaction exists on a different account, return `404`. |
| REQ-TXN-014 | There are no APIs to update or delete transactions. Transactions are immutable after creation. |
| REQ-TXN-015 | `TransactionResponse` includes `id`, `amount`, `currency`, `type`, `createdTimestamp`, and when present `reference` and `userId` of the owning user. |
| REQ-TXN-016 | Creating a transaction and updating `balance` must be atomic. Concurrent withdrawals must not drive `balance` below `0.00`. |

## 7. API contract and validation requirements

| ID | Requirement |
|---|---|
| REQ-API-001 | Implement the paths, methods, success/error codes, and schemas in `openapi.yaml`. |
| REQ-API-002 | Request and response bodies use `application/json`. |
| REQ-API-003 | Create success codes: user `201`, account `201`, transaction `201`. Fetch/list/update success: `200`. Delete success: `204` with no body. |
| REQ-API-004 | `400` validation failures use `BadRequestErrorResponse`: `message` plus `details[]` each with `field`, `message`, and `type`. Exception: `POST /v1/users` `400` has no schema in the spec; still return a useful JSON error, preferably the same shape. |
| REQ-API-005 | `401`, `403`, `404`, `409`, `422`, and `500` use `ErrorResponse` with `message`. |
| REQ-API-006 | Copy-paste `403` descriptions in the spec that mention "transaction" on user/account operations do not change behaviour: `403` means the authenticated caller is not allowed to access that resource. |
| REQ-API-007 | Submit an updated OpenAPI file that keeps the original contract and adds the authentication operation(s) and any credential field(s) required by REQ-AUTH-002. |
| REQ-API-008 | Path parameter `accountNumber` is used everywhere the brief says `{accountId}`. |

## 8. Authentication and authorisation assumptions

| ID | Requirement |
|---|---|
| REQ-AUTH-001 | Implement at least one authentication endpoint that returns a JWT. The supplied OpenAPI has `bearerAuth` but no login path; the submitted spec must document the new operation. |
| REQ-AUTH-002 | Proposed contract (needs plan approval): `POST /v1/auth/login` with JSON `{ "email", "password" }` returns `200` and a token payload; invalid credentials return `401`; malformed body returns `400`. `CreateUserRequest` is extended with required `password` (write-only, not returned). See Q-001. |
| REQ-AUTH-003 | Protected endpoints require `Authorization: Bearer <jwt>`. Missing, malformed, expired, or otherwise invalid tokens return `401`. |
| REQ-AUTH-004 | `POST /v1/users` and the authentication endpoint(s) do not require a bearer token. |
| REQ-AUTH-005 | The JWT must identify the authenticated `userId` so ownership checks can be performed without trusting client-supplied owner IDs. |
| REQ-AUTH-006 | Authorisation is resource ownership: the caller may only fetch/update/delete their own user and only operate on accounts (and those accounts' transactions) they own. |
| REQ-AUTH-007 | When a resource exists but is not owned by the caller, return `403`, not `404`. When it does not exist, return `404`. |

## 9. Persistence and consistency requirements

| ID | Requirement |
|---|---|
| REQ-DATA-001 | Users, accounts, and transactions persist for the lifetime of the running application (or database, if one is used). In-memory-only storage is acceptable for the take-home if stated in the README, but data must survive across HTTP requests. |
| REQ-DATA-002 | Each account belongs to one user. Each transaction belongs to one account. |
| REQ-DATA-003 | `balance` is the running total of successful deposits minus successful withdrawals for that account, starting at `0.00`. It must not be writable except via successful transactions (and initial create). |
| REQ-DATA-004 | A failed request (validation, authn, authz, not found, conflict, insufficient funds) must not partially persist a user/account/transaction or change `balance`. |
| REQ-DATA-005 | Delete user is refused while any account still exists for that user (REQ-USER-012). |
| REQ-DATA-006 | After an account is deleted, it must not be fetchable (`404`). Transactions for that account must not remain visible through the API. |

## 10. Error handling requirements

| ID | Requirement |
|---|---|
| REQ-ERR-001 | Missing or invalid credentials on protected endpoints: `401`. |
| REQ-ERR-002 | Authenticated but not the owner of an existing resource: `403`. |
| REQ-ERR-003 | Resource does not exist (or transaction/account mismatch as in REQ-TXN-013): `404`. |
| REQ-ERR-004 | Missing/invalid request data or path format: `400`. |
| REQ-ERR-005 | Delete user while accounts exist: `409`. |
| REQ-ERR-006 | Withdrawal with insufficient funds: `422`. |
| REQ-ERR-007 | Unexpected failures: `500` with `ErrorResponse`. Do not leak stack traces or secrets. |
| REQ-ERR-008 | Error bodies must not include password values or raw tokens. |

## 11. Non-functional requirements

| ID | Requirement |
|---|---|
| REQ-NFR-001 | Implementation language is **.NET 10**, as specified by the user. The exercise allows Java, .NET, or JavaScript; this submission will use .NET 10. Host, libraries, and persistence within that platform remain design choices. |
| REQ-NFR-002 | The service must be runnable locally from the README with documented commands. |
| REQ-NFR-003 | Code structure should be explainable in a walkthrough and easy to extend in a pairing session (thin HTTP layer, testable domain rules). |
| REQ-NFR-004 | Monetary values must not accumulate binary floating-point error in balances (use a decimal-safe representation in implementation). |
| REQ-NFR-005 | Passwords must be stored hashed, never in plaintext. JWT signing secret must not be committed. |
| REQ-NFR-006 | Take-home scale: single service, no requirement for high availability, multi-region, or production throughput. |
| REQ-NFR-007 | Target timebox: complete enough to submit within 7 days unless the hiring team agrees otherwise. |

## 12. Testing and quality requirements

| ID | Requirement |
|---|---|
| REQ-TEST-001 | Automated tests must cover the minimum viable submission paths in section 14, including success and the corresponding `400`/`401`/`403`/`404` cases. |
| REQ-TEST-002 | If withdrawal is implemented, tests must cover sufficient funds, insufficient funds (`422`), and unchanged balance on failure. |
| REQ-TEST-003 | If delete user is implemented, tests must cover `204` with no accounts and `409` with an account. |
| REQ-TEST-004 | Tests must be runnable from the README. Do not claim tests passed unless they were executed. |
| REQ-TEST-005 | Manual or automated checks should show the implemented operations match the OpenAPI status codes and JSON shapes. |

## 13. Delivery/repository requirements

| ID | Requirement |
|---|---|
| REQ-DEL-001 | Submit by pushing to a public GitHub repository and sharing the link with the hiring team. |
| REQ-DEL-002 | The repository must include the updated OpenAPI spec, a README that explains how to run, test, and authenticate, and enough context to walk through trade-offs. |
| REQ-DEL-003 | Do not commit secrets, credentials, or personal data. |
| REQ-DEL-004 | AI assistance is allowed; do not present the work as human-only. Keep design trade-offs documented so they can be explained in interview. |

## 14. Minimum viable submission scope

The brief requires at least Create and Fetch for User, Account, and Transaction. Fetch of those resources requires authentication.

Must implement:

1. `POST /v1/users` (create user, including credential secret).
2. Authentication endpoint returning JWT (REQ-AUTH-001 / REQ-AUTH-002).
3. `GET /v1/users/{userId}` (own user; `401`/`403`/`404` as specified).
4. `POST /v1/accounts`.
5. `GET /v1/accounts/{accountNumber}` (own account; `401`/`403`/`404` as specified).
6. `POST /v1/accounts/{accountNumber}/transactions` for `deposit` (and `400`/`401`/`403`/`404`).
7. `GET /v1/accounts/{accountNumber}/transactions/{transactionId}`.
8. Updated OpenAPI, README, public repo, and automated tests for the above.

Withdrawal, list, PATCH, and DELETE may be omitted from the minimum submission but should be designed so they can be added quickly.

## 15. Stretch scope

In preferred order if time remains:

1. Withdrawal including `422` insufficient funds.
2. `GET /v1/accounts` and `GET /v1/accounts/{accountNumber}/transactions`.
3. PATCH user and PATCH account.
4. DELETE user (`204`/`409`/`403`/`404`) and DELETE account.
5. Concurrent withdrawal safety (REQ-TXN-016) with a test.
6. Docker/Compose for one-command review.
7. OpenAPI UI serving the submitted spec.

## 16. Assumptions and open questions

### Assumptions (used unless the user rejects them)

| ID | Assumption |
|---|---|
| A-001 | OpenAPI wins over scenario wording for paths, schemas, and status codes. |
| A-002 | Create user is unauthenticated. The scenario Given that says the caller is authenticated when required fields are missing is a brief error. |
| A-003 | PATCH is partial (OpenAPI: no required fields), despite scenario text saying "all the required data". |
| A-004 | Authentication is email + password at user creation, and `POST /v1/auth/login` returns a JWT (REQ-AUTH-002). |
| A-005 | Email uniqueness is required so login has a single user per email. |
| A-006 | New accounts start at balance `0.00`. |
| A-007 | Amounts are compared and stored to two decimal places. |
| A-008 | Implementation language is .NET 10 (user answer to Q-007). Framework and persistence choices within .NET 10 remain design decisions. |
| A-009 | Persistence technology is a design choice; REQ-DATA-001 is the requirement. |
| A-010 | Account delete is allowed even if the account has transactions or a non-zero balance, because neither the brief nor the OpenAPI defines `409` for account delete. Associated transactions become inaccessible. |

### Open questions (please confirm or correct)

| ID | Question | Proposed default |
|---|---|---|
| Q-001 | How should credentials be collected, given `CreateUserRequest` has no password and there is no login path? | **Accepted with plan approval:** add required `password` on create user and `POST /v1/auth/login`. |
| Q-002 | Login success body: raw `{ "token": "..." }` versus `{ "accessToken", "tokenType", "expiresIn" }`? | **Accepted with plan approval:** `{ "token": "<jwt>" }`. |
| Q-003 | Transaction id pattern is `^tan-[A-Za-z0-9]$` (one character) but the example is `tan-123abc`. | **Accepted with plan approval:** treat as a spec defect; use `^tan-[A-Za-z0-9]+$` in the submitted OpenAPI. |
| Q-004 | Duplicate email: `400` or `409`? Create user has no `409` in the spec. | **Accepted with plan approval:** `400` with a validation-style body. |
| Q-005 | Should deleting an account with remaining balance or transactions be refused? | **Accepted with plan approval:** no; allow delete (A-010). |
| Q-006 | If a deposit would take `balance` above `10000.00`, what happens? The spec has no dedicated code. | **Accepted with plan approval:** reject with `422` and do not persist the transaction. |
| Q-007 | Preferred implementation language among Java, .NET, and JavaScript? | **Answered:** .NET 10. |

### Spec defects to preserve in behaviour notes

- Several `403` descriptions incorrectly say "transaction" on user operations.
- `POST /v1/users` `400` omits a response schema.
- Some schemas use `format` where `pattern` is intended (`phoneNumber`, `accountNumber`, `userId`).

## 17. Acceptance criteria and traceability

A requirement is met when an automated or documented test shows the stated HTTP status and, where specified, JSON shape.

| Criterion | Requirements | Notes |
|---|---|---|
| AC-USER-CREATE | REQ-USER-001, REQ-USER-002, REQ-USER-003, REQ-USER-004, REQ-USER-005, REQ-API-003 | `201` with valid body; `400` if required data missing |
| AC-USER-FETCH | REQ-USER-006, REQ-USER-007, REQ-USER-008, REQ-AUTH-003, REQ-AUTH-007 | `200` / `403` / `404` / `401` |
| AC-USER-PATCH | REQ-USER-009, REQ-USER-010 | Stretch relative to MVP |
| AC-USER-DELETE | REQ-USER-011, REQ-USER-012, REQ-USER-013 | Stretch; `409` if accounts exist |
| AC-AUTH | REQ-AUTH-001, REQ-AUTH-002, REQ-AUTH-003, REQ-AUTH-004, REQ-AUTH-005 | Login + bearer on protected routes |
| AC-ACCOUNT-CREATE-FETCH | REQ-ACCOUNT-001–005, REQ-ACCOUNT-007–009 | MVP create + fetch |
| AC-ACCOUNT-LIST | REQ-ACCOUNT-006 | Stretch |
| AC-ACCOUNT-PATCH-DELETE | REQ-ACCOUNT-010, REQ-ACCOUNT-011 | Stretch |
| AC-TXN-DEPOSIT | REQ-TXN-001–005, REQ-TXN-008, REQ-TXN-009 | MVP transaction create |
| AC-TXN-WITHDRAW | REQ-TXN-006, REQ-TXN-007 | Stretch |
| AC-TXN-FETCH | REQ-TXN-011, REQ-TXN-012, REQ-TXN-013 | MVP fetch |
| AC-TXN-LIST | REQ-TXN-010 | Stretch |
| AC-TXN-IMMUTABLE | REQ-TXN-014 | No update/delete endpoints |
| AC-DELIVERY | REQ-DEL-001–004, REQ-API-007, REQ-TEST-001, REQ-NFR-001–002 | Public repo, spec, README, tests |

Planning does not choose frameworks, persistence engines, or package layout beyond the user-specified language (.NET 10). Those remaining choices belong in design after this document is explicitly approved.
