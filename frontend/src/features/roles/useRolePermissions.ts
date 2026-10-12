import { useMutation, useQuery } from "@tanstack/react-query";
import { createRole, getPermissions, getRoles, setRolePermissions } from "./api";

export function useRoles() {
    return useQuery({
        queryKey: ["roles"],
        queryFn: getRoles,
    });
}

export function usePermissions() {
    return useQuery({
        queryKey: ["permissions"],
        queryFn: getPermissions,
    });
}

export function useCreateRole() {
    return useMutation({ mutationFn: createRole });
}

export function useSetRolePermissions() {
    return useMutation({
        mutationFn: ({ roleId, permissions }: { roleId: number; permissions: string[] }) =>
            setRolePermissions(roleId, permissions),
    });
}
