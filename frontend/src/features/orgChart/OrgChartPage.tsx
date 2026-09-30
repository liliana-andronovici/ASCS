import { useMemo, useState } from "react";
import { Plus, Minus } from "lucide-react";
import { useEmployeeDirectory } from "../employees/useEmployees";
import { Avatar } from "../../components/Avatar";
import type { Employee } from "../employees/types";

type OrgView = "reporting" | "departments" | "locations";

function initialsOf(e: Employee) {
    return `${e.firstName[0] ?? ""}${e.lastName[0] ?? ""}`.toUpperCase();
}

export default function OrgChartPage() {
    const { data, isLoading, isError } = useEmployeeDirectory();
    const [view, setView] = useState<OrgView>("reporting");

    const { root, managers, reportsByManager } = useMemo(() => {
        const employees = data?.items ?? [];
        const active = employees.filter((e) => e.status === "Active");

        const root = active.find((e) => e.managerId == null);
        const managers = root ? active.filter((e) => e.managerId === root.id) : [];

        const reportsByManager = new Map<number, Employee[]>();
        managers.forEach((m) => {
            reportsByManager.set(m.id, active.filter((e) => e.managerId === m.id));
        });

        return { root, managers, reportsByManager };
    }, [data]);

    if (isLoading) return <div className="p-6 text-sm text-gray-500">Loading org chart...</div>;
    if (isError) return <div className="p-6 text-sm text-red-600">Couldn't load the org chart.</div>;

    return (
        <div className="flex min-h-full flex-col bg-white">
            <div className="flex h-[60px] items-center justify-between border-b border-gray-200 px-6">
                <h1 className="text-xl font-bold text-navy">Org chart</h1>

                <div className="flex items-center gap-3">
                    <div className="flex overflow-hidden rounded-full border border-gray-300 text-sm font-medium">
                        <button
                            onClick={() => setView("reporting")}
                            className={`px-4 py-1.5 ${view === "reporting" ? "bg-coral text-white" : "text-gray-500"}`}
                        >
                            Reporting line
                        </button>
                        <button
                            disabled
                            title="Departments view isn't built yet"
                            className="cursor-not-allowed px-4 py-1.5 text-gray-300"
                        >
                            Departments
                        </button>
                        <button
                            disabled
                            title="Locations view isn't built yet"
                            className="cursor-not-allowed px-4 py-1.5 text-gray-300"
                        >
                            Locations
                        </button>
                    </div>

                    <button
                        disabled
                        title="Zoom isn't built yet"
                        className="grid h-8 w-8 cursor-not-allowed place-items-center rounded-md border border-gray-300 text-gray-300"
                    >
                        <Plus size={16} />
                    </button>
                    <button
                        disabled
                        title="Zoom isn't built yet"
                        className="grid h-8 w-8 cursor-not-allowed place-items-center rounded-md border border-gray-300 text-gray-300"
                    >
                        <Minus size={16} />
                    </button>
                </div>
            </div>

            <div
                className="flex flex-1 flex-col items-center overflow-auto px-6 py-9"
                style={{
                    backgroundImage:
                        "linear-gradient(rgba(22,35,46,.05) 1px,transparent 1px),linear-gradient(90deg,rgba(22,35,46,.05) 1px,transparent 1px)",
                    backgroundSize: "24px 24px",
                }}
            >
                {!root ? (
                    <div className="py-12 text-sm text-gray-500">
                        No one is set up without a manager yet, so the chart has no starting point.
                    </div>
                ) : (
                    <>
                        <div className="flex w-56 items-center gap-2.5 rounded-xl bg-navy px-3.5 py-3 text-white">
                            <span className="grid h-9 w-9 shrink-0 place-items-center rounded-full bg-white text-xs font-extrabold text-navy">
                                {initialsOf(root)}
                            </span>
                            <span className="text-[13px] leading-tight">
                                <strong className="block">
                                    {root.firstName} {root.lastName}
                                </strong>
                                <span className="opacity-70">{root.title}</span>
                            </span>
                        </div>

                        <span className="h-7 w-0.5 bg-gray-200" />
                        <div className="h-0.5 w-full max-w-4xl bg-gray-200" />

                        {managers.length === 0 ? (
                            <div className="py-8 text-sm text-gray-500">
                                {root.firstName} has no direct reports yet.
                            </div>
                        ) : (
                            <div className="grid w-full max-w-6xl grid-cols-[repeat(auto-fit,minmax(220px,1fr))] gap-6">
                                {managers.map((m) => {
                                    const reports = reportsByManager.get(m.id) ?? [];
                                    return (
                                        <div key={m.id} className="flex flex-col items-center">
                                            <span className="h-6 w-0.5 bg-gray-200" />
                                            <div className="flex w-full items-center gap-2.5 rounded-xl border border-gray-200 bg-white px-3.5 py-3 shadow-sm">
                                                <Avatar firstName={m.firstName} lastName={m.lastName} />
                                                <span className="min-w-0 text-[13px] leading-tight">
                                                    <strong className="block truncate">
                                                        {m.firstName} {m.lastName}
                                                    </strong>
                                                    <span className="text-gray-500">{m.title}</span>
                                                </span>
                                                <span className="ml-auto text-[11px] font-extrabold text-gray-500">
                                                    {reports.length}
                                                </span>
                                            </div>

                                            {reports.length > 0 && (
                                                <>
                                                    <span className="h-5 w-0.5 bg-gray-200" />
                                                    <div className="flex w-full flex-col gap-1.5">
                                                        {reports.map((r) => (
                                                            <div
                                                                key={r.id}
                                                                className="flex items-center gap-2 rounded-[10px] border border-gray-100 bg-white px-3 py-2.5 text-xs leading-tight"
                                                            >
                                                                <Avatar firstName={r.firstName} lastName={r.lastName} size="sm" />
                                                                <span>
                                                                    <strong className="block">
                                                                        {r.firstName} {r.lastName}
                                                                    </strong>
                                                                    <span className="text-gray-500">{r.title}</span>
                                                                </span>
                                                            </div>
                                                        ))}
                                                    </div>
                                                </>
                                            )}
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </>
                )}
            </div>
        </div>
    );
}
