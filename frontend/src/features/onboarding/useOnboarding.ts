import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { addTaskFor, getChecklistFor, getSummaries, toggleTask } from "./api";
import type { AddTaskInput } from "./types";

export function useOnboardingSummaries() {
    return useQuery({ queryKey: ["onboarding", "summaries"], queryFn: getSummaries });
}

export function useOnboardingChecklistFor(employeeId: number) {
    return useQuery({
        queryKey: ["onboarding", "checklist", employeeId],
        queryFn: () => getChecklistFor(employeeId),
        enabled: Number.isSafeInteger(employeeId) && employeeId > 0,
    });
}

export function useToggleOnboardingTask(employeeId: number) {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (taskId: number) => toggleTask(employeeId, taskId),
        onSuccess: async () => {
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ["onboarding", "checklist", employeeId] }),
                queryClient.invalidateQueries({ queryKey: ["onboarding", "summaries"] }),
            ]);
        },
    });
}

export function useAddOnboardingTaskFor(employeeId: number) {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: AddTaskInput) => addTaskFor(employeeId, input),
        onSuccess: async () => {
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ["onboarding", "checklist", employeeId] }),
                queryClient.invalidateQueries({ queryKey: ["onboarding", "summaries"] }),
            ]);
        },
    });
}
