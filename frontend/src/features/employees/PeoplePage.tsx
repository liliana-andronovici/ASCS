import { useEffect, useMemo, useState } from "react";
import { Search } from "lucide-react";
import { Link } from "react-router-dom";
import { useEmployees, useEmployeeDirectory } from "./useEmployees";
import { useDepartments } from "../departments/useDepartments";
import { EmployeeFormModal } from "./EmployeeFormModal";
import { DeleteEmployeeModal } from "./DeleteEmployeeModal";
import { Avatar } from "../../components/Avatar";
import { Badge } from "../../components/Badge";
import type { BadgeVariant } from "../../components/Badge";
import { Checkbox } from "../../components/Checkbox";
import { Button } from "../../components/Button";
import type { Employee, EmployeeStatus } from "./types";

const STATUS_BADGE: Record<EmployeeStatus, BadgeVariant> = {
    Active: "active",
    Inactive: "inactive",
};

function formatStarted(dateIso: string) {
    return new Date(dateIso).toLocaleDateString("en-US", { month: "short", year: "numeric" });
}

export default function PeoplePage() {
    const [searchInput, setSearchInput] = useState("");
    const [search, setSearch] = useState("");
    const [departmentId, setDepartmentId] = useState<number | undefined>(undefined);
    const [status, setStatus] = useState<EmployeeStatus | undefined>("Active");
    const [page, setPage] = useState(1);
    const [view, setView] = useState<"table" | "grid">("table");
    const [selected, setSelected] = useState<Set<number>>(new Set());
    const [formOpen, setFormOpen] = useState(false);
    const [editingEmployee, setEditingEmployee] = useState<Employee | undefined>(undefined);
    const [deletingEmployee, setDeletingEmployee] = useState<Employee | null>(null);

    useEffect(() => {
        const timeout = setTimeout(() => {
            setSearch(searchInput);
            setPage(1);
        }, 300);
        return () => clearTimeout(timeout);
    }, [searchInput]);

    const { data, isLoading, isError, error } = useEmployees({
        search: search || undefined,
        departmentId,
        status,
        page,
    });
    const { data: departments, isError: isDepartmentsError } = useDepartments();
    const { data: directory } = useEmployeeDirectory();

    const departmentNameById = useMemo(() => {
        const map = new Map<number, string>();
        departments?.forEach((d) => map.set(Number(d.id), d.name));
        return map;
    }, [departments]);

    const managerNameById = useMemo(() => {
        const map = new Map<number, string>();
        directory?.items.forEach((e) => map.set(e.id, `${e.firstName} ${e.lastName}`));
        return map;
    }, [directory]);

    function openCreate() {
        setEditingEmployee(undefined);
        setFormOpen(true);
    }

    function openEdit(emp: Employee) {
        setEditingEmployee(emp);
        setFormOpen(true);
    }

    function toggleRow(id: number) {
        setSelected((prev) => {
            const next = new Set(prev);
            next.has(id) ? next.delete(id) : next.add(id);
            return next;
        });
    }

    function toggleAll() {
        if (!data) return;
        setSelected((prev) =>
            prev.size === data.items.length ? new Set() : new Set(data.items.map((e) => e.id))
        );
    }

    if (isLoading && !data) return <div className="p-6">Loading employees...</div>;
    if (isError) return <div className="p-6 text-red-600">Error: {(error as Error).message}</div>;
    if (!data) return null;

    const from = (data.page - 1) * data.pageSize + 1;
    const to = Math.min(data.page * data.pageSize, data.totalCount);

    return (
        <div className="p-6">
            <div className="rounded-lg border border-gray-200 bg-white shadow-card">
                <div className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
                    <h1 className="flex items-baseline gap-2 text-xl font-bold text-navy">
                        People <span className="text-base font-normal text-gray-400">{data.totalCount}</span>
                    </h1>
                    <div className="flex gap-2">
                        <Button onClick={openCreate} className="!w-auto px-4">
                            Add person
                        </Button>
                    </div>
                </div>

                <div className="flex flex-wrap items-center justify-between gap-3 px-6 py-4">
                    <div className="flex flex-wrap items-center gap-3">
                        <div className="flex items-center gap-2 rounded-full border border-gray-300 px-4 py-2 text-sm text-gray-500">
                            <Search size={16} />
                            <input
                                value={searchInput}
                                onChange={(e) => setSearchInput(e.target.value)}
                                placeholder="Search by name, title, email"
                                className="w-56 bg-transparent focus:outline-none"
                            />
                        </div>

                        <label className="flex items-center gap-1 rounded-full border border-gray-300 px-4 py-2 text-sm text-gray-500">
                            Department
                            <select
                                value={departmentId ?? ""}
                                onChange={(e) => {
                                    setDepartmentId(e.target.value ? Number(e.target.value) : undefined);
                                    setPage(1);
                                }}
                                className="bg-transparent font-semibold text-navy focus:outline-none"
                            >
                                <option value="">All</option>
                                {departments?.map((d) => (
                                    <option key={d.id} value={d.id}>
                                        {d.name}
                                    </option>
                                ))}
                            </select>
                        </label>

                        <span
                            title="Location filtering isn't available yet — no /api/locations endpoint."
                            className="flex items-center gap-1 rounded-full border border-gray-200 px-4 py-2 text-sm text-gray-300"
                        >
                            Location <span className="font-semibold">All</span>
                        </span>

                        <label
                            className={`flex items-center gap-1 rounded-full border px-4 py-2 text-sm ${
                                status ? "border-coral text-coral" : "border-gray-300 text-gray-500"
                            }`}
                        >
                            Status
                            <select
                                value={status ?? ""}
                                onChange={(e) => {
                                    setStatus((e.target.value || undefined) as EmployeeStatus | undefined);
                                    setPage(1);
                                }}
                                className="bg-transparent font-semibold focus:outline-none"
                            >
                                <option value="">All</option>
                                <option value="Active">Active</option>
                                <option value="Inactive">Inactive</option>
                            </select>
                        </label>
                    </div>

                    <div className="flex overflow-hidden rounded-full border border-gray-300 text-sm font-medium">
                        <button
                            onClick={() => setView("table")}
                            className={`px-4 py-2 ${view === "table" ? "bg-coral text-white" : "text-gray-500"}`}
                        >
                            Table
                        </button>
                        <button
                            disabled
                            title="Grid view isn't built yet"
                            className="cursor-not-allowed px-4 py-2 text-gray-300"
                        >
                            Grid
                        </button>
                    </div>
                </div>

                {isDepartmentsError && (
                    <div className="mx-6 mb-4 rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-700">
                        Couldn't load departments — department names won't show until this loads.
                    </div>
                )}

                <table className="w-full text-left text-sm">
                    <thead>
                    <tr className="border-y border-gray-200 text-gray-500">
                        <th className="w-10 py-3 pl-6 pr-4">
                            <Checkbox
                                checked={!!data.items.length && selected.size === data.items.length}
                                onChange={toggleAll}
                            />
                        </th>
                        <th className="py-3 font-medium">Name</th>
                        <th className="py-3 font-medium">Department</th>
                        <th className="py-3 font-medium">Location</th>
                        <th className="py-3 font-medium">Manager</th>
                        <th className="py-3 font-medium">Status</th>
                        <th className="py-3 font-medium">Started</th>
                        <th className="py-3 pr-6 font-medium"></th>
                    </tr>
                    </thead>
                    <tbody>
                    {data.items.map((emp) => (
                        <tr key={emp.id} className="group border-b border-gray-100 last:border-0 hover:bg-gray-50">
                            <td className="py-3 pl-6 pr-4">
                                <Checkbox checked={selected.has(emp.id)} onChange={() => toggleRow(emp.id)} />
                            </td>
                            <td className="py-3">
                                <div className="flex items-center gap-3">
                                    <Avatar firstName={emp.firstName} lastName={emp.lastName} />
                                    <div>
                                        <Link to={`/people/${emp.id}`} className="block font-semibold text-navy hover:underline">{emp.firstName} {emp.lastName}</Link>
                                        <div className="text-gray-500">{emp.title}</div>
                                    </div>
                                </div>
                            </td>
                            <td className="py-3 text-gray-700">
                                {emp.departmentId ? departmentNameById.get(emp.departmentId) ?? "—" : "—"}
                            </td>
                            <td className="py-3 text-gray-400">—</td>
                            <td className="py-3 text-gray-700">
                                {emp.managerId ? managerNameById.get(emp.managerId) ?? "—" : "—"}
                            </td>
                            <td className="py-3">
                                <Badge variant={STATUS_BADGE[emp.status]}>{emp.status}</Badge>
                            </td>
                            <td className="py-3 text-gray-500">{formatStarted(emp.startedOnUtc)}</td>
                            <td className="py-3 pr-6">
                                <div className="flex justify-end gap-3 opacity-0 group-hover:opacity-100">
                                    <button
                                        onClick={() => openEdit(emp)}
                                        className="text-xs font-medium text-navy hover:underline"
                                    >
                                        Edit
                                    </button>
                                    <button
                                        onClick={() => setDeletingEmployee(emp)}
                                        className="text-xs font-medium text-red-600 hover:underline"
                                    >
                                        Remove
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>

                <div className="flex items-center justify-between px-6 py-4 text-sm text-gray-500">
                    <span>
                        {data.totalCount === 0 ? "0 of 0" : `${from}-${to} of ${data.totalCount}`}
                    </span>
                    <div className="flex gap-2">
                        <Button
                            variant="secondary"
                            className="!w-auto px-4"
                            disabled={page <= 1}
                            onClick={() => setPage((p) => p - 1)}
                        >
                            Previous
                        </Button>
                        <Button
                            variant="secondary"
                            className="!w-auto px-4"
                            disabled={to >= data.totalCount}
                            onClick={() => setPage((p) => p + 1)}
                        >
                            Next
                        </Button>
                    </div>
                </div>
            </div>

            <EmployeeFormModal
                key={editingEmployee?.id ?? "new"}
                open={formOpen}
                onClose={() => setFormOpen(false)}
                employee={editingEmployee}
            />
            <DeleteEmployeeModal
                open={!!deletingEmployee}
                onClose={() => setDeletingEmployee(null)}
                employee={deletingEmployee}
            />
        </div>
    );
}