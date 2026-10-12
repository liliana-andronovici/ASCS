import { apiFetch } from "../../lib/api";
import type { Permission, Role } from "./types";

export function getRoles(): Promise<Role[]> {
    return apiFetch<Role[]>("/RoleJobs");
}

export function getPermissions(): Promise<Permission[]> {
    return apiFetch<Permission[]>("/PermissionJobs");
}

export function createRole(name: string): Promise<{ jobId: string }> {
    return apiFetch<{ jobId: string }>("/RoleJobs", {
        method: "POST",
        body: JSON.stringify({ name }),
    });
}

export function setRolePermissions(roleId: number, permissions: string[]): Promise<{ jobId: string }> {
    return apiFetch<{ jobId: string }>(`/RoleJobs/${roleId}/permissions`, {
        method: "PUT",
        body: JSON.stringify(permissions),
    });
}
