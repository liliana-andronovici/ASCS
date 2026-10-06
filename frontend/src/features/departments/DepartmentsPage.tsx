import { useMemo, useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { Button } from "../../components/Button";
import { useEmployeeDirectory } from "../employees/useEmployees";
import { useDepartments } from "./useDepartments";
import { DepartmentFormModal } from "./DepartmentFormModal";
import { DeleteDepartmentModal } from "./DeleteDepartmentModal";
import type { Department } from "./types";

export default function DepartmentsPage() {
    const [formOpen, setFormOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState<Department | undefined>(undefined);
    const [deletingDepartment, setDeletingDepartment] = useState<Department | null>(null);

    const { data: departments, isLoading, isError, error } = useDepartments();
    const { data: directory, isError: isEmployeesError } = useEmployeeDirectory();

    const employeeCountByDepartmentId = useMemo(() => {
        const counts = new Map<number, number>();
        directory?.items.forEach((employee) => {
            if (employee.departmentId !== null) {
                counts.set(employee.departmentId, (counts.get(employee.departmentId) ?? 0) + 1);
            }
        });
        return counts;
    }, [directory]);

    const isDirectoryIncomplete = !!directory && directory.items.length < directory.totalCount;

    function openCreate() {
        setEditingDepartment(undefined);
        setFormOpen(true);
    }

    function openEdit(department: Department) {
        setEditingDepartment(department);
        setFormOpen(true);
    }

    if (isLoading && !departments) return <div className="p-6">Loading departments...</div>;
    if (isError) return <div role="alert" className="p-6 text-red-600">Error: {error.message}</div>;
    if (!departments) return null;

    return (
        <div className="min-h-full bg-white">
            <header className="flex flex-wrap items-center justify-between gap-3 border-b border-gray-200 px-6 py-4">
                <h1 className="text-2xl font-semibold text-navy">Departments</h1>
                <Button onClick={openCreate} className="!w-auto flex items-center gap-2 px-4">
                    <Plus size={18} aria-hidden="true" />
                    Add department
                </Button>
            </header>

            <div className="p-6">
                {isEmployeesError && (
                    <div role="alert" className="mb-4 rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-700">
                        Couldn't load employees — employee counts are unavailable.
                    </div>
                )}
                {!isEmployeesError && isDirectoryIncomplete && (
                    <div role="status" className="mb-4 rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-700">
                        Only part of the employee directory is available. Counts marked + are minimums.
                    </div>
                )}

                <div className="overflow-x-auto rounded-lg border border-gray-200">
                    <table className="w-full min-w-[600px] text-left text-sm text-navy">
                        <thead>
                        <tr className="border-b border-gray-200 bg-gray-50">
                            <th scope="col" className="px-4 py-4 font-semibold">Name</th>
                            <th scope="col" className="w-48 px-4 py-4 font-semibold">No. of employees</th>
                            <th scope="col" className="w-24 px-4 py-4 text-center font-semibold">Modify</th>
                            <th scope="col" className="w-24 px-4 py-4 text-center font-semibold">Delete</th>
                        </tr>
                        </thead>
                        <tbody>
                        {departments.map((department) => (
                            <tr
                                key={department.id}
                                className="border-b border-gray-100 last:border-0 transition-shadow duration-200 hover:shadow-card"
                            >
                                <td className="px-4 py-4 font-semibold">{department.name}</td>
                                <td className="px-4 py-4">
                                    {isEmployeesError ? "—" : !directory ? "Loading..." : (
                                        <>
                                            {employeeCountByDepartmentId.get(department.id) ?? 0}
                                            {isDirectoryIncomplete ? "+" : ""}
                                        </>
                                    )}
                                </td>
                                <td className="px-4 py-4">
                                    <Button
                                        variant="secondary"
                                        onClick={() => openEdit(department)}
                                        className="!w-auto flex h-12 w-12 items-center justify-center !border-gray-200 !p-3 mx-auto"
                                        aria-label={"Edit " + department.name}
                                    >
                                        <Pencil size={20} aria-hidden="true" />
                                    </Button>
                                </td>
                                <td className="px-4 py-4">
                                    <Button
                                        variant="secondary"
                                        onClick={() => setDeletingDepartment(department)}
                                        className="!w-auto flex h-12 w-12 items-center justify-center !border-red-200 !p-3 !text-coral mx-auto"
                                        aria-label={"Delete " + department.name}
                                    >
                                        <Trash2 size={20} aria-hidden="true" />
                                    </Button>
                                </td>
                            </tr>
                        ))}
                        {departments.length === 0 && (
                            <tr>
                                <td colSpan={4} className="px-4 py-8 text-center text-gray-500">
                                    No departments yet. Add a department to get started.
                                </td>
                            </tr>
                        )}
                        </tbody>
                    </table>
                </div>
            </div>

            {formOpen && (
                <DepartmentFormModal
                    key={editingDepartment?.id ?? "new"}
                    open={formOpen}
                    onClose={() => setFormOpen(false)}
                    department={editingDepartment}
                />
            )}
            {deletingDepartment && (
                <DeleteDepartmentModal
                    open={!!deletingDepartment}
                    onClose={() => setDeletingDepartment(null)}
                    department={deletingDepartment}
                />
            )}
        </div>
    );
}
