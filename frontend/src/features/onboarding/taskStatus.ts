import type { OnboardingTask, OnboardingTaskStatus } from "./types";

export function todayIso(): string {
    return new Date().toISOString().slice(0, 10);
}

export function getTaskStatus(task: OnboardingTask): OnboardingTaskStatus {
    if (task.completedOn) return "done";
    if (task.dueDate && task.dueDate < todayIso()) return "overdue";
    return "pending";
}

function formatShort(iso: string): string {
    return new Date(iso).toLocaleDateString("en-US", { day: "numeric", month: "short" });
}

export function getTaskDisplayNote(task: OnboardingTask): string {
    if (task.note) return task.note;
    if (task.completedOn) return `done ${formatShort(task.completedOn)}`;
    if (!task.dueDate) return "";
    return getTaskStatus(task) === "overdue"
        ? `overdue since ${formatShort(task.dueDate)}`
        : `by ${formatShort(task.dueDate)}`;
}