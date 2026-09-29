import type {
    TimeOffOverview,
    TimeOffPreview,
    TimeOffPreviewInput,
    TimeOffRequest,
    TimeOffRequestInput,
} from "./types";

let nextId = 4;

let requests: TimeOffRequest[] = [
    { id: 3, type: "Vacation", startDate: "2026-11-02", endDate: "2026-11-06", days: 5, approverName: "Andrei Matei", status: "Pending", canWithdraw: true },
    { id: 2, type: "Sick", startDate: "2026-09-14", endDate: "2026-09-15", days: 2, approverName: "Andrei Matei", status: "Approved", canWithdraw: false },
    { id: 1, type: "Vacation", startDate: "2026-07-20", endDate: "2026-07-31", days: 10, approverName: "Andrei Matei", status: "Approved", canWithdraw: false },
];

const overview: TimeOffOverview = {
    policyName: "Standard 25-day",
    resetsOn: "2027-01-01",
    approverName: "Andrei Matei",
    balances: [
        { type: "Vacation", label: "Vacation", headline: { value: 13, label: "days left" }, used: 12, pending: 5, total: 25, caption: "12 used · 5 requested · 8 left" },
        { type: "Sick", label: "Sick", headline: { value: 2, label: "days taken" }, used: 2, pending: 0, total: null, caption: "No limit" },
        { type: "Remote", label: "Remote", headline: { value: 4, label: "days this month" }, used: 4, pending: 0, total: null, caption: "Hybrid · Tue, Thu in office" },
    ],
};

const delay = <T,>(value: T): Promise<T> =>
    new Promise((resolve) => setTimeout(() => resolve(value), 300));

function workingDays(startDate: string, endDate: string): number {
    let count = 0;
    const current = new Date(startDate);
    const end = new Date(endDate);
    while (current <= end) {
        const day = current.getUTCDay();
        if (day !== 0 && day !== 6) count++;
        current.setUTCDate(current.getUTCDate() + 1);
    }
    return count;
}

export const mockApi = {
    getOverview: () => delay(overview),

    getMyRequests: () => delay([...requests]),

    previewRequest: (input: TimeOffPreviewInput): Promise<TimeOffPreview> =>
        delay({
            workingDays: workingDays(input.startDate, input.endDate),
            remainingAfter: input.type === "Vacation" ? 8 - workingDays(input.startDate, input.endDate) : null,
            overlappingPeople: ["Ioana Rusu", "Dan Ciobanu"],
        }),

    createTimeOffRequest: (input: TimeOffRequestInput): Promise<number> => {
        const id = nextId++;
        requests = [
            {
                id,
                type: input.type,
                startDate: input.startDate,
                endDate: input.endDate,
                days: workingDays(input.startDate, input.endDate),
                approverName: "Andrei Matei",
                status: "Pending",
                canWithdraw: true,
            },
            ...requests,
        ];
        return delay(id);
    },

    withdrawRequest: (id: number): Promise<void> => {
        requests = requests.map((r) =>
            r.id === id ? { ...r, status: "Withdrawn", canWithdraw: false } : r
        );
        return delay(undefined);
    },
};