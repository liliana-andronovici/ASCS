import { useMemo, useState } from "react";
import { DirectoryTable } from "../../components/DirectoryTable";
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
        <div className="p-6">
            <DirectoryTable
                title="Departments"
                addLabel="Add department"
                items={departments}
                emptyMessage="No departments yet. Add a department to get started."
                employeeCount={(department) => isEmployeesError ? "—" : !directory ? "Loading..." : (
                    <>{employeeCountByDepartmentId.get(department.id) ?? 0}{isDirectoryIncomplete ? "+" : ""}</>
                )}
                onAdd={openCreate}
                onEdit={openEdit}
                onDelete={setDeletingDepartment}
            >
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
            </DirectoryTable>

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
