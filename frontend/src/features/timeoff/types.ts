export const TIME_OFF_TYPES = ["Vacation", "Sick", "Remote"] as const;
export type TimeOffType = (typeof TIME_OFF_TYPES)[number];
export type TimeOffStatus = "Pending" | "Approved" | "Declined";

export type TimeOffRequest = {
    id: number;
    type: TimeOffType;
    startDateUtc: string;
    endDateUtc: string;
    note: string;
    status: TimeOffStatus;
    createdOnUtc: string;
    updatedOnUtc: string;
};

export type TimeOffRequestInput = {
    type: TimeOffType;
    startDate: string;
    endDate: string;
    note: string;
};
