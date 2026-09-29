import { useState } from "react";
import type { ReactNode } from "react";
import { Link, useParams } from "react-router-dom";
import { ChevronLeft } from "lucide-react";
import { useEmployee } from "./useEmployee";
import { useEmployeeDirectory } from "./useEmployees";
import { useDepartments } from "../departments/useDepartments";
import { useLocations } from "../locations/useLocations";
import { EmployeeFormModal } from "./EmployeeFormModal";
import { Badge } from "../../components/Badge";
import type { BadgeVariant } from "../../components/Badge";
import { Button } from "../../components/Button";
import type { EmployeeStatus } from "./types";

const STATUS_BADGE: Record<EmployeeStatus, BadgeVariant> = {
    Active: "active",
    Inactive: "inactive",
};

const TABS = ["Overview", "Time off", "Documents", "Equipment", "History"];
const AVAILABLE_TABS = ["Overview", "Time off", "Documents", "Equipment"];

// Placeholder values until the backend has time off data.
const TIME_OFF = { total: 25, used: 12, requested: 3, sick: 2 };

function formatDate(iso: string) {
    return new Date(iso).toLocaleDateString("en-GB", { day: "numeric", month: "short", year: "numeric" });
}

function formatSince(iso: string) {
    return new Date(iso).toLocaleDateString("en-US", { month: "short", year: "numeric" });
}

function tenure(iso: string) {
    const years = (Date.now() - new Date(iso).getTime()) / (365.25 * 24 * 60 * 60 * 1000);
    return years < 1 ? "less than a year" : `${years.toFixed(1)} years`;
}

function Section({ title, divider, children }: { title: string; divider?: boolean; children: ReactNode }) {
    return (
        <div className={`px-6 py-5 ${divider ? "md:border-r md:border-gray-200" : ""}`}>
            <h2 className="mb-3 text-base font-bold text-navy">{title}</h2>
            <dl className="grid grid-cols-[96px_1fr] gap-y-2.5 text-[13px]">{children}</dl>
        </div>
    );
}

function Field({ label, value }: { label: string; value: string }) {
    return (
        <>
            <dt className="text-gray-500">{label}</dt>
            <dd className="text-navy">{value}</dd>
        </>
    );
}

function TimeOffBalance() {
    const { total, used, requested, sick } = TIME_OFF;
    const remaining = total - used;
    const left = total - used - requested;

    return (
        <div className="max-w-xl px-6 py-6">
            <h2 className="mb-4 text-xl font-bold text-navy">Time off balance</h2>
            <div className="flex gap-12">
                <div>
                    <div className="text-5xl font-bold leading-none text-navy">
                        {remaining}
                        <span className="text-xl font-normal text-gray-500"> / {total}</span>
                    </div>
                    <span className="mt-2 block text-sm uppercase tracking-[0.08em] text-gray-500">Vacation</span>
                </div>
                <div>
                    <div className="text-5xl font-bold leading-none text-navy">{sick}</div>
                    <span className="mt-2 block text-sm uppercase tracking-[0.08em] text-gray-500">Sick</span>
                </div>
            </div>
            <div className="mt-4 flex h-2.5 overflow-hidden rounded-full bg-gray-200">
                <span className="bg-navy" style={{ width: `${(used / total) * 100}%` }} />
                <span className="bg-coral" style={{ width: `${(requested / total) * 100}%` }} />
            </div>
            <div className="mt-2 text-base text-gray-500">
                {used} used · {requested} requested · {left} left
            </div>
        </div>
    );
}

function DocumentsList() {
    return (
        <div className="max-w-3xl px-6 py-6">
            <h2 className="mb-4 text-xl font-bold text-navy">Documents</h2>
            <div className="border-y border-gray-200 py-10 text-center text-sm text-gray-500">
                No documents yet. PDF files added for this employee will appear here.
            </div>
        </div>
    );
}

function EquipmentList() {
    return (
        <div className="max-w-3xl px-6 py-6">
            <h2 className="mb-4 text-xl font-bold text-navy">Equipment</h2>
            <div className="border-y border-gray-200 py-10 text-center text-sm text-gray-500">
                No equipment assigned yet. Laptops and other equipment given to this employee will appear here.
            </div>
        </div>
    );
}

