import { apiFetch } from "../../lib/api";
import { mockApi } from "./mock";
import type {
    TimeOffOverview,
    TimeOffPreview,
    TimeOffPreviewInput,
    TimeOffRequest,
    TimeOffRequestInput,
} from "./types";

const USE_MOCK = import.meta.env.VITE_USE_MOCK === "true";

export function getOverview(): Promise<TimeOffOverview> {
    if (USE_MOCK) return mockApi.getOverview();
    return apiFetch<TimeOffOverview>("/TimeOff/overview");
}

export function getMyRequests(): Promise<TimeOffRequest[]> {
    if (USE_MOCK) return mockApi.getMyRequests();
    return apiFetch<TimeOffRequest[]>("/TimeOff/requests");
}

export function previewRequest(input: TimeOffPreviewInput): Promise<TimeOffPreview> {
    if (USE_MOCK) return mockApi.previewRequest(input);
    return apiFetch<TimeOffPreview>("/TimeOff/requests/preview", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function createTimeOffRequest(input: TimeOffRequestInput): Promise<number> {
    if (USE_MOCK) return mockApi.createTimeOffRequest(input);
    return apiFetch<number>("/TimeOff/requests", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function withdrawRequest(id: number): Promise<void> {
    if (USE_MOCK) return mockApi.withdrawRequest(id);
    return apiFetch<void>("/TimeOff/requests/withdraw", {
        method: "POST",
        body: JSON.stringify({ id }),
    });
}