import { todayIso } from "./taskStatus";
import type { AddTaskInput, OnboardingChecklist } from "./types";

let nextTaskId = 100;

let checklist: OnboardingChecklist = {
    firstName: "Mihai",
    lastName: "Dinu",
    title: "Product Designer",
    department: "Product",
    location: "Bucharest",
    startsOn: "2026-09-14",
    managerName: "Elena Stan",
    buddyName: "Ioana Rusu",
    sections: [
        {
            key: "beforeDayOne",
            title: "Before day one",
            tasks: [
                { id: 1, title: "Send offer and contract", assignee: "Ana", dueDate: null, completedOn: null, note: "signed 20 Aug" },
                { id: 2, title: "Create accounts: Google, Slack, Figma", assignee: "IT", dueDate: null, completedOn: null, note: "done 1 Sep" },
                { id: 3, title: "Order MacBook Pro 14\"", assignee: "IT", dueDate: null, completedOn: null, note: "shipped" },
                { id: 4, title: "Assign buddy", assignee: "Elena · Ioana Rusu", dueDate: null, completedOn: "2026-08-25" },
                { id: 5, title: "Book desk in Bucharest", assignee: "Ana · B-07", dueDate: null, completedOn: "2026-08-25" },
                { id: 6, title: "Add to payroll", assignee: "Laura", dueDate: null, completedOn: null, note: "done 3 Sep" },
                { id: 7, title: "Collect ID and bank details", assignee: "Mihai", dueDate: "2026-09-02", completedOn: null },
            ],
        },
        {
            key: "firstWeek",
            title: "First week",
            tasks: [
                { id: 8, title: "Welcome breakfast with team", assignee: "Elena", dueDate: "2026-09-14", completedOn: "2026-09-14" },
                { id: 9, title: "Security and data training", assignee: "IT", dueDate: "2026-09-14", completedOn: "2026-09-14" },
                { id: 10, title: "Read product handbook", assignee: "Mihai", dueDate: "2026-09-16", completedOn: "2026-09-15" },
                { id: 11, title: "1:1 with manager", assignee: "Elena", dueDate: "2026-09-15", completedOn: null },
                { id: 12, title: "Design tooling walkthrough", assignee: "Ioana", dueDate: "2026-09-16", completedOn: null },
                { id: 13, title: "Sign equipment receipt", assignee: "Mihai", dueDate: "2026-09-15", completedOn: null },
            ],
        },
        {
            key: "firstMonth",
            title: "First month",
            tasks: [
                { id: 14, title: "Set 90-day goals", assignee: "Elena + Mihai", dueDate: "2026-09-28", completedOn: null },
                { id: 15, title: "Meet each department lead", assignee: "Mihai", dueDate: "2026-10-09", completedOn: null },
                { id: 16, title: "30-day check-in with People", assignee: "Ana", dueDate: "2026-10-14", completedOn: null },
            ],
        },
    ],
};

const delay = <T,>(value: T): Promise<T> =>
    new Promise((resolve) => setTimeout(() => resolve(value), 200));

export const mockApi = {
    getChecklist: () => delay(checklist),

    toggleTask: (taskId: number): Promise<OnboardingChecklist> => {
        checklist = {
            ...checklist,
            sections: checklist.sections.map((section) => ({
                ...section,
                tasks: section.tasks.map((task) =>
                    task.id === taskId
                        ? { ...task, completedOn: task.completedOn ? null : todayIso() }
                        : task
                ),
            })),
        };
        return delay(checklist);
    },

    addTask: (input: AddTaskInput): Promise<OnboardingChecklist> => {
        checklist = {
            ...checklist,
            sections: checklist.sections.map((section) =>
                section.key === input.sectionKey
                    ? {
                        ...section,
                        tasks: [
                            ...section.tasks,
                            {
                                id: nextTaskId++,
                                title: input.title,
                                assignee: input.assignee,
                                dueDate: input.dueDate || null,
                                completedOn: null,
                            },
                        ],
                    }
                    : section
            ),
        };
        return delay(checklist);
    },
};