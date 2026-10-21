import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useParams, useSearchParams } from "react-router-dom";
import { Avatar } from "../../components/Avatar";
import { Badge } from "../../components/Badge";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { Select } from "../../components/Select";
import { ApiError } from "../../lib/api";
import { getEquipment } from "../equipment/api";
import { addEmployeeEquipment, getEmployeeById, getEmployeeEquipment } from "./api";

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
    const { employeeId: rawEmployeeId } = useParams();
    const employeeId = Number(rawEmployeeId);
    const validEmployeeId = Number.isInteger(employeeId) && employeeId > 0;
    const [searchParams, setSearchParams] = useSearchParams();
    const activeTab = ["assets", "equipment"].includes(searchParams.get("tab") ?? "") ? "equipment" : "overview";

    const employeeQuery = useQuery({
        queryKey: ["employees", employeeId],
        queryFn: () => getEmployeeById(employeeId),
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

    return (
        <div className="p-6">
            <Link to="/people" className="text-sm font-medium text-coral hover:underline">← Back to People</Link>
            <section className="mt-4 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-card">
                <div className="flex flex-wrap items-center gap-4 border-b border-gray-200 px-6 py-5">
                    <Avatar firstName={employee.firstName} lastName={employee.lastName} />
                    <div className="min-w-0 flex-1">
                        <h1 className="text-xl font-semibold text-navy">{employee.firstName} {employee.lastName}</h1>
                        <p className="text-sm text-gray-500">{employee.title} · {employee.email}</p>
                    </div>
                    <Badge variant={employee.status === "Active" ? "active" : "inactive"}>{employee.status}</Badge>
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
                </div>

                {activeTab === "overview" ? (
                    <div role="tabpanel" id="overview-panel" aria-labelledby="overview-tab" className="grid gap-5 p-6 text-sm sm:grid-cols-2">
                        <div><span className="block text-gray-500">Work email</span><span className="font-medium text-navy">{employee.email}</span></div>
                        <div><span className="block text-gray-500">Job title</span><span className="font-medium text-navy">{employee.title}</span></div>
                        <div><span className="block text-gray-500">Start date</span><span className="font-medium text-navy">{formatDate(employee.startedOnUtc)}</span></div>
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
