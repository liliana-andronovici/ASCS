import { useQuery, keepPreviousData } from "@tanstack/react-query";
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