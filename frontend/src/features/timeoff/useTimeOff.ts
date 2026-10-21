import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createTimeOffRequest, deleteTimeOffRequest, getMyRequests } from "./api";

export const timeOffKeys = {
    requests: ["time-off", "requests"] as const,
};

export function useTimeOffRequests() {
    return useQuery({ queryKey: timeOffKeys.requests, queryFn: getMyRequests });
}

export function useCreateTimeOffRequest() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: createTimeOffRequest,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: timeOffKeys.requests }),
    });
}

export function useDeleteTimeOffRequest() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: deleteTimeOffRequest,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: timeOffKeys.requests }),
    });
}
