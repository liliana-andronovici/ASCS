# Home dashboard backend integration

## Connected data

- Current employee: existing `GET /api/employees/{id}` using the employee ID from the current session.
- Dashboard directory: existing `GET /api/employees?page=...&pageSize=100&sortBy=Id&sortDirection=Ascending`.
- Every directory page is loaded and validated. Partial, duplicate, inconsistent, or malformed results display an error instead of misleading totals. Employee mutations invalidate the `employees` query prefix, including Home.
- Headcount counts active employees whose start date is today or earlier. The monthly join count counts those who started this month through today. Starting this month includes active employees with a start date anywhere in the current month, including upcoming starters.
- Coming up derives scheduled start dates and work anniversaries from active employees in the complete directory. It is not a general events endpoint.

Dates are displayed in the browser's local timezone. Calendar arithmetic preserves midnight across daylight-saving changes. The dashboard clock updates every 30 seconds.

## Unavailable data

The locally inspected backend does not expose dashboard meetings or time-off endpoints. Home does not call guessed routes by default and does not replace missing data with fake meetings or zero absences.

Optional feeds can be enabled only after confirming a backend contract. Set these paths relative to `VITE_API_URL`, then restart/rebuild Vite:

```dotenv
# VITE_HOME_MEETINGS_API_PATH=<confirmed meetings route>
# VITE_HOME_LEAVE_API_PATH=<confirmed time-off route>
```

Adapters and proposed JSON validation are isolated in `api.ts` and `contracts.ts`. Do not set these variables merely to make the unavailable labels disappear. Match the server's actual paths, filters, enum values, response envelopes, and permission scope first.

Both optional adapters currently send `from` and `to` ISO instants with an exclusive `to`. Meetings cover Monday through Friday (end Saturday 00:00); absences cover the full week including weekends (end next Monday 00:00).

Proposed meetings response: an array of `{ id, title, startUtc, endUtc, location }`, with ISO timestamps containing a timezone, and location a string or null. Return entries overlapping the requested interval. If this is meant to show only the signed-in user's meetings, enforce that scope in the backend; company-wide room reservations are not automatically personal meetings.

Proposed leave response: an array of `{ id, employeeId, startDateUtc, endDateUtc, type, status }`. Current adapter expects types Vacation/Sick/Remote and statuses Pending/Approved/Declined, with an inclusive final leave date. Approved entries are counted once per employee even if requests overlap. These values are not a claim about the unmerged time-off API; adapt them once that contract is confirmed.

## Verification

`npm run test:home` verifies directory pagination and schema failures, statistics, duplicate absences, weekend absences, meetings crossing midnight, Friday/weekend bounds, and daylight-saving calendar arithmetic. Browser checks use an external local test server, not the real database. A live end-to-end check is still required when the .NET backend is running.
