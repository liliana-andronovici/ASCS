export const TIME_OFF_TYPES = ["Vacation", "Sick", "Remote"] as const;
export type TimeOffType = (typeof TIME_OFF_TYPES)[number];
export type TimeOffStatus = "Pending" | "Approved" | "Declined" | "Withdrawn";

export type TimeOffBalance = {
    type: TimeOffType;
    label: string;
    headline: { value: number; label: string };
    used: number;
    pending: number;
    total: number | null;
    caption: string;
};

export type TimeOffOverview = {
    approverName: string;
    balances: TimeOffBalance[];
};

export type TimeOffRequest = {
    id: number;
    type: TimeOffType;
    startDate: string;
    endDate: string;
    days: number;
    approverName: string;
    status: TimeOffStatus;
    canWithdraw: boolean;
};

export type TimeOffRequestInput = {
    type: TimeOffType;
    startDate: string;
    endDate: string;
    note: string;
};

export type TimeOffPreviewInput = Pick<TimeOffRequestInput, "type" | "startDate" | "endDate">;

export type TimeOffPreview = {
    workingDays: number;
    remainingAfter: number | null;
};
