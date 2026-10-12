import { useState, type FormEvent } from "react";
import { useCreateRole, usePermissions, useRoles, useSetRolePermissions } from "./useRolePermissions";

export default function RolesPermissionsPage() {
    const rolesQuery = useRoles();
    const permissionsQuery = usePermissions();
    const createRole = useCreateRole();
    const setPermissions = useSetRolePermissions();
    const [isCreatingRole, setIsCreatingRole] = useState(false);
    const [roleName, setRoleName] = useState("");
    const [roleNameError, setRoleNameError] = useState<string | null>(null);
    const [editingRoleId, setEditingRoleId] = useState<number | null>(null);
    const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);
    const [statusMessage, setStatusMessage] = useState<string | null>(null);
    const isLoading = rolesQuery.isLoading || permissionsQuery.isLoading;
    const isError = rolesQuery.isError || permissionsQuery.isError;
    const roles = rolesQuery.data ?? [];
    const permissions = permissionsQuery.data ?? [];
    const editingRole = roles.find((role) => role.id === editingRoleId);
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
                setStatusMessage(`${name} was queued for creation. Reload later to confirm it was added.`);
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
                    setStatusMessage(`Changes for ${editingRole.name} were queued. Reload later to confirm they were applied.`);
                    setEditingRoleId(null);
                },
            }
        );
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
                                {roleNameError ?? "Could not queue the new role."}
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
                        Could not load roles and permissions.
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
                                                <button
                                                    type="button"
                                                    onClick={() => startEditing(role.id, role.permissions)}
                                                    disabled={setPermissions.isPending}
                                                    className="text-xs font-medium text-[#b93f38] hover:underline disabled:opacity-50"
                                                    aria-label={`Edit permissions for ${role.name}`}
                                                >
                                                    Edit
                                                </button>
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
                                Could not queue the permission changes.
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
        </div>
    );
}
