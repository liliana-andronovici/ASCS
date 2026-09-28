import { apiFetch } from "../../lib/api";
import type { Employee, PagedResult, EmployeeQuery } from "./types";

export function getEmployees(query: EmployeeQuery = {}): Promise<PagedResult<Employee>> {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([key, value]) => {
        if (value !== undefined) params.set(key, String(value));
    });
    const qs = params.toString();
    return apiFetch<PagedResult<Employee>>(`/Employee${qs ? `?${qs}` : ""}`);
}

export function getEmployeeById(id: number): Promise<Employee> {
    return apiFetch<Employee>(`/Employee/${id}`);
}

export interface CreateEmployeeInput {
    firstName: string;
    lastName: string;
    email: string;
    title: string;
    startedOnUtc: string;
    departmentId: number | null;
    locationId: number | null;
    managerId: number | null;
    status: "Active" | "Inactive";
}

export type UpdateEmployeeInput = CreateEmployeeInput;

export function createEmployee(input: CreateEmployeeInput): Promise<void> {
    return apiFetch<void>("/Employee", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function updateEmployee(id: number, input: UpdateEmployeeInput): Promise<void> {
    return apiFetch<void>("/Employee", {
        method: "PUT",
        body: JSON.stringify({ id, ...input }),
    });
}

export function deleteEmployee(id: number): Promise<void> {
    return apiFetch<void>(`/Employee/${id}`, {
        method: "DELETE",
    });
}