import { getCurrentEmployeeId } from "../../lib/sessions";
import { useEmployeeById } from "../employees/useEmployees";

export function useCurrentEmployee() {
    const employeeId = getCurrentEmployeeId();
    const query = useEmployeeById(employeeId);
    return { employee: query.data, isLoading: query.isLoading, isError: query.isError };
}
