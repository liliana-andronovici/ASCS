import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createEmployee, updateEmployee, deleteEmployee } from "./api";
import type { CreateEmployeeInput, UpdateEmployeeInput } from "./api";

export function useCreateEmployee() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: CreateEmployeeInput) => createEmployee(input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["employees"] }),
    });
}

export function useUpdateEmployee() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: ({ id, input }: { id: number; input: UpdateEmployeeInput }) =>
            updateEmployee(id, input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["employees"] }),
    });
}

export function useDeleteEmployee() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (id: number) => deleteEmployee(id),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["employees"] }),
    });
}