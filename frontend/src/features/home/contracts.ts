import { z } from "zod";

const dateString = z.string().refine((value) => Number.isFinite(Date.parse(value)), "Invalid date");
export const homeEmployeeSchema = z.object({
    id: z.number().int().positive(), firstName: z.string(), lastName: z.string(),
    email: z.string(), title: z.string(), startedOnUtc: dateString,
    departmentId: z.number().int().nullable(), locationId: z.number().int().nullable(), managerId: z.number().int().nullable(),
    status: z.enum(["Active", "Inactive"]),
});
export const homeEmployeePageSchema = z.object({
    items: z.array(homeEmployeeSchema), totalCount: z.number().int().nonnegative(),
    page: z.number().int().positive(), pageSize: z.number().int().positive(),
});

// Optional dashboard feeds are proposed contracts, not endpoints in the current backend.
export const leaveSchema = z.object({
    id: z.number().int().positive(), employeeId: z.number().int().positive(),
    startDateUtc: dateString, endDateUtc: dateString,
    type: z.enum(["Vacation", "Sick", "Remote"]), status: z.enum(["Pending", "Approved", "Declined"]),
}).refine((item) => Date.parse(item.endDateUtc) >= Date.parse(item.startDateUtc), "Invalid leave interval");
export const meetingSchema = z.object({
    id: z.number().int().positive(), title: z.string(),
    startUtc: z.iso.datetime({ offset: true }), endUtc: z.iso.datetime({ offset: true }), location: z.string().nullable(),
}).refine((item) => Date.parse(item.endUtc) > Date.parse(item.startUtc), "Invalid meeting interval");
export type LeaveRequest = z.infer<typeof leaveSchema>;
export type Meeting = z.infer<typeof meetingSchema>;
