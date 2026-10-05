import { apiFetch } from "../../lib/api";
import type { AddTaskInput, OnboardingChecklist, OnboardingSummary } from "./types";

export function getSummaries(): Promise<OnboardingSummary[]> {
    return apiFetch<OnboardingSummary[]>("/Onboarding");
}

export function getChecklist(): Promise<OnboardingChecklist> {
    return apiFetch<OnboardingChecklist>("/Onboarding/checklist");
}

export function getChecklistFor(employeeId: number): Promise<OnboardingChecklist> {
    return apiFetch<OnboardingChecklist>(`/Onboarding/${employeeId}`);
}

export function toggleTask(taskId: number): Promise<void> {
    return apiFetch<void>(`/Onboarding/tasks/${taskId}/toggle`, {
        method: "POST",
    });
}

export function addTaskFor(employeeId: number, input: AddTaskInput): Promise<void> {
    return apiFetch<void>(`/Onboarding/${employeeId}/tasks`, {
        method: "POST",
        body: JSON.stringify(input),
    });
}