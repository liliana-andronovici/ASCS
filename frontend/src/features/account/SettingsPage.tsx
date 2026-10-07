import { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { Avatar } from "../../components/Avatar";
import { Badge } from "../../components/Badge";
import { useCurrentEmployee } from "./useCurrentEmployee";
import { useLocalAvatar } from "./useLocalAvatar";
import { setLocalAvatar } from "../../lib/sessions";
import { useLocalPreferences, type Preferences } from "./useLocalPreferences";
import { useUpdateEmployee } from "../employees/useEmployeeMutations";
import { employeeFormSchema, type EmployeeFormValues } from "../employees/employeeSchema";

const TABS = [
    { id: "profile", label: "My profile" },
    { id: "notifications", label: "Notifications" },
    { id: "preferences", label: "Preferences" },
    { id: "security", label: "Security" },
] as const;

type TabId = (typeof TABS)[number]["id"];

export default function SettingsPage() {
    const [tab, setTab] = useState<TabId>("profile");

    return (
        <div className="p-6">
            <div className="flex overflow-hidden rounded-lg border border-gray-200 bg-white shadow-card">
                <nav className="w-48 shrink-0 border-r border-gray-200 py-4">
                    {TABS.map((t) => (
                        <button
                            key={t.id}
                            onClick={() => setTab(t.id)}
                            className={`block w-full px-4 py-2 text-left text-sm ${
                                tab === t.id
                                    ? "bg-[#fdecea] font-semibold text-coral"
                                    : "text-navy hover:bg-gray-50"
                            }`}
                        >
                            {t.label}
                        </button>
                    ))}
                </nav>

                <div className="flex-1 p-6">
                    {tab === "profile" && <ProfileTab />}
                    {tab === "notifications" && <NotificationsTab />}
                    {tab === "preferences" && <PreferencesTab />}
                    {tab === "security" && <SecurityTab />}
                </div>
            </div>
        </div>
    );
}

function formatSince(dateIso: string) {
    return new Date(dateIso).toLocaleDateString("en-US", { month: "short", year: "numeric" });
}

function ProfileTab() {
    const { employee, isFallback, isLoading } = useCurrentEmployee();
    const photoUrl = useLocalAvatar();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const updateMutation = useUpdateEmployee();

    const {
        register,
        handleSubmit,
        formState: { errors, isDirty },
    } = useForm<EmployeeFormValues>({
        resolver: zodResolver(employeeFormSchema),
        values: {
            firstName: employee.firstName,
            lastName: employee.lastName,
            email: employee.email,
            title: employee.title,
            startedOnUtc: employee.startedOnUtc.slice(0, 10),
        },
    });

    if (isLoading) return <div className="text-sm text-gray-500">Loading your profile...</div>;

    function handlePhotoChange(e: React.ChangeEvent<HTMLInputElement>) {
        const file = e.target.files?.[0];
        if (!file) return;
        const reader = new FileReader();
        reader.onload = () => {
            if (typeof reader.result === "string") {
                setLocalAvatar(reader.result);
            }
        };
        reader.readAsDataURL(file);
    }

    function onSubmit(values: EmployeeFormValues) {
        if (isFallback) return;
        updateMutation.mutate({
            id: employee.id,
            input: {
                ...values,
                startedOnUtc: new Date(`${values.startedOnUtc}T00:00:00.000Z`).toISOString(),
                departmentId: employee.departmentId,
                locationId: employee.locationId,
                managerId: employee.managerId,
                status: employee.status,
            },
        });
    }

    return (
        <div className="flex max-w-xl flex-col gap-6">
            <div className="flex items-center gap-4">
                <Avatar firstName={employee.firstName} lastName={employee.lastName} size="lg" photoUrl={photoUrl} />
                <div className="flex flex-col gap-1">
                    <h2 className="text-xl font-bold text-navy">
                        {employee.firstName} {employee.lastName}
                    </h2>
                    <p className="text-sm text-gray-500">{employee.title}</p>
                    <div className="flex items-center gap-2">
                        <Badge variant={employee.status === "Active" ? "active" : "inactive"}>
                            {employee.status}
                        </Badge>
                        <span className="text-xs text-gray-400">Since {formatSince(employee.startedOnUtc)}</span>
                    </div>
                    <input
                        ref={fileInputRef}
                        type="file"
                        accept="image/*"
                        onChange={handlePhotoChange}
                        className="hidden"
                    />
                    <button
                        onClick={() => fileInputRef.current?.click()}
                        className="mt-1 self-start text-sm font-medium text-coral hover:underline"
                    >
                        Change photo
                    </button>
                </div>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
                {isFallback && (
                    <div className="rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-700">
                        Nu esti logat cu un cont real, deci aici vezi date exemplu care nu pot fi
                        salvate. Loghează-te ca sa iti poti edita propriul profil.
                    </div>
                )}

                <div>
                    <Input label="First name" disabled={isFallback} {...register("firstName")} />
                    {errors.firstName && <p className="mt-1 text-sm text-red-600">{errors.firstName.message}</p>}
                </div>
                <div>
                    <Input label="Last name" disabled={isFallback} {...register("lastName")} />
                    {errors.lastName && <p className="mt-1 text-sm text-red-600">{errors.lastName.message}</p>}
                </div>
                <div>
                    <Input label="Work email" type="email" disabled={isFallback} {...register("email")} />
                    {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
                </div>
                <div>
                    <Input label="Started on" type="date" disabled={isFallback} {...register("startedOnUtc")} />
                </div>

                {updateMutation.isError && (
                    <p className="text-sm text-red-600">Couldn't save your changes. Please try again.</p>
                )}
                {updateMutation.isSuccess && <p className="text-sm text-green-700">Saved.</p>}

                <Button
                    type="submit"
                    className="!w-auto px-4"
                    disabled={isFallback || !isDirty || updateMutation.isPending}
                >
                    {updateMutation.isPending ? "Saving..." : "Save changes"}
                </Button>
            </form>
        </div>
    );
}

function SecurityTab() {
    return (
        <div className="flex max-w-md flex-col gap-4">
            <h2 className="text-lg font-semibold text-navy">Security</h2>
            <div className="rounded-md bg-amber-50 px-4 py-3 text-sm text-amber-700">
                Schimbarea parolei si autentificarea in doi pasi nu sunt disponibile inca,
                backend-ul nu expune un endpoint pentru asta, doar sign in si sign up.
            </div>
            <div className="flex flex-col gap-1 opacity-50">
                <label className="text-sm text-gray-700">Current password</label>
                <input disabled type="password" className="rounded-md border border-gray-300 px-3 py-2 text-sm" />
            </div>
            <div className="flex flex-col gap-1 opacity-50">
                <label className="text-sm text-gray-700">New password</label>
                <input disabled type="password" className="rounded-md border border-gray-300 px-3 py-2 text-sm" />
            </div>
            <Button disabled className="!w-auto px-4">
                Update password
            </Button>
        </div>
    );
}

function ToggleRow({
    label,
    description,
    checked,
    onChange,
}: {
    label: string;
    description: string;
    checked: boolean;
    onChange: (value: boolean) => void;
}) {
    return (
        <label className="flex cursor-pointer items-start justify-between gap-4 border-b border-gray-100 py-3">
            <span>
                <span className="block text-sm font-medium text-navy">{label}</span>
                <span className="block text-xs text-gray-500">{description}</span>
            </span>
            <input
                type="checkbox"
                checked={checked}
                onChange={(e) => onChange(e.target.checked)}
                className="mt-1 h-4 w-4 accent-[#e8594f]"
            />
        </label>
    );
}

function LocalOnlyNote() {
    return (
        <div className="rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-700">
            Aceste setari se salveaza doar in acest browser, backend-ul nu are inca endpoint pentru ele.
        </div>
    );
}

function NotificationsTab() {
    const { prefs, update } = useLocalPreferences();
    return (
        <div className="flex max-w-xl flex-col gap-4">
            <h2 className="text-lg font-semibold text-navy">Notifications</h2>
            <LocalOnlyNote />
            <div>
                <ToggleRow
                    label="Email notifications"
                    description="Receive important updates by email."
                    checked={prefs.emailNotifications}
                    onChange={(v) => update("emailNotifications", v)}
                />
                <ToggleRow
                    label="Time off alerts"
                    description="Get notified when a time off request needs attention."
                    checked={prefs.timeOffAlerts}
                    onChange={(v) => update("timeOffAlerts", v)}
                />
                <ToggleRow
                    label="New hire alerts"
                    description="Get notified when someone new joins the company."
                    checked={prefs.newHireAlerts}
                    onChange={(v) => update("newHireAlerts", v)}
                />
            </div>
        </div>
    );
}

function PreferencesTab() {
    const { prefs, update } = useLocalPreferences();
    return (
        <div className="flex max-w-xl flex-col gap-4">
            <h2 className="text-lg font-semibold text-navy">Preferences</h2>
            <LocalOnlyNote />
            <div className="flex flex-col gap-1">
                <label htmlFor="pageSize" className="text-sm text-gray-700">
                    Rows per page in tables
                </label>
                <select
                    id="pageSize"
                    value={prefs.pageSize}
                    onChange={(e) => update("pageSize", Number(e.target.value) as Preferences["pageSize"])}
                    className="w-40 rounded-md border border-gray-300 px-3 py-2 text-sm"
                >
                    <option value={10}>10</option>
                    <option value={25}>25</option>
                    <option value={50}>50</option>
                </select>
            </div>
            <ToggleRow
                label="Compact tables"
                description="Show more rows with tighter spacing."
                checked={prefs.compactTables}
                onChange={(v) => update("compactTables", v)}
            />
        </div>
    );
}
