import { apiFetch } from "../../lib/api";
import type {
    TimeOffOverview,
    TimeOffPreview,
    TimeOffPreviewInput,
    TimeOffRequest,
    TimeOffRequestInput,
} from "./types";

export function getOverview(): Promise<TimeOffOverview> {
    return apiFetch<TimeOffOverview>("/TimeOff/overview");
}

export function getMyRequests(): Promise<TimeOffRequest[]> {
    return apiFetch<TimeOffRequest[]>("/TimeOff/requests");
}

export function previewRequest(input: TimeOffPreviewInput): Promise<TimeOffPreview> {
    return apiFetch<TimeOffPreview>("/TimeOff/requests/preview", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function createTimeOffRequest(input: TimeOffRequestInput): Promise<number> {
    return apiFetch<number>("/TimeOff/requests", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function withdrawRequest(id: number): Promise<void> {
    return apiFetch<void>("/TimeOff/requests/withdraw", {
        method: "POST",
        body: JSON.stringify({ id }),
    });
}