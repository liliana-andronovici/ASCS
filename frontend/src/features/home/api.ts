import { z } from "zod";
import { apiFetch } from "../../lib/api";
import type { Employee } from "../employees/types";
import { homeEmployeePageSchema, leaveSchema, meetingSchema } from "./contracts";

export const HOME_LEAVE_PATH = import.meta.env.VITE_HOME_LEAVE_API_PATH?.trim() || "";
export const HOME_MEETINGS_PATH = import.meta.env.VITE_HOME_MEETINGS_API_PATH?.trim() || "";

export async function getHomeEmployees(signal?: AbortSignal): Promise<Employee[]> {
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

function feedPath(path: string, from: string, to: string) {
    if (!path) throw new Error("This dashboard feed is not configured.");
    return `${path}${path.includes("?") ? "&" : "?"}${new URLSearchParams({ from, to })}`;
}

export async function getHomeLeave(from: string, to: string, signal?: AbortSignal) {
    return z.array(leaveSchema).parse(await apiFetch<unknown>(feedPath(HOME_LEAVE_PATH, from, to), { signal }));
}

export async function getHomeMeetings(from: string, to: string, signal?: AbortSignal) {
    return z.array(meetingSchema).parse(await apiFetch<unknown>(feedPath(HOME_MEETINGS_PATH, from, to), { signal }));
}
