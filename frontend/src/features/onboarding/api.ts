import { apiFetch } from "../../lib/api";
import type { AddTaskInput, OnboardingChecklist } from "./types";

export function getChecklist(): Promise<OnboardingChecklist> {
    return apiFetch<OnboardingChecklist>("/Onboarding/checklist");
}

export function toggleTask(taskId: number): Promise<void> {
    return apiFetch<void>(`/Onboarding/tasks/${taskId}/toggle`, {
        method: "POST",
    });
}

export function addTask(input: AddTaskInput): Promise<void> {
    return apiFetch<void>("/Onboarding/tasks", {
        method: "POST",
        body: JSON.stringify(input),
    });
}