import { skipToken, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
    createTimeOffRequest,
    getMyRequests,
    getOverview,
    previewRequest,
    withdrawRequest,
} from "./api";
import type { TimeOffPreviewInput } from "./types";

export const timeOffKeys = {
    all: ["time-off"] as const,
    overview: ["time-off", "overview"] as const,
    requests: ["time-off", "requests"] as const,
    preview: (input: TimeOffPreviewInput | null) => ["time-off", "preview", input] as const,
};

export function useTimeOffOverview() {
    return useQuery({ queryKey: timeOffKeys.overview, queryFn: getOverview });
}

export function useTimeOffRequests() {
    return useQuery({ queryKey: timeOffKeys.requests, queryFn: getMyRequests });
}

export function useRequestPreview(input: TimeOffPreviewInput | null) {
    return useQuery({
        queryKey: timeOffKeys.preview(input),
        queryFn: input ? () => previewRequest(input) : skipToken,
    });
}

export function useCreateTimeOffRequest() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: createTimeOffRequest,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: timeOffKeys.all }),
    });
}

export function useWithdrawTimeOffRequest() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: withdrawRequest,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: timeOffKeys.all }),
    });
}