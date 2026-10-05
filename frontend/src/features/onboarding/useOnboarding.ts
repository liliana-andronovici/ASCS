import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { addTaskFor, getChecklist, getChecklistFor, getSummaries, toggleTask } from "./api";
import type { AddTaskInput } from "./types";

export function useOnboardingSummaries() {
    return useQuery({ queryKey: ["onboarding", "summaries"], queryFn: getSummaries });
}

export function useOnboardingChecklist() {
    return useQuery({ queryKey: ["onboarding", "checklist"], queryFn: getChecklist });
}

export function useOnboardingChecklistFor(employeeId: number) {
    return useQuery({
        queryKey: ["onboarding", "checklist", employeeId],
        queryFn: () => getChecklistFor(employeeId),
    });
}

export function useToggleOnboardingTask(queryKey: unknown[]) {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (taskId: number) => toggleTask(taskId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey }),
    });
}

export function useAddOnboardingTaskFor(employeeId: number) {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: AddTaskInput) => addTaskFor(employeeId, input),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: ["onboarding", "checklist", employeeId] }),
    });
}