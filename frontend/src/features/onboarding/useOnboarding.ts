import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { addTask, getChecklist, toggleTask } from "./api";
import type { AddTaskInput } from "./types";

const QUERY_KEY = ["onboarding", "checklist"];

export function useOnboardingChecklist() {
    return useQuery({ queryKey: QUERY_KEY, queryFn: getChecklist });
}

export function useToggleOnboardingTask() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (taskId: number) => toggleTask(taskId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
    });
}

export function useAddOnboardingTask() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: AddTaskInput) => addTask(input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
    });
}