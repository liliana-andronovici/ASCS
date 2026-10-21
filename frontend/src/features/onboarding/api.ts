import { apiFetch } from "../../lib/api";
import type { AddTaskInput, OnboardingChecklist, OnboardingSummary, UpcomingOnboardingTask } from "./types";

export function getSummaries(): Promise<OnboardingSummary[]> {
    return apiFetch<OnboardingSummary[]>("/Onboarding");
}

export function getUpcomingTasks(): Promise<UpcomingOnboardingTask[]> {
    return apiFetch<UpcomingOnboardingTask[]>("/Onboarding/upcoming");
}

export function getChecklistFor(employeeId: number): Promise<OnboardingChecklist> {
    return apiFetch<OnboardingChecklist>(`/Onboarding/${employeeId}`);
}

export function toggleTask(employeeId: number, taskId: number): Promise<void> {
    return apiFetch<void>(`/Onboarding/${employeeId}/tasks/${taskId}/toggle`, {
        method: "POST",
    });
}

export function addTaskFor(employeeId: number, input: AddTaskInput): Promise<void> {
    return apiFetch<void>(`/Onboarding/${employeeId}/tasks`, {
        method: "POST",
        body: JSON.stringify(input),
    });
}
