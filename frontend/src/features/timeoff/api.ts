import { apiFetch } from "../../lib/api";
import type { TimeOffRequest, TimeOffRequestInput } from "./types";

export function getMyRequests(): Promise<TimeOffRequest[]> {
    return apiFetch<TimeOffRequest[]>("/LeaveRequest");
}

export function createTimeOffRequest(input: TimeOffRequestInput): Promise<number> {
    return apiFetch<number>("/LeaveRequest", {
        method: "POST",
        body: JSON.stringify({
            type: input.type,
            startDateUtc: `${input.startDate}T00:00:00.000Z`,
            endDateUtc: `${input.endDate}T00:00:00.000Z`,
            note: input.note,
        }),
    });
}

export function deleteTimeOffRequest(id: number): Promise<void> {
    return apiFetch<void>(`/LeaveRequest/${id}`, { method: "DELETE" });
}
