import { apiFetch } from "../../lib/api";
import type { Permission, Role, RoleEmployee } from "./types";

export function getRoles(): Promise<Role[]> {
    return apiFetch<Role[]>("/roles");
}

export function getPermissions(): Promise<Permission[]> {
    return apiFetch<Permission[]>("/permissions");
}

export function createRole(name: string): Promise<{ id: number }> {
    return apiFetch<{ id: number }>("/roles", {
        method: "POST",
        body: JSON.stringify({ name }),
    });
}

export function setRolePermissions(roleId: number, permissions: string[]): Promise<void> {
    return apiFetch<void>(`/roles/${roleId}/permissions`, {
        method: "PUT",
        body: JSON.stringify(permissions),
    });
}

export function renameRole(roleId: number, name: string): Promise<void> {
    return apiFetch<void>(`/roles/${roleId}`, {
        method: "PUT",
        body: JSON.stringify({ name }),
    });
}

export function deleteRole(roleId: number): Promise<void> {
    return apiFetch<void>(`/roles/${roleId}`, { method: "DELETE" });
}

export function getRoleEmployees(roleId: number): Promise<RoleEmployee[]> {
    return apiFetch<RoleEmployee[]>(`/roles/${roleId}/employees`);
}

export function addRoleEmployee(roleId: number, employeeId: number): Promise<void> {
    return apiFetch<void>(`/roles/${roleId}/employees/${employeeId}`, { method: "POST" });
}

export function removeRoleEmployee(roleId: number, employeeId: number): Promise<void> {
    return apiFetch<void>(`/roles/${roleId}/employees/${employeeId}`, { method: "DELETE" });
}
