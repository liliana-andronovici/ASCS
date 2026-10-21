import { useState, type FormEvent } from "react";
import { useQuery } from "@tanstack/react-query";
import { getEmployees } from "../employees/api";
import { ApiError } from "../../lib/api";
import { useAddRoleEmployee, useCreateRole, useDeleteRole, usePermissions, useRemoveRoleEmployee, useRenameRole, useRoleEmployees, useRoles, useSetRolePermissions } from "./useRolePermissions";

function errorMessage(error: unknown, fallback: string): string {
    if (!(error instanceof ApiError)) return fallback;
    try {
        const body = JSON.parse(error.message) as { message?: string; errors?: string[] };
        return body.message ?? body.errors?.join(" ") ?? fallback;
    } catch {
        return fallback;
    }
}

export default function RolesPermissionsPage() {
    const rolesQuery = useRoles();
    const permissionsQuery = usePermissions();
    const createRole = useCreateRole();
    const setPermissions = useSetRolePermissions();
    const renameRole = useRenameRole();
    const deleteRole = useDeleteRole();
    const addEmployee = useAddRoleEmployee();
    const removeEmployee = useRemoveRoleEmployee();
    const [isCreatingRole, setIsCreatingRole] = useState(false);
    const [roleName, setRoleName] = useState("");
    const [roleNameError, setRoleNameError] = useState<string | null>(null);
    const [editingRoleId, setEditingRoleId] = useState<number | null>(null);
    const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);
    const [statusMessage, setStatusMessage] = useState<string | null>(null);
    const [selectedRoleId, setSelectedRoleId] = useState<number | null>(null);
    const [newRoleName, setNewRoleName] = useState("");
    const [employeeSearch, setEmployeeSearch] = useState("");
    const [confirmDelete, setConfirmDelete] = useState(false);
    const isLoading = rolesQuery.isLoading || permissionsQuery.isLoading;
    const isError = rolesQuery.isError || permissionsQuery.isError;
    const roles = rolesQuery.data ?? [];
    const permissions = permissionsQuery.data ?? [];
    const editingRole = roles.find((role) => role.id === editingRoleId);
    const selectedRole = roles.find((role) => role.id === selectedRoleId);
    const roleEmployees = useRoleEmployees(selectedRoleId);
    const employeeResults = useQuery({
        queryKey: ["role-employee-search", employeeSearch.trim()],
        queryFn: () => getEmployees({ search: employeeSearch.trim(), pageSize: 20 }),
        enabled: selectedRoleId !== null && employeeSearch.trim().length >= 2,
    });
    const hasChanges = editingRole !== undefined &&
        (selectedPermissions.length !== editingRole.permissions.length ||
            selectedPermissions.some((name) => !editingRole.permissions.includes(name)));

    function startEditing(roleId: number, currentPermissions: string[]) {
        setEditingRoleId(roleId);
        setSelectedPermissions(currentPermissions);
        setStatusMessage(null);
        setPermissions.reset();
    }

    function submitRole(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const name = roleName.trim();
        if (!name) {
            setRoleNameError("Enter a role name.");
            return;
        }
        setRoleNameError(null);
        createRole.mutate(name, {
            onSuccess: () => {
                setStatusMessage(`${name} was created.`);
                setRoleName("");
                setIsCreatingRole(false);
            },
        });
    }

    function togglePermission(name: string) {
        setSelectedPermissions((current) =>
            current.includes(name) ? current.filter((permission) => permission !== name) : [...current, name]
        );
    }

    function savePermissions() {
        if (!editingRole || !hasChanges) return;
        setPermissions.mutate(
            { roleId: editingRole.id, permissions: selectedPermissions },
            {
                onSuccess: () => {
                    setStatusMessage(`Permissions for ${editingRole.name} were saved. Sign in again to refresh affected users' access.`);
                    setEditingRoleId(null);
                },
            }
        );
    }

    function openRole(roleId: number, name: string) {
        renameRole.reset();
        deleteRole.reset();
        addEmployee.reset();
        removeEmployee.reset();
        setSelectedRoleId(roleId);
        setNewRoleName(name);
        setEmployeeSearch("");
        setConfirmDelete(false);
        setStatusMessage(null);
    }

    function submitRename(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!selectedRole) return;
        const name = newRoleName.trim();
        if (!name || name === selectedRole.name) return;
        renameRole.mutate({ roleId: selectedRole.id, name }, {
            onSuccess: () => setStatusMessage(`Role renamed to ${name}.`),
        });
    }

    function submitDelete() {
        if (!selectedRole) return;
        deleteRole.mutate(selectedRole.id, {
            onSuccess: () => {
                setStatusMessage(`${selectedRole.name} was deleted.`);
                setSelectedRoleId(null);
                setConfirmDelete(false);
            },
        });
    }

    return (
        <div className="p-5 md:p-6">
            <section className="overflow-hidden rounded-xl border border-[#dfe5ec] bg-white shadow-card">
                <div className="border-b border-[#dfe5ec] px-5 py-5">
                    <div className="flex flex-wrap items-start justify-between gap-4">
                        <div>
                            <h2 className="text-xl font-semibold text-[#16232e]">Roles & permissions</h2>
                            <p className="mt-1 text-sm text-[#6b7a89]">
                                {isLoading || isError
                                    ? "Review which permissions each role has."
                                    : `${roles.length} roles. Review which permissions each role has.`}
                            </p>
                        </div>
                        <button
                            type="button"
                            onClick={() => {
                                setIsCreatingRole(true);
                                setRoleNameError(null);
                                createRole.reset();
                            }}
                            disabled={isCreatingRole || isLoading || isError}
                            className="rounded-lg border border-[#cfd7e0] bg-white px-4 py-2 text-sm font-semibold text-[#16232e] disabled:opacity-50"
                        >
                            New role
                        </button>
                    </div>
                </div>
                {isCreatingRole && (
                    <form onSubmit={submitRole} className="flex flex-wrap items-end gap-3 border-b border-[#dfe5ec] px-5 py-4">
                        <div className="min-w-[220px] flex-1">
                            <label htmlFor="new-role-name" className="mb-1 block text-sm font-medium text-[#16232e]">
                                Role name
                            </label>
                            <input
                                id="new-role-name"
                                value={roleName}
                                onChange={(event) => {
                                    setRoleName(event.target.value);
                                    setRoleNameError(null);
                                }}
                                maxLength={100}
                                disabled={createRole.isPending}
                                className="w-full rounded-lg border border-[#cfd7e0] px-3 py-2 text-sm text-[#16232e]"
                            />
                        </div>
                        <button
                            type="button"
                            onClick={() => {
                                setIsCreatingRole(false);
                                setRoleName("");
                                setRoleNameError(null);
                            }}
                            disabled={createRole.isPending}
                            className="rounded-lg border border-[#cfd7e0] px-4 py-2 text-sm text-[#16232e] disabled:opacity-50"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={createRole.isPending}
                            className="rounded-lg bg-coral px-4 py-2 text-sm font-semibold text-white disabled:opacity-50"
                        >
                            {createRole.isPending ? "Creating..." : "Create role"}
                        </button>
                        {(roleNameError || createRole.isError) && (
                            <p role="alert" className="w-full text-sm text-[#b93f38]">
                                {roleNameError ?? errorMessage(createRole.error, "Could not create the role.")}
                            </p>
                        )}
                    </form>
                )}
                {statusMessage && (
                    <div role="status" className="flex flex-wrap items-center justify-between gap-3 border-b border-[#dfe5ec] bg-[#f8fafc] px-5 py-3 text-sm text-[#16232e]">
                        <span>{statusMessage}</span>
                        <button
                            type="button"
                            onClick={() => void Promise.all([rolesQuery.refetch(), permissionsQuery.refetch()])}
                            disabled={rolesQuery.isFetching || permissionsQuery.isFetching}
                            className="font-semibold text-[#b93f38] hover:underline disabled:opacity-50"
                        >
                            Refresh
                        </button>
                    </div>
                )}
                {isLoading ? (
                    <p role="status" className="p-5 text-sm text-[#6b7a89]">
                        Loading roles and permissions...
                    </p>
                ) : isError ? (
                    <p role="alert" className="p-5 text-sm text-[#b93f38]">
                        {rolesQuery.error instanceof ApiError && rolesQuery.error.status === 403
                            ? "Your account cannot manage roles and permissions."
                            : "Could not load roles and permissions."}
                    </p>
                ) : roles.length === 0 || permissions.length === 0 ? (
                    <p className="p-5 text-sm text-[#6b7a89]">
                        No roles or permissions available.
                    </p>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-[640px] w-full text-left text-sm">
                            <caption className="sr-only">Permissions assigned to each role</caption>
                            <thead className="bg-[#f8fafc] text-[#6b7a89]">
                                <tr>
                                    <th scope="col" className="w-[34%] px-5 py-3 font-semibold">
                                        Permission
                                    </th>
                                    {roles.map((role) => (
                                        <th key={role.id} scope="col" className="px-4 py-3 font-semibold text-[#16232e]">
                                            <div className="flex items-center justify-between gap-2">
                                                <span className="flex flex-col">
                                                    <span>{role.name}</span>
                                                    <span className="text-xs font-normal text-[#6b7a89]">
                                                        {role.employeeCount} {role.employeeCount === 1 ? "person" : "people"}
                                                    </span>
                                                </span>
                                                <span className="flex gap-2">
                                                    <button
                                                        type="button"
                                                        onClick={() => startEditing(role.id, role.permissions)}
                                                        disabled={setPermissions.isPending}
                                                        className="text-xs font-medium text-[#b93f38] hover:underline disabled:opacity-50"
                                                        aria-label={`Edit permissions for ${role.name}`}
                                                    >
                                                        Edit
                                                    </button>
                                                    <button
                                                        type="button"
                                                        onClick={() => openRole(role.id, role.name)}
                                                        className="text-xs font-medium text-[#b93f38] hover:underline"
                                                        aria-label={`Manage ${role.name} role and employees`}
                                                    >
                                                        Manage
                                                    </button>
                                                </span>
                                            </div>
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {permissions.map((permission) => (
                                    <tr key={permission.id} className="border-t border-[#e9edf2]">
                                        <th scope="row" className="px-5 py-3 font-semibold text-[#16232e]">
                                            {permission.name}
                                        </th>
                                        {roles.map((role) => {
                                            const granted = (editingRoleId === role.id ? selectedPermissions : role.permissions)
                                                .includes(permission.name);
                                            return (
                                                <td key={role.id} className="px-4 py-3">
                                                    {editingRoleId === role.id ? (
                                                        <input
                                                            type="checkbox"
                                                            checked={granted}
                                                            onChange={() => togglePermission(permission.name)}
                                                            disabled={setPermissions.isPending}
                                                            aria-label={`${permission.name} for ${role.name}`}
                                                            className="size-[18px] accent-[#e35d56]"
                                                        />
                                                    ) : (
                                                        <span
                                                            role="img"
                                                            aria-label={`${permission.name} ${granted ? "granted" : "not granted"} for ${role.name}`}
                                                            className={`grid size-[18px] place-items-center rounded-[5px] border text-[11px] ${
                                                                granted
                                                                    ? "border-coral bg-coral text-white"
                                                                    : "border-[#cfd7e0] bg-white"
                                                            }`}
                                                        >
                                                            {granted ? "✓" : ""}
                                                        </span>
                                                    )}
                                                </td>
                                            );
                                        })}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
                {editingRole && hasChanges && (
                    <div role="status" className="mx-5 mt-4 rounded-lg bg-[#fdecea] px-4 py-3 text-sm text-[#16232e]">
                        <strong className="text-[#b93f38]">Unsaved:</strong> Permission changes for {editingRole.name}.
                        {` ${editingRole.employeeCount} ${editingRole.employeeCount === 1 ? "person has" : "people have"} this role.`}
                    </div>
                )}
                {editingRole && (
                    <div className="flex flex-wrap items-center gap-3 border-t border-[#dfe5ec] px-5 py-4">
                        <span className="mr-auto text-sm text-[#16232e]">Editing {editingRole.name}</span>
                        {setPermissions.isError && (
                            <span role="alert" className="text-sm text-[#b93f38]">
                                {errorMessage(setPermissions.error, "Could not save the permission changes.")}
                            </span>
                        )}
                        <button
                            type="button"
                            onClick={() => setEditingRoleId(null)}
                            disabled={setPermissions.isPending}
                            className="rounded-lg border border-[#cfd7e0] px-4 py-2 text-sm text-[#16232e] disabled:opacity-50"
                        >
                            Cancel
                        </button>
                        <button
                            type="button"
                            onClick={savePermissions}
                            disabled={!hasChanges || setPermissions.isPending}
                            className="rounded-lg bg-coral px-4 py-2 text-sm font-semibold text-white disabled:opacity-50"
                        >
                            {setPermissions.isPending ? "Saving..." : "Save changes"}
                        </button>
                    </div>
                )}
            </section>
            {selectedRole && (
                <section className="mt-5 rounded-xl border border-[#dfe5ec] bg-white p-5 shadow-card" aria-label={`Manage ${selectedRole.name}`}>
                    <div className="flex items-start justify-between gap-4">
                        <div>
                            <h3 className="text-lg font-semibold text-[#16232e]">Manage {selectedRole.name}</h3>
                            <p className="text-sm text-[#6b7a89]">Assign employees and update this role.</p>
                        </div>
                        <button type="button" onClick={() => setSelectedRoleId(null)} className="text-sm text-[#b93f38] hover:underline">Close</button>
                    </div>

                    <div className="mt-5 flex flex-wrap items-end gap-4 border-b border-[#e9edf2] pb-5">
                        <form onSubmit={submitRename} className="flex flex-wrap items-end gap-2">
                            <div>
                                <label htmlFor="rename-role" className="mb-1 block text-sm font-medium">Role name</label>
                                <input id="rename-role" value={newRoleName} onChange={(event) => setNewRoleName(event.target.value)} maxLength={100} required disabled={renameRole.isPending} className="rounded-lg border border-[#cfd7e0] px-3 py-2 text-sm" />
                            </div>
                            <button type="submit" disabled={renameRole.isPending || !newRoleName.trim() || newRoleName.trim() === selectedRole.name} className="rounded-lg border border-[#cfd7e0] px-3 py-2 text-sm font-semibold disabled:opacity-50">Rename</button>
                        </form>
                        {confirmDelete ? (
                            <div className="flex items-center gap-2 text-sm">
                                <span>Delete {selectedRole.name} and remove it from {selectedRole.employeeCount} {selectedRole.employeeCount === 1 ? "person" : "people"}?</span>
                                <button type="button" onClick={submitDelete} disabled={deleteRole.isPending} className="rounded-lg bg-[#b93f38] px-3 py-2 font-semibold text-white disabled:opacity-50">Confirm delete</button>
                                <button type="button" onClick={() => setConfirmDelete(false)} className="px-2 py-2">Cancel</button>
                            </div>
                        ) : (
                            <button type="button" onClick={() => setConfirmDelete(true)} className="rounded-lg border border-[#eab7b4] px-3 py-2 text-sm text-[#b93f38]">Delete role</button>
                        )}
                    </div>
                    {(renameRole.isError || deleteRole.isError) && (
                        <p role="alert" className="mt-3 text-sm text-[#b93f38]">{errorMessage(renameRole.error ?? deleteRole.error, "Could not update this role.")}</p>
                    )}

                    <div className="mt-5 grid gap-6 md:grid-cols-2">
                        <div>
                            <h4 className="font-semibold text-[#16232e]">Assigned employees</h4>
                            {roleEmployees.isLoading && <p role="status" className="mt-2 text-sm">Loading employees...</p>}
                            {roleEmployees.isError && <p role="alert" className="mt-2 text-sm text-[#b93f38]">Could not load assigned employees.</p>}
                            {roleEmployees.data?.length === 0 && <p className="mt-2 text-sm text-[#6b7a89]">No employees assigned.</p>}
                            <ul className="mt-2 space-y-2">
                                {roleEmployees.data?.map((employee) => (
                                    <li key={employee.id} className="flex items-center justify-between gap-3 rounded-lg border border-[#e9edf2] px-3 py-2 text-sm">
                                        <span>{employee.firstName} {employee.lastName}<span className="block text-xs text-[#6b7a89]">{employee.email}</span></span>
                                        <button type="button" onClick={() => removeEmployee.mutate({ roleId: selectedRole.id, employeeId: employee.id })} disabled={removeEmployee.isPending} className="text-[#b93f38] disabled:opacity-50">Remove</button>
                                    </li>
                                ))}
                            </ul>
                        </div>
                        <div>
                            <label htmlFor="role-employee-search" className="block font-semibold text-[#16232e]">Assign an employee</label>
                            <input id="role-employee-search" value={employeeSearch} onChange={(event) => setEmployeeSearch(event.target.value)} placeholder="Search by name or email" className="mt-2 w-full rounded-lg border border-[#cfd7e0] px-3 py-2 text-sm" />
                            {employeeSearch.trim().length >= 2 && (
                                <div className="mt-2 text-sm">
                                    {employeeResults.isLoading && <p role="status">Searching...</p>}
                                    {employeeResults.isError && <p role="alert" className="text-[#b93f38]">Could not search employees.</p>}
                                    {employeeResults.data?.items.length === 0 && <p>No matching employees.</p>}
                                    <ul className="space-y-2">
                                        {employeeResults.data?.items.filter((employee) => !roleEmployees.data?.some((assigned) => assigned.id === employee.id)).map((employee) => (
                                            <li key={employee.id} className="flex items-center justify-between gap-3 rounded-lg border border-[#e9edf2] px-3 py-2">
                                                <span>{employee.firstName} {employee.lastName}<span className="block text-xs text-[#6b7a89]">{employee.email}</span></span>
                                                <button type="button" onClick={() => addEmployee.mutate({ roleId: selectedRole.id, employeeId: employee.id })} disabled={addEmployee.isPending} className="font-semibold text-[#b93f38] disabled:opacity-50">Assign</button>
                                            </li>
                                        ))}
                                    </ul>
                                    {(employeeResults.data?.totalCount ?? 0) > 20 && <p className="mt-2 text-[#6b7a89]">Showing the first 20 matches. Refine your search to find someone else.</p>}
                                </div>
                            )}
                        </div>
                    </div>
                    {(addEmployee.isError || removeEmployee.isError) && (
                        <p role="alert" className="mt-3 text-sm text-[#b93f38]">{errorMessage(addEmployee.error ?? removeEmployee.error, "Could not change this employee's role.")}</p>
                    )}
                    {(addEmployee.isSuccess || removeEmployee.isSuccess) && <p role="status" className="mt-3 text-sm text-[#16232e]">Assignment saved. The employee must sign in again for access changes to take effect.</p>}
                </section>
            )}
        </div>
    );
}
