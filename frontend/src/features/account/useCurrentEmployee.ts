import { useEmployeeById } from "../employees/useEmployees";
import { getCurrentEmployeeId } from "../../lib/sessions";

export function useCurrentEmployee() {
    const employeeId = getCurrentEmployeeId();
    const query = useEmployeeById(employeeId);

    return { employee: query.data, isLoading: query.isLoading, isError: query.isError };
}
