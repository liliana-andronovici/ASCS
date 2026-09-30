export type OnboardingTaskStatus = "done" | "pending" | "overdue";

export interface OnboardingTask {
    id: number;
    title: string;
    assignee: string;
    dueDate: string | null;
    completedOn: string | null;
    note?: string;
}

export interface OnboardingSection {
    key: "beforeDayOne" | "firstWeek" | "firstMonth";
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
    assignee: string;
    dueDate: string;
}