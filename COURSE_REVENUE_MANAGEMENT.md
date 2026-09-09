# Course assignments and revenue management

## Data model

- `user_course_assignments` connects an internal `users` record to a `courses`
  record and a `COURSE_ASSIGNMENT_TYPE` lookup. A filtered unique index prevents
  duplicate active user/course/type assignments while preserving soft-deleted
  history.
- `course_revenue_shares` stores a course-specific percentage or fixed-amount
  rule. The beneficiary is nullable for the Platform share.
- `course_revenue_distributions` is the immutable financial snapshot created
  when a payment is authorised. It stores the applied rule and calculated amount
  so later configuration changes cannot alter history.
- `revenue_settlements` groups pending beneficiary distributions for payout.
  Customer payment and beneficiary payout remain separate states.

The migration is `20260907204132_AddCourseAssignmentsAndRevenueSharing`.

## Assignment types

The `COURSE_ASSIGNMENT_TYPE` lookup contains Owner, Trainer, Assistant Trainer,
Marketing, Sales, Operations, Other, and Platform. The same lookup is used for
assignment and revenue share types so APIs do not rely on hard-coded role names.

## APIs

Internal-user authentication is required. Management operations and settlement
operations additionally require the existing `Admin` JWT role.

- `GET|POST /api/user-course-assignments`
- `PUT|DELETE /api/user-course-assignments/{id}`
- `GET /api/users/{userId}/courses`
- `GET /api/me/courses`
- `GET /api/me/dashboard`
- `GET /api/me/revenue?courseId=&dateFrom=&dateTo=&status=`
- `GET|POST /api/courses/{courseId}/revenue-shares`
- `PUT|DELETE /api/courses/{courseId}/revenue-shares/{id}`
- `GET /api/courses/{courseId}/revenue-summary`
- `GET|POST /api/revenue-settlements`
- `PUT /api/revenue-settlements/{id}/status`

`/api/me/*` always derives the internal user ID from the authenticated JWT and
does not accept a client-provided user ID. Non-admin internal users can only read
revenue information for courses assigned to them. Admin dashboard queries retain
global access.

## Payment trigger and idempotency

Revenue distribution runs in `InvoicePaymentProcessor.ProcessAsync`, after Telr
authorization is verified and within the same serializable transaction that
marks the payment and invoice paid. It is not created during checkout.

For each invoice item, the processor selects active revenue rules whose effective
date contains the payment time. A percentage share is calculated from the final
invoice line amount (`LineTotal`); a fixed share applies its configured amount to
the invoice line. Values are rounded to two decimal places.

Idempotency has two layers:

1. An already-authorised and paid transaction exits before fulfillment runs.
2. A unique index on `(PaymentTransactionId, InvoiceItemId, RevenueShareId)`
   prevents duplicate snapshots during concurrent callback processing.

## Validation

Application and database validation enforce non-negative values, percentage
values no greater than 100, valid effective dates, and duplicate prevention.
The service also prevents the sum of active percentage shares for a course from
exceeding 100%. Partial plans below 100% are permitted so administrators can
build a plan incrementally; activating a distinct “complete plan” is not modeled
in the existing system.

## Settlements

Creating a settlement claims all unclaimed pending distributions for one
beneficiary and period under a serializable transaction. Supported transitions
are `Draft -> Approved -> Paid`, with cancellation from Draft or Approved. Only
the Paid transition changes distributions from Pending to Paid. Cancelling
releases the distributions for a future settlement.

## Seeded permission links

- `Courses.AssignUsers`
- `Courses.ViewAssignedUsers`
- `RevenueShares.View`
- `RevenueShares.Manage`
- `RevenueDistributions.View`
- `RevenueSettlements.View`
- `RevenueSettlements.Manage`
- `RevenueDashboard.View`

The existing application does not currently have an authorization handler that
evaluates `rolelinks` per request. The new endpoints therefore follow the current
JWT behavior: authenticated internal users for scoped reads and the existing
`Admin` role for mutations/global operations. The permission links are available
for the existing role-management UI and for a future handler without replacing
the current authorization model.
