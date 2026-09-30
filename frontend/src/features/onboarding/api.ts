import { apiFetch } from "../../lib/api";
import { mockApi } from "./mock";
import type { AddTaskInput, OnboardingChecklist } from "./types";

const USE_MOCK = import.meta.env.VITE_USE_MOCK === "true";

export function getChecklist(): Promise<OnboardingChecklist> {
    if (USE_MOCK) return mockApi.getChecklist();
    return apiFetch<OnboardingChecklist>("/onboarding/checklist");
}

export function toggleTask(taskId: number): Promise<OnboardingChecklist> {
    if (USE_MOCK) return mockApi.toggleTask(taskId);
    return apiFetch<OnboardingChecklist>(`/onboarding/tasks/${taskId}/toggle`, {
        method: "POST",
    });
}

export function addTask(input: AddTaskInput): Promise<OnboardingChecklist> {
    if (USE_MOCK) return mockApi.addTask(input);
    return apiFetch<OnboardingChecklist>("/onboarding/tasks", {
        method: "POST",
        body: JSON.stringify(input),
    });
}