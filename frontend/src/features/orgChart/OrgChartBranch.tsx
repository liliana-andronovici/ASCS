import { useState } from "react";
import { Avatar } from "../../components/Avatar";
import type { OrgChartEmployee } from "./types";

export function EmployeeCard({ employee, root = false }: { employee: OrgChartEmployee; root?: boolean }) {
    return (
        <div className={"flex min-h-20 w-72 items-center gap-3 rounded-lg border p-4 text-left shadow-card " +
            (root ? "border-navy bg-navy text-white" : "border-gray-200 bg-white text-navy")}>
            <Avatar firstName={employee.firstName} lastName={employee.lastName} />
            <div className="min-w-0 flex-1">
                <div className="break-words text-sm font-semibold">{employee.firstName} {employee.lastName}</div>
                <div className={"break-words text-sm " + (root ? "text-white/70" : "text-gray-500")}>{employee.title}</div>
            </div>
            {employee.directReports.length > 0 && (
                <span className={"w-14 shrink-0 text-xs font-semibold " + (root ? "text-white/70" : "text-gray-500")}>
                    {employee.directReports.length} {employee.directReports.length === 1 ? "report" : "reports"}
                </span>
            )}
        </div>
    );
}

export function OrgChartBranch({ employee, root = false }: { employee: OrgChartEmployee; root?: boolean }) {
    const [expanded, setExpanded] = useState(true);
    const hasReports = employee.directReports.length > 0;
    const onlyLeaves = hasReports && employee.directReports.every((report) => report.directReports.length === 0);

    return (
        <li className="org-chart-branch">
            {hasReports ? (
                <button
                    type="button"
                    onClick={() => setExpanded((value) => !value)}
                    aria-expanded={expanded}
                    aria-label={(expanded ? "Collapse reports for " : "Expand reports for ") + employee.firstName + " " + employee.lastName}
                    className="cursor-pointer rounded-lg transition-shadow hover:shadow-md focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-coral"
                >
                    <EmployeeCard employee={employee} root={root} />
                </button>
            ) : <EmployeeCard employee={employee} root={root} />}
            {expanded && hasReports && (
                <ul className={onlyLeaves ? "org-chart-leaves" : "org-chart-children"}>
                    {employee.directReports.map((report) => (
                        <OrgChartBranch key={report.id} employee={report} />
                    ))}
                </ul>
            )}
        </li>
    );
}
