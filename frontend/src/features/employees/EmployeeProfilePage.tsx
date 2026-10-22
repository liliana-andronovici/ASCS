import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useParams, useSearchParams } from "react-router-dom";
import { ChevronLeft } from "lucide-react";
import { Badge } from "../../components/Badge";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { Select } from "../../components/Select";
import { ApiError } from "../../lib/api";
import { getEquipment } from "../equipment/api";
import { addEmployeeEquipment, getEmployeeDocuments, getEmployeeEquipment, getEmployeeHistory, getEmployeeProfile, patchEmployeeProfile } from "./api";

function canManageEmployees(): boolean {
    const token = localStorage.getItem("accessToken");
    if (!token) return false;
    try {
        const payload = JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
        const permissions = payload.permissions;
        return Array.isArray(permissions) ? permissions.includes("ManageEmployee") : permissions === "ManageEmployee";
    } catch {
        return false;
    }
}

function formatDate(value: string): string {
    return new Date(value).toLocaleDateString(undefined, {
        day: "numeric",
        month: "short",
        year: "numeric",
    });
}

export default function EmployeeProfilePage() {
    const queryClient = useQueryClient();
    const [selectedEquipmentId, setSelectedEquipmentId] = useState("");
    const [assignedOn, setAssignedOn] = useState(new Date().toISOString().slice(0, 10));
    const [assigned, setAssigned] = useState(false);
    const [editing, setEditing] = useState(false);
    const [profileForm, setProfileForm] = useState({ phone: "", desk: "", employeeCode: "", employmentType: "", contractType: "", weeklyHours: "", languages: "" });
    const { employeeId: rawEmployeeId } = useParams();
    const employeeId = Number(rawEmployeeId);
    const validEmployeeId = Number.isInteger(employeeId) && employeeId > 0;
    const [searchParams, setSearchParams] = useSearchParams();
    const tab = searchParams.get("tab");
    const activeTab = tab === "assets" || tab === "equipment" ? "equipment" : tab === "documents" || tab === "history" ? tab : "overview";

    const employeeQuery = useQuery({
        queryKey: ["employees", employeeId],
        queryFn: () => getEmployeeProfile(employeeId),
        enabled: validEmployeeId,
    });
    const equipmentQuery = useQuery({
        queryKey: ["employees", employeeId, "equipment"],
        queryFn: () => getEmployeeEquipment(employeeId),
        enabled: validEmployeeId && activeTab === "equipment" && employeeQuery.isSuccess,
    });
    const catalogQuery = useQuery({
        queryKey: ["equipment"],
        queryFn: getEquipment,
        enabled: validEmployeeId && activeTab === "equipment" && employeeQuery.isSuccess,
    });
    const documentsQuery = useQuery({
        queryKey: ["employees", employeeId, "documents"],
        queryFn: () => getEmployeeDocuments(employeeId),
        enabled: validEmployeeId && activeTab === "documents" && employeeQuery.isSuccess,
    });
    const historyQuery = useQuery({
        queryKey: ["employees", employeeId, "history"],
        queryFn: () => getEmployeeHistory(employeeId),
        enabled: validEmployeeId && activeTab === "history" && employeeQuery.isSuccess,
    });
    const saveProfile = useMutation({
        mutationFn: () => patchEmployeeProfile(employeeId, {
            profileVersion: employeeQuery.data!.profileVersion,
            phone: profileForm.phone || null,
            desk: profileForm.desk || null,
            employeeCode: profileForm.employeeCode || null,
            employmentType: profileForm.employmentType || null,
            contractType: profileForm.contractType || null,
            weeklyHours: profileForm.weeklyHours === "" ? null : Number(profileForm.weeklyHours),
            languages: profileForm.languages.split(",").map((value) => value.trim()).filter(Boolean),
        }),
        onSuccess: async () => {
            setEditing(false);
            await queryClient.invalidateQueries({ queryKey: ["employees", employeeId] });
        },
    });

    function startEditing() {
        const employee = employeeQuery.data;
        if (!employee) return;
        setProfileForm({
            phone: employee.phone ?? "", desk: employee.desk ?? "", employeeCode: employee.employeeCode ?? "",
            employmentType: employee.employmentType ?? "", contractType: employee.contractType ?? "",
            weeklyHours: employee.weeklyHours == null ? "" : String(employee.weeklyHours),
            languages: employee.languages.join(", "),
        });
        saveProfile.reset();
        setEditing(true);
    }
    const assignMutation = useMutation({
        mutationFn: () => addEmployeeEquipment(employeeId, { equipmentId: Number(selectedEquipmentId), assignedOn }),
        onSuccess: async () => {
            setSelectedEquipmentId("");
            setAssigned(true);
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ["employees", employeeId, "equipment"] }),
                queryClient.invalidateQueries({ queryKey: ["equipment"] }),
            ]);
        },
    });

    function assign(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!selectedEquipmentId || !assignedOn) return;
        setAssigned(false);
        assignMutation.mutate();
    }

    if (!validEmployeeId) {
        return <p role="alert" className="p-6 text-sm text-red-600">Invalid employee ID.</p>;
    }

    if (employeeQuery.isPending) {
        return <p role="status" className="p-6 text-sm text-gray-500">Loading employee...</p>;
    }

    if (employeeQuery.isError) {
        return (
            <div className="p-6 text-sm text-red-600" role="alert">
                {employeeQuery.error instanceof ApiError && employeeQuery.error.status === 404
                    ? "Employee not found."
                    : "Could not load this employee."}
            </div>
        );
    }

    const employee = employeeQuery.data;
    const initials = `${employee.firstName[0] ?? ""}${employee.lastName[0] ?? ""}`.toUpperCase();
    const place = [employee.departmentName, employee.locationName].filter(Boolean).join(" · ");

    return (
        <div className="flex min-h-full flex-col bg-white">
            <div className="flex h-[60px] items-center justify-between border-b border-gray-200 px-6">
                <Link to="/people" className="flex items-center gap-1 text-sm text-gray-500 hover:text-navy"><ChevronLeft size={16} /> People</Link>
                {canManageEmployees() && !editing && <Button type="button" fullWidth={false} onClick={() => { setSearchParams({}); startEditing(); }}>Edit profile</Button>}
            </div>
            <section className="flex-1">
                <div className="flex flex-wrap items-center gap-6 border-b border-gray-200 px-6 py-7">
                    <div className="flex h-28 w-28 flex-none items-center justify-center rounded-full bg-navy text-3xl font-bold text-white">{initials}</div>
                    <div className="min-w-0 flex-1">
                        {place && <p className="mb-1 text-[11px] uppercase tracking-[0.1em] text-gray-500">{place}</p>}
                        <h1 className="text-[32px] font-bold leading-tight text-navy">{employee.firstName} {employee.lastName}</h1>
                        <p className="mt-1 text-sm text-gray-500">{employee.title}{employee.manager && <> · reports to <Link to={`/people/${employee.manager.id}`} className="text-coral hover:underline">{employee.manager.firstName} {employee.manager.lastName}</Link></>}</p>
                    </div>
                    <div className="flex items-center gap-3"><Badge variant={employee.status === "Active" ? "active" : "inactive"}>{employee.status}</Badge><span className="text-sm text-gray-500">Since {new Date(employee.startedOnUtc).toLocaleDateString("en-US", { month: "short", year: "numeric" })}</span></div>
                </div>

                <div role="tablist" aria-label="Employee profile sections" className="flex gap-6 border-b border-gray-200 px-6">
                    <button
                        type="button"
                        role="tab"
                        id="overview-tab"
                        aria-controls="overview-panel"
                        aria-selected={activeTab === "overview"}
                        onClick={() => setSearchParams({})}
                        className={`border-b-2 py-3 text-sm font-medium ${activeTab === "overview" ? "border-coral text-navy" : "border-transparent text-gray-500 hover:text-navy"}`}
                    >
                        Overview
                    </button>
                    <Link to="/timeOff" className="border-b-2 border-transparent py-3 text-sm font-medium text-gray-500 hover:text-navy">Time off</Link>
                    <button
                        type="button"
                        role="tab"
                        id="equipment-tab"
                        aria-controls="equipment-panel"
                        aria-selected={activeTab === "equipment"}
                        onClick={() => setSearchParams({ tab: "assets" })}
                        className={`border-b-2 py-3 text-sm font-medium ${activeTab === "equipment" ? "border-coral text-navy" : "border-transparent text-gray-500 hover:text-navy"}`}
                    >
                        Assets
                    </button>
                    {(["documents", "history"] as const).map((section) => (
                        <button key={section} type="button" role="tab" id={`${section}-tab`}
                            aria-controls={`${section}-panel`} aria-selected={activeTab === section}
                            onClick={() => setSearchParams({ tab: section })}
                            className={`border-b-2 py-3 text-sm font-medium ${activeTab === section ? "border-coral text-navy" : "border-transparent text-gray-500 hover:text-navy"}`}>
                            {section === "documents" ? "Documents" : "History"}
                        </button>
                    ))}
                </div>

                {activeTab === "overview" ? (
                    <div role="tabpanel" id="overview-panel" aria-labelledby="overview-tab" className="p-6 text-sm">
                        <div className="mb-5 flex items-center justify-between">
                            <h2 className="text-base font-semibold text-navy">Employee details</h2>
                        </div>
                        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
                            {([
                                ["Work email", employee.email], ["Job title", employee.title],
                                ["Department", employee.departmentName], ["Location", employee.locationName],
                                ["Manager", employee.manager ? `${employee.manager.firstName} ${employee.manager.lastName}` : null],
                                ["Start date", formatDate(employee.startedOnUtc)], ["Phone", employee.phone],
                                ["Desk", employee.desk], ["Employee code", employee.employeeCode],
                                ["Employment type", employee.employmentType], ["Contract type", employee.contractType],
                                ["Weekly hours", employee.weeklyHours == null ? null : String(employee.weeklyHours)],
                                ["Birthday", employee.birthdayMonth && employee.birthdayDay ? `${employee.birthdayDay}/${employee.birthdayMonth}` : null],
                                ["Languages", employee.languages.join(", ")],
                            ] as const).map(([label, value]) => (
                                <div key={label}><span className="block text-gray-500">{label}</span><span className="font-medium text-navy">{value || "—"}</span></div>
                            ))}
                        </div>
                        <div className="mt-7 border-t border-gray-200 pt-5">
                            <h3 className="font-semibold text-navy">Direct reports ({employee.directReports.length})</h3>
                            {employee.directReports.length === 0 ? <p className="mt-2 text-gray-500">No direct reports.</p> : (
                                <ul className="mt-2 grid gap-2 sm:grid-cols-2">
                                    {employee.directReports.map((report) => <li key={report.id}><Link className="text-coral hover:underline" to={`/people/${report.id}`}>{report.firstName} {report.lastName}</Link><span className="text-gray-500"> · {report.title}</span></li>)}
                                </ul>
                            )}
                        </div>
                        {editing && <form className="mt-7 grid gap-4 border-t border-gray-200 pt-5 sm:grid-cols-2" onSubmit={(event) => { event.preventDefault(); saveProfile.mutate(); }}>
                            {(["phone", "desk", "employeeCode", "employmentType", "contractType", "weeklyHours", "languages"] as const).map((field) => (
                                <Input key={field} id={`profile-${field}`} label={field === "employeeCode" ? "Employee code" : field === "employmentType" ? "Employment type" : field === "contractType" ? "Contract type" : field === "weeklyHours" ? "Weekly hours" : field === "languages" ? "Languages (comma separated)" : field === "phone" ? "Phone" : "Desk"}
                                    type={field === "weeklyHours" ? "number" : "text"} value={profileForm[field]}
                                    onChange={(event) => setProfileForm((current) => ({ ...current, [field]: event.target.value }))}
                                    disabled={saveProfile.isPending} />
                            ))}
                            {saveProfile.isError && <p role="alert" className="text-red-600 sm:col-span-2">{saveProfile.error instanceof ApiError ? saveProfile.error.message : "Could not save this profile."}</p>}
                            <div className="flex gap-2 sm:col-span-2"><Button type="submit" disabled={saveProfile.isPending}>{saveProfile.isPending ? "Saving..." : "Save profile"}</Button><Button type="button" onClick={() => setEditing(false)}>Cancel</Button></div>
                        </form>}
                    </div>
                ) : activeTab === "documents" ? (
                    <div role="tabpanel" id="documents-panel" aria-labelledby="documents-tab" className="p-6 text-sm">
                        <h2 className="text-base font-semibold text-navy">Documents</h2>
                        {documentsQuery.isPending && <p className="mt-4 text-gray-500">Loading documents...</p>}
                        {documentsQuery.isError && <p role="alert" className="mt-4 text-red-600">{documentsQuery.error instanceof ApiError && documentsQuery.error.status === 401 ? "Sign in to view documents." : documentsQuery.error instanceof ApiError && documentsQuery.error.status === 403 ? "You do not have access to these documents." : "Could not load documents."}</p>}
                        {documentsQuery.data?.items.length === 0 && <p className="mt-4 text-gray-500">No documents for this employee.</p>}
                        <ul className="mt-4 divide-y divide-gray-100">{documentsQuery.data?.items.map((document) => <li key={document.id} className="py-3"><span className="font-medium text-navy">{document.name}</span><span className="ml-2 text-gray-500">{document.fileName} · {formatDate(document.createdOnUtc)}</span></li>)}</ul>
                    </div>
                ) : activeTab === "history" ? (
                    <div role="tabpanel" id="history-panel" aria-labelledby="history-tab" className="p-6 text-sm">
                        <h2 className="text-base font-semibold text-navy">Profile history</h2>
                        {historyQuery.isPending && <p className="mt-4 text-gray-500">Loading history...</p>}
                        {historyQuery.isError && <p role="alert" className="mt-4 text-red-600">{historyQuery.error instanceof ApiError && historyQuery.error.status === 401 ? "Sign in to view history." : historyQuery.error instanceof ApiError && historyQuery.error.status === 403 ? "You do not have access to this history." : "Could not load history."}</p>}
                        {historyQuery.data?.items.length === 0 && <p className="mt-4 text-gray-500">No profile changes recorded yet.</p>}
                        <ul className="mt-4 divide-y divide-gray-100">{historyQuery.data?.items.map((entry) => <li key={entry.id} className="py-3"><span className="font-medium text-navy">{entry.changedFields.join(", ")}</span><span className="ml-2 text-gray-500">{formatDate(entry.createdOnUtc)} · Employee #{entry.actorEmployeeId ?? "unknown"}</span></li>)}</ul>
                    </div>
                ) : (
                    <div role="tabpanel" id="equipment-panel" aria-labelledby="equipment-tab" className="p-6">
                        <h2 className="text-base font-semibold text-navy">Assets</h2>
                        <p className="mt-1 text-sm text-gray-500">Assets assigned to this employee and their return history.</p>

                        <form onSubmit={assign} className="mt-6 grid gap-4 rounded-lg border border-gray-200 p-4 sm:grid-cols-2">
                            <div className="sm:col-span-2"><h3 className="font-semibold text-navy">Assign asset</h3></div>
                            <Select
                                id="employee-equipment-type"
                                label="Asset"
                                value={selectedEquipmentId}
                                onChange={(event) => { setSelectedEquipmentId(event.target.value); setAssigned(false); assignMutation.reset(); }}
                                required
                                disabled={catalogQuery.isPending || catalogQuery.isError || assignMutation.isPending}
                                options={[
                                    { value: "", label: catalogQuery.isPending ? "Loading assets..." : "Select an asset" },
                                    ...(catalogQuery.data ?? []).filter((item) => !item.isAssigned).map((item) => ({ value: String(item.id), label: `${item.name} · ${item.code} · ${item.typeName}` })),
                                ]}
                            />
                            <Input id="employee-equipment-date" label="Assigned date" type="date" value={assignedOn} onChange={(event) => setAssignedOn(event.target.value)} required disabled={assignMutation.isPending} />
                            {catalogQuery.isError && <p role="alert" className="text-sm text-red-600 sm:col-span-2">Could not load the asset catalog.</p>}
                            {catalogQuery.data && !catalogQuery.data.some((item) => !item.isAssigned) && <p className="text-sm text-gray-500 sm:col-span-2">No available assets. Add one on the <Link to="/assets" className="font-medium text-coral hover:underline">Assets page</Link>.</p>}
                            {assignMutation.isError && <p role="alert" className="text-sm text-red-600 sm:col-span-2">{assignMutation.error instanceof ApiError ? assignMutation.error.message : "Could not assign asset."}</p>}
                            {assigned && <p role="status" className="text-sm text-green-700 sm:col-span-2">Asset assigned.</p>}
                            <Button type="submit" disabled={!selectedEquipmentId || assignMutation.isPending || catalogQuery.isError}>{assignMutation.isPending ? "Assigning..." : "Assign asset"}</Button>
                        </form>

                        {equipmentQuery.isPending && <p role="status" className="mt-5 text-sm text-gray-500">Loading assets...</p>}
                        {equipmentQuery.isError && (
                            <p role="alert" className="mt-5 text-sm text-red-600">Could not load assets.</p>
                        )}
                        {equipmentQuery.data?.length === 0 && (
                            <p className="mt-5 text-sm text-gray-500">No asset records for this employee.</p>
                        )}
                        {equipmentQuery.data && equipmentQuery.data.length > 0 && (
                            <div className="mt-5 overflow-x-auto">
                                <table className="min-w-full text-left text-sm">
                                    <thead className="border-b border-gray-200 text-gray-500">
                                        <tr>
                                            <th scope="col" className="py-3 pr-5 font-medium">Item</th>
                                            <th scope="col" className="py-3 pr-5 font-medium">Code</th>
                                            <th scope="col" className="py-3 pr-5 font-medium">Type</th>
                                            <th scope="col" className="py-3 pr-5 font-medium">Assigned</th>
                                            <th scope="col" className="py-3 pr-5 font-medium">Returned</th>
                                            <th scope="col" className="py-3 font-medium">Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {equipmentQuery.data.map((item) => (
                                            <tr key={item.id} className="border-b border-gray-100 last:border-0">
                                                <td className="py-3 pr-5 font-medium text-navy">{item.name}</td>
                                                <td className="py-3 pr-5 text-gray-700">{item.code}</td>
                                                <td className="py-3 pr-5 text-gray-700">{item.typeName}</td>
                                                <td className="py-3 pr-5 text-gray-700">{formatDate(item.assignedOnUtc)}</td>
                                                <td className="py-3 pr-5 text-gray-700">{item.returnedOnUtc ? formatDate(item.returnedOnUtc) : "—"}</td>
                                                <td className="py-3 text-gray-700">{item.returnedOnUtc ? "Returned" : "Assigned"}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </div>
                )}
            </section>
        </div>
    );
}
