import { ApiError } from "../../lib/api";
import { ZodError } from "zod";

export function roomErrorMessage(error: unknown): string {
    if (error instanceof ZodError) return "The server response does not match the room booking API contract.";
    if (error instanceof ApiError) {
        if (error.status === 404 || error.status === 405) return "Room booking is not available on this server yet.";
        if (error.status === 409) return "This room was just booked for that time. Refresh the schedule and choose another slot.";
        if (error.status === 403) return "You do not have permission to perform this action.";
        if (error.status === 401) return "Your session has expired. Please sign in again.";
        if (error.status === 429) return "Too many requests. Please wait and try again.";
        if (error.status === 400) return "The booking could not be accepted. Check the room, title, date, and duration.";
    }
    return "Unable to reach the room booking service. Please try again.";
}
