import { z } from "zod";
import { apiFetch } from "../../lib/api";
import type { Employee } from "../employees/types";
import { homeEmployeePageSchema, meetingSchema } from "./contracts";

export const HOME_LEAVE_PATH = "/TimeOff/out-today";
export const HOME_MEETINGS_PATH = import.meta.env.VITE_HOME_MEETINGS_API_PATH?.trim() || "/RoomBookings";

async function getHomeEmployeesFromApi(signal?: AbortSignal): Promise<Employee[]> {
    const employees: Employee[] = [];
    const ids = new Set<number>();
    let totalCount: number | undefined;
    for (let page = 1; ; page++) {
        const query = new URLSearchParams({ page: String(page), pageSize: "100", sortBy: "Id", sortDirection: "Ascending" });
        const response = homeEmployeePageSchema.parse(await apiFetch<unknown>(`/employees?${query}`, { signal }));
        totalCount ??= response.totalCount;
        if (response.page !== page || response.totalCount !== totalCount || response.items.length > response.pageSize) {
            throw new Error("Employee pagination changed or is invalid. Please refresh.");
        }
        for (const employee of response.items) {
            if (ids.has(employee.id)) throw new Error("Employee pagination contains duplicates. Please refresh.");
            ids.add(employee.id);
            employees.push(employee);
        }
        if (employees.length === totalCount) return employees;
        if (employees.length > totalCount || response.items.length === 0 || response.items.length < response.pageSize) {
            throw new Error("Incomplete employee directory. Please refresh.");
        }
    }
}

export async function getHomeEmployees(signal?: AbortSignal): Promise<Employee[]> {
    return getHomeEmployeesFromApi(signal);
}

export async function getHomeOutToday(date: string, signal?: AbortSignal) {
    const response = await apiFetch<unknown>(`${HOME_LEAVE_PATH}?${new URLSearchParams({ date })}`, { signal });
    return z.object({ count: z.number().int().nonnegative() }).parse(response);
}

export async function getHomeMeetings(from: string, to: string, employeeId: number, signal?: AbortSignal) {
    const query = new URLSearchParams({ fromUtc: from, toUtc: to });
    const bookings = z.array(z.object({
        id: z.number().int().positive(),
        title: z.string(),
        roomName: z.string(),
        organizerEmployeeId: z.number().int().positive(),
        startDateTimeUtc: z.iso.datetime({ offset: true }),
        durationMinutes: z.number().int().positive(),
    })).parse(await apiFetch<unknown>(`${HOME_MEETINGS_PATH}?${query}`, { signal }));

    return bookings.filter((booking) => booking.organizerEmployeeId === employeeId).map((booking) => meetingSchema.parse({
        id: booking.id,
        title: booking.title,
        startUtc: booking.startDateTimeUtc,
        endUtc: new Date(Date.parse(booking.startDateTimeUtc) + booking.durationMinutes * 60_000).toISOString(),
        location: `Meeting room · ${booking.roomName}`,
    }));
}
