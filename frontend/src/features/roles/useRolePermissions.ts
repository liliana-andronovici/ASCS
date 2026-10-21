import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { addRoleEmployee, createRole, deleteRole, getPermissions, getRoleEmployees, getRoles, removeRoleEmployee, renameRole, setRolePermissions } from "./api";

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
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: createRole,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
    });
}

export function useSetRolePermissions() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ roleId, permissions }: { roleId: number; permissions: string[] }) =>
            setRolePermissions(roleId, permissions),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
    });
}

export function useRenameRole() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ roleId, name }: { roleId: number; name: string }) => renameRole(roleId, name),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
    });
}

export function useDeleteRole() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: deleteRole,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["roles"] }),
    });
}

export function useRoleEmployees(roleId: number | null) {
    return useQuery({
        queryKey: ["roles", roleId, "employees"],
        queryFn: () => getRoleEmployees(roleId!),
        enabled: roleId !== null,
    });
}

export function useAddRoleEmployee() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ roleId, employeeId }: { roleId: number; employeeId: number }) => addRoleEmployee(roleId, employeeId),
        onSuccess: (_, { roleId }) => Promise.all([
            queryClient.invalidateQueries({ queryKey: ["roles", roleId, "employees"] }),
            queryClient.invalidateQueries({ queryKey: ["roles"] }),
        ]),
    });
}

export function useRemoveRoleEmployee() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ roleId, employeeId }: { roleId: number; employeeId: number }) => removeRoleEmployee(roleId, employeeId),
        onSuccess: (_, { roleId }) => Promise.all([
            queryClient.invalidateQueries({ queryKey: ["roles", roleId, "employees"] }),
            queryClient.invalidateQueries({ queryKey: ["roles"] }),
        ]),
    });
}