export default function EmployeeProfilePage() {
    const { id } = useParams();
    const employeeId = Number(id);
    const [formOpen, setFormOpen] = useState(false);
    const [activeTab, setActiveTab] = useState("Overview");

    const { data: employee, isLoading, isError, error } = useEmployee(employeeId);
    const { data: departments } = useDepartments();
    const { data: locations } = useLocations();
    const { data: directory } = useEmployeeDirectory();

    if (isLoading) return <div className="p-6">Loading profile...</div>;
    if (isError) return <div className="p-6 text-red-600">Error: {(error as Error).message}</div>;
    if (!employee) return null;

    const department = departments?.find((d) => Number(d.id) === employee.departmentId);
    const location = locations?.find((l) => l.id === employee.locationId);
    const manager = directory?.items.find((e) => e.id === employee.managerId);

    const initials = `${employee.firstName[0] ?? ""}${employee.lastName[0] ?? ""}`.toUpperCase();
    const place = [department?.name, location?.name].filter(Boolean).join(" · ");

    return (
        <div className="flex min-h-full flex-col bg-white">
            <div className="flex h-[60px] items-center justify-between border-b border-gray-200 px-6">
                <Link to="/people" className="flex items-center gap-1 text-sm text-gray-500 hover:text-navy">
                    <ChevronLeft size={16} />
                    People
                </Link>
                <Button onClick={() => setFormOpen(true)} className="!w-auto px-4">
                    Edit profile
                </Button>
            </div>

            <div className="flex items-center gap-6 border-b border-gray-200 px-6 py-7">
                <div className="flex h-28 w-28 flex-none items-center justify-center rounded-full bg-navy text-3xl font-bold text-white">
                    {initials}
                </div>
                <div className="flex-1">
                    {place && (
                        <div className="mb-1 text-[11px] uppercase tracking-[0.1em] text-gray-500">{place}</div>
                    )}
                    <h1 className="text-[32px] font-bold leading-tight text-navy">
                        {employee.firstName} {employee.lastName}
                    </h1>
                    <div className="mt-1 text-sm text-gray-500">
                        {employee.title}
                        {manager && ` · reports to ${manager.firstName} ${manager.lastName}`}
                    </div>
                </div>
                <div className="flex items-center gap-3">
                    <Badge variant={STATUS_BADGE[employee.status]}>{employee.status}</Badge>
                    <span className="text-sm text-gray-500">Since {formatSince(employee.startedOnUtc)}</span>
                </div>
            </div>

            <div className="flex gap-6 border-b border-gray-200 px-6 text-sm font-medium">
                {TABS.map((tab) =>
                    AVAILABLE_TABS.includes(tab) ? (
                        <button
                            key={tab}
                            type="button"
                            onClick={() => setActiveTab(tab)}
                            className={
                                tab === activeTab
                                    ? "-mb-px border-b-[3px] border-coral py-3 font-semibold text-coral"
                                    : "py-3 text-gray-500 hover:text-navy"
                            }
                        >
                            {tab}
                        </button>
                    ) : (
                        <span key={tab} title="Not built yet" className="cursor-not-allowed py-3 text-gray-500">
                            {tab}
                        </span>
                    )
                )}
            </div>

            {activeTab === "Overview" && (
                <div className="grid flex-1 md:grid-cols-2">
                    <Section title="Contact" divider>
                        <Field label="Work email" value={employee.email} />
                        <Field label="Phone" value="—" />
                        <Field label="Desk" value="—" />
                        <Field label="Birthday" value="—" />
                        <Field label="Languages" value="—" />
                    </Section>
                    <Section title="Employment">
                        <Field label="Department" value={department?.name ?? "—"} />
                        <Field label="Location" value={location?.name ?? "—"} />
                        <Field
                            label="Start date"
                            value={`${formatDate(employee.startedOnUtc)} · ${tenure(employee.startedOnUtc)}`}
                        />
                    </Section>
                </div>
            )}

            {activeTab === "Time off" && (
                <div className="flex-1">
                    <TimeOffBalance />
                </div>
            )}

            {activeTab === "Documents" && (
                <div className="flex-1">
                    <DocumentsList />
                </div>
            )}

            {activeTab === "Equipment" && (
                <div className="flex-1">
                    <EquipmentList />
                </div>
            )}

            <EmployeeFormModal
                key={employee.id}
                open={formOpen}
                onClose={() => setFormOpen(false)}
                employee={employee}
            />
        </div>
    );
}
