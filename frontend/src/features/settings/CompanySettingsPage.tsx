import { useQueries, useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Building2, Network, Users } from "lucide-react";
import { getEmployees } from "../employees/api";
import { useDepartments } from "../departments/useDepartments";
import { useLocations } from "../locations/useLocations";

export default function CompanySettingsPage() {
    const departmentsQuery = useDepartments();
    const locationsQuery = useLocations();
    const activeEmployeesQuery = useQuery({
        queryKey: ["employees", "company", "active"],
        queryFn: () => getEmployees({ status: "Active", pageSize: 1 }),
    });
    const departments = [...(departmentsQuery.data ?? [])].sort((a, b) => a.name.localeCompare(b.name));
    const locations = [...(locationsQuery.data ?? [])].sort((a, b) => a.name.localeCompare(b.name));
    const departmentCounts = useQueries({
        queries: departments.map((department) => ({
            queryKey: ["employees", "company", "department", department.id],
            queryFn: () => getEmployees({ departmentId: department.id, status: "Active", pageSize: 1 }),
        })),
    });
    const locationCounts = useQueries({
        queries: locations.map((location) => ({
            queryKey: ["employees", "company", "location", location.id],
            queryFn: () => getEmployees({ locationId: location.id, status: "Active", pageSize: 1 }),
        })),
    });

    const departmentError = departmentsQuery.isError;
    const locationError = locationsQuery.isError;

    return (
        <div className="p-6">
            <div className="mb-6">
                <h2 className="text-xl font-bold text-navy">Company overview</h2>
                <p className="mt-1 text-sm text-gray-500">People, departments, and offices in Infigo HR.</p>
            </div>

            <div className="grid gap-4 sm:grid-cols-3">
                <div className="rounded-lg border border-gray-200 bg-white p-5 shadow-card">
                    <Users size={20} className="text-coral" />
                    <p className="mt-3 text-sm text-gray-500">Active employees</p>
                    <p className="text-3xl font-bold text-navy">{activeEmployeesQuery.isPending ? "…" : activeEmployeesQuery.isError ? "—" : activeEmployeesQuery.data.totalCount}</p>
                </div>
                <div className="rounded-lg border border-gray-200 bg-white p-5 shadow-card">
                    <Network size={20} className="text-coral" />
                    <p className="mt-3 text-sm text-gray-500">Departments</p>
                    <p className="text-3xl font-bold text-navy">{departmentsQuery.isPending ? "…" : departmentError ? "—" : departments.length}</p>
                </div>
                <div className="rounded-lg border border-gray-200 bg-white p-5 shadow-card">
                    <Building2 size={20} className="text-coral" />
                    <p className="mt-3 text-sm text-gray-500">Offices</p>
                    <p className="text-3xl font-bold text-navy">{locationsQuery.isPending ? "…" : locationError ? "—" : locations.length}</p>
                </div>
            </div>

            {activeEmployeesQuery.isError && <p role="alert" className="mt-4 text-sm text-red-600">Could not load the employee count. <button className="underline" onClick={() => activeEmployeesQuery.refetch()}>Retry</button></p>}

            <div className="mt-6 grid gap-6 xl:grid-cols-2">
                <section className="rounded-lg border border-gray-200 bg-white shadow-card">
                    <div className="flex items-center justify-between border-b border-gray-100 px-5 py-4">
                        <h3 className="font-semibold text-navy">Offices</h3>
                        <Link to="/locations" className="text-sm font-medium text-coral hover:underline">Manage offices</Link>
                    </div>
                    {locationsQuery.isPending && <p className="p-5 text-sm text-gray-500">Loading offices...</p>}
                    {locationError && <p role="alert" className="p-5 text-sm text-red-600">Could not load offices. <button className="underline" onClick={() => locationsQuery.refetch()}>Retry</button></p>}
                    {locationsQuery.isSuccess && locations.length === 0 && <p className="p-5 text-sm text-gray-500">No offices added yet.</p>}
                    {locations.length > 0 && <ul className="divide-y divide-gray-100">{locations.map((location, index) => <li key={location.id} className="flex items-center justify-between gap-3 px-5 py-3 text-sm">
                        <span className="font-medium text-navy">{location.name}</span>
                        <span className="text-gray-500">{locationCounts[index]?.isPending ? "Loading…" : locationCounts[index]?.isError ? "Count unavailable" : `${locationCounts[index]?.data?.totalCount ?? 0} active employees`}</span>
                    </li>)}</ul>}
                </section>

                <section className="rounded-lg border border-gray-200 bg-white shadow-card">
                    <div className="flex items-center justify-between border-b border-gray-100 px-5 py-4">
                        <h3 className="font-semibold text-navy">Departments</h3>
                        <Link to="/departments" className="text-sm font-medium text-coral hover:underline">Manage departments</Link>
                    </div>
                    {departmentsQuery.isPending && <p className="p-5 text-sm text-gray-500">Loading departments...</p>}
                    {departmentError && <p role="alert" className="p-5 text-sm text-red-600">Could not load departments. <button className="underline" onClick={() => departmentsQuery.refetch()}>Retry</button></p>}
                    {departmentsQuery.isSuccess && departments.length === 0 && <p className="p-5 text-sm text-gray-500">No departments added yet.</p>}
                    {departments.length > 0 && <ul className="divide-y divide-gray-100">{departments.map((department, index) => <li key={department.id} className="flex items-center justify-between gap-3 px-5 py-3 text-sm">
                        <span className="font-medium text-navy">{department.name}</span>
                        <span className="text-gray-500">{departmentCounts[index]?.isPending ? "Loading…" : departmentCounts[index]?.isError ? "Count unavailable" : `${departmentCounts[index]?.data?.totalCount ?? 0} active employees`}</span>
                    </li>)}</ul>}
                </section>
            </div>
        </div>
    );
}
