import { apiFetch } from "../../lib/api";
import type { Employee, EmployeeDocument, EmployeeEquipment, EmployeeHistory, EmployeeProfile, PagedResult, EmployeeQuery } from "./types";

export function getEmployees(query: EmployeeQuery = {}): Promise<PagedResult<Employee>> {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([key, value]) => {
        if (value !== undefined) params.set(key, String(value));
    });
    const qs = params.toString();
    return apiFetch<PagedResult<Employee>>(`/employees${qs ? `?${qs}` : ""}`);
}

export function getEmployeeById(id: number): Promise<Employee> {
    return apiFetch<Employee>(`/employees/${id}`);
}

export function getEmployeeProfile(id: number): Promise<EmployeeProfile> {
    return apiFetch<EmployeeProfile>(`/people/${id}`);
}

export function patchEmployeeProfile(id: number, changes: Record<string, unknown>): Promise<EmployeeProfile> {
    return apiFetch<EmployeeProfile>(`/people/${id}`, { method: "PATCH", body: JSON.stringify(changes) });
}

export function getEmployeeDocuments(id: number): Promise<PagedResult<EmployeeDocument>> {
    return apiFetch<PagedResult<EmployeeDocument>>(`/people/${id}/documents`);
}

export function getEmployeeHistory(id: number): Promise<PagedResult<EmployeeHistory>> {
    return apiFetch<PagedResult<EmployeeHistory>>(`/people/${id}/history`);
}

export function getEmployeeEquipment(id: number): Promise<EmployeeEquipment[]> {
    return apiFetch<EmployeeEquipment[]>(`/employees/${id}/equipment`);
}

export interface CreateEmployeeEquipmentInput {
    equipmentId: number;
    assignedOn: string;
}

export function addEmployeeEquipment(id: number, input: CreateEmployeeEquipmentInput): Promise<number> {
    return apiFetch<number>(`/employees/${id}/equipment`, {
        method: "POST",
        body: JSON.stringify(input),
    });
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

export function createEmployee(input: CreateEmployeeInput): Promise<number> {
    return apiFetch<number>("/employees", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function updateEmployee(id: number, input: UpdateEmployeeInput): Promise<void> {
    return apiFetch<void>("/employees", {
        method: "PUT",
        body: JSON.stringify({ id, ...input }),
    });
}

export function deleteEmployee(id: number): Promise<void> {
    return apiFetch<void>(`/employees/${id}`, {
        method: "DELETE",
    });
}
