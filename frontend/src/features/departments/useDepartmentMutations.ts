import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createDepartment, updateDepartment, deleteDepartment } from "./api";
import type { CreateDepartmentInput, UpdateDepartmentInput } from "./api";

export function useCreateDepartment() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: CreateDepartmentInput) => createDepartment(input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["departments"] }),
    });
}

export function useUpdateDepartment() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ id, input }: { id: number; input: UpdateDepartmentInput }) =>
            updateDepartment(id, input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["departments"] }),
    });
}

export function useDeleteDepartment() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (id: number) => deleteDepartment(id),
        onSuccess: () => Promise.all([
            queryClient.invalidateQueries({ queryKey: ["departments"] }),
            queryClient.invalidateQueries({ queryKey: ["employees"] }),
        ]),
    });
}
