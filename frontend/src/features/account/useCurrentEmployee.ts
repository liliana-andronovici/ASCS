import { useQuery } from "@tanstack/react-query";
import { getCurrentEmployeeId } from "../../lib/sessions";
import { getEmployeeById } from "../employees/api";

export function useCurrentEmployee() {
    const employeeId = getCurrentEmployeeId();
    const query = useQuery({
        queryKey: ["employees", employeeId],
        queryFn: () => getEmployeeById(employeeId!),
        enabled: employeeId !== null,
    });
    return { employee: query.data, isLoading: query.isLoading, isError: query.isError };
}
