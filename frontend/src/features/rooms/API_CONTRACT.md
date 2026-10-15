# Room booking frontend integration

The routes and JSON shapes below are a **proposed contract**, not endpoints confirmed to exist. They are based on the RoomEntity and RoomBookingEntity domain models. Confirm the contract with the backend developer before enabling this feature in production.

The page uses the shared API client and current authenticated session. It does not load sample data or save bookings only in memory. Missing endpoints display an unavailable state.

## Configuration

`VITE_API_URL` configures the shared API base (default `/api`). Optional path overrides:

```dotenv
VITE_ROOMS_API_PATH=/Rooms
VITE_ROOM_BOOKINGS_API_PATH=/RoomBookings
```

These are paths relative to the API base. Update the path variables and the small adapter in `api.ts` if the final routes differ. Vite environment settings take effect when the dev server restarts or the app is rebuilt.

## List rooms

`GET /api/Rooms` returns a JSON array:

```json
[{ "id": 1, "name": "Meeting room", "floor": 2, "seats": 8, "amenities": "Screen", "locationId": 1, "locationName": "Head office" }]
```

`locationName` is optional. The UI groups offices by `locationId`; missing names display `Location #<id>`. Numeric IDs must be positive. Amenities may be null or omitted. Responses are validated with Zod; a paginated response will require an adapter change.

## List bookings

`GET /api/RoomBookings?fromUtc=<ISO instant>&toUtc=<ISO instant>` returns a JSON array:

```json
[{ "id": 1, "roomId": 1, "organizerEmployeeId": 7, "organizerName": "Example Organizer", "title": "Planning", "startDateTimeUtc": "2026-10-19T09:00:00Z", "durationMinutes": 60 }]
```

Return every reservation **overlapping** `[fromUtc, toUtc)`, including a reservation that starts before `fromUtc` and ends inside the interval. `organizerName` is optional; missing names display `Employee #<id>`. UTC timestamps must include `Z` or an explicit offset. The calendar displays times in the browser's local timezone, Monday–Friday, 08:00–19:00. Office timezone metadata is not available yet; confirm this display rule for multi-timezone offices.

## Create booking

`POST /api/RoomBookings` receives:

```json
{ "roomId": 1, "title": "Planning", "startDateTimeUtc": "2026-10-19T09:00:00Z", "durationMinutes": 60 }
```

The organizer must be derived from the authenticated user by the server, not accepted as a trusted client identity. The current form does not select participants. Creation may return an ID, a record, or an empty successful response; the frontend reloads the booking list after success. Do not return a successful status unless the reservation is saved.

The server must enforce room access, title and duration limits, valid dates, and overlap prevention atomically. The frontend checks the currently loaded schedule for convenience; it cannot prevent two users from choosing the same slot concurrently. Return `409` when a slot is taken. The frontend displays the conflict and refreshes the schedule. It does not automatically retry POST requests.

## Failure and empty states

- `404` / `405`: backend integration is unavailable, no sample-data fallback.
- Empty rooms array: no rooms configured, booking controls hidden.
- Empty booking array: empty calendar for the selected week.
- `400`, `401`, `403`, `409`, `429`, network and schema errors have distinct messages where applicable.
- Loading or failed schedules disable reservation submission; a failed POST keeps the draft.
- Failed booking list refresh after successful POST leaves the schedule unavailable with a Retry action; it does not create a local fake booking.

Cancellation, editing, participants, real-time updates, and office-specific timezones need their own backend contracts and UI work; this change prepares the current list/create page only.

Run the date-mapping and API-adapter tests with `npm run test:rooms` from `frontend`.
