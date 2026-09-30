import { useEmployeeById } from "../employees/useEmployees";
import { getCurrentEmployeeId } from "../../lib/sessions";
import type { Employee } from "../employees/types";

export const FALLBACK_EMPLOYEE: Employee = {
    id: 0,
    firstName: "Ana",
    lastName: "Popescu",
    email: "ana.popescu@infigo.ro",
    title: "People & Culture Manager",
    startedOnUtc: new Date().toISOString(),
    departmentId: null,
    locationId: null,
    managerId: null,
    status: "Active",
};

export function useCurrentEmployee() {
    const employeeId = getCurrentEmployeeId();
    const query = useEmployeeById(employeeId);

    const employee: Employee = query.data ?? FALLBACK_EMPLOYEE;
    const isFallback = employeeId === null || !query.data;

    return { employee, isFallback, isLoading: query.isLoading };
}