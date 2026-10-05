export type OnboardingTaskStatus = "done" | "pending" | "overdue";

export interface OnboardingTask {
    id: number;
    title: string;
    note?: string;
    dueDate: string | null;
    completedOn: string | null;
}

export interface OnboardingSection {
    key: "beforeDayOne" | "firstWeek" | "firstMonth" | "other";
    title: string;
    tasks: OnboardingTask[];
}

export interface OnboardingChecklist {
    firstName: string;
    lastName: string;
    title: string;
    department: string;
    location: string;
    startsOn: string;
    managerName: string;
    buddyName: string;
    sections: OnboardingSection[];
}

export interface AddTaskInput {
    sectionKey: OnboardingSection["key"];
    title: string;
    note?: string;
    dueDate: string;
}

export interface OnboardingSummary {
    employeeId: number;
    firstName: string;
    lastName: string;
    title: string;
    department: string;
    startsOn: string;
    completedTaskCount: number;
    totalTaskCount: number;
    isComplete: boolean;
}