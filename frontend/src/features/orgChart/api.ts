import { apiFetch } from "../../lib/api";
import { getEmployees } from "../employees/api";
import type { OrgChartEmployee } from "./types";

export function getOrgChart(): Promise<OrgChartEmployee[]> {
    return apiFetch<OrgChartEmployee[]>("/employees/org-chart");
}

// Group views need department/location IDs, which the hierarchy endpoint omits.
export async function getOrgChartEmployees() {
    const firstPage = await getEmployees({ page: 1, pageSize: 500 });
    const employees = [...firstPage.items];
    let result = firstPage;

    while (employees.length < result.totalCount) {
        if (result.items.length === 0 || result.pageSize <= 0) {
            throw new Error("The employee directory is incomplete. Please try again.");
        }
        result = await getEmployees({ page: result.page + 1, pageSize: result.pageSize });
        employees.push(...result.items);
    }

    return employees;
}
