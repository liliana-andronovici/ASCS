import { apiFetch } from "../../lib/api";
import type { Department } from "./types";

export function getDepartments(): Promise<Department[]> {
    return apiFetch<Department[]>("/Department");
}

export interface CreateDepartmentInput {
    name: string;
}

export type UpdateDepartmentInput = CreateDepartmentInput;

export function createDepartment(input: CreateDepartmentInput): Promise<number> {
    return apiFetch<number>("/Department", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function updateDepartment(id: number, input: UpdateDepartmentInput): Promise<void> {
    return apiFetch<void>("/Department", {
        method: "PUT",
        body: JSON.stringify({ id, ...input }),
    });
}

export function deleteDepartment(id: number): Promise<void> {
    return apiFetch<void>("/Department/" + id, {
        method: "DELETE",
    });
}