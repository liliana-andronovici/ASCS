import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createLocation, updateLocation, deleteLocation } from "./api";

export function useCreateLocation() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: { name: string }) => createLocation(input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["locations"] }),
    });
}

export function useUpdateLocation() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (input: { id: number; name: string }) => updateLocation(input),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["locations"] }),
    });
}

export function useDeleteLocation() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (id: number) => deleteLocation(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["locations"] });
            queryClient.invalidateQueries({ queryKey: ["employees"] });
        },
    });
}
