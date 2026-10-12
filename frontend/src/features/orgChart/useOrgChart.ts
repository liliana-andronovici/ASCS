import { useQuery } from "@tanstack/react-query";
import { ApiError } from "../../lib/api";
import { getDepartments } from "../departments/api";
import { getLocations } from "../locations/api";
import { getOrgChart, getOrgChartEmployees } from "./api";

export type OrgChartView = "reporting" | "departments" | "locations";

export function useOrgChart(view: OrgChartView) {
    const hierarchy = useQuery({
        queryKey: ["employees", "org-chart"],
        queryFn: getOrgChart,
        enabled: view === "reporting",
        retry: (count, error) => !(error instanceof ApiError && [401, 403].includes(error.status)) && count < 1,
    });
    const employees = useQuery({
        queryKey: ["employees", "org-chart-groups"],
        queryFn: getOrgChartEmployees,
        enabled: view !== "reporting",
    });
    const departments = useQuery({
        queryKey: ["departments"],
        queryFn: getDepartments,
        enabled: view === "departments",
    });
    const locations = useQuery({
        queryKey: ["locations"],
        queryFn: getLocations,
        enabled: view === "locations",
    });

    return { hierarchy, employees, departments, locations };
}
