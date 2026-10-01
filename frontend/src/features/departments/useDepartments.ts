import { useQuery } from "@tanstack/react-query";
import { getDepartments } from "./api";

export function useDepartments() {
    return useQuery({
        queryKey: ["departments"],
        queryFn: getDepartments,
        staleTime: 5 * 60 * 1000,
    });
}