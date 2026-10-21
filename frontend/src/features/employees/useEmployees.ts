import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { getEmployeeById, getEmployees } from "./api";
import type { EmployeeQuery } from "./types";

export function useEmployees(query: EmployeeQuery = {}) {
    return useQuery({
        queryKey: ["employees", query],
        queryFn: () => getEmployees(query),
        placeholderData: keepPreviousData,
    });
}

export function useEmployeeDirectory() {
    return useQuery({
        queryKey: ["employees", "directory"],
        queryFn: () => getEmployees({ pageSize: 500 }),
        staleTime: 5 * 60 * 1000,
    });
}

export function useEmployeeById(id: number | null) {
    return useQuery({
        queryKey: ["employees", id],
        queryFn: () => getEmployeeById(id!),
        enabled: id !== null,
    });
}
