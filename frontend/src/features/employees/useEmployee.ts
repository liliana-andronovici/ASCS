import { useQuery } from "@tanstack/react-query";
import { getEmployeeById } from "./api";

export function useEmployee(id: number) {
    return useQuery({
        queryKey: ["employees", "detail", id],
        queryFn: () => getEmployeeById(id),
        enabled: Number.isFinite(id),
    });
}
