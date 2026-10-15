import { useState } from "react";
import { Button } from "../../components/Button";
import { PageHeader } from "../../layout/PageHeader";
import { ProfileTab } from "./ProfileTab";
import { useLocalPreferences, type Preferences } from "./useLocalPreferences";

const GROUPS = [
    {
        title: "Account",
        tabs: [
            { id: "profile", label: "Profile" },
            { id: "security", label: "Password & security" },
        ],
    },
    {
        title: "Preferences",
        tabs: [
            { id: "notifications", label: "Notifications" },
            { id: "display", label: "Tables & display" },
        ],
    },
] as const;

type TabId = (typeof GROUPS)[number]["tabs"][number]["id"];

export default function SettingsPage() {
    const [tab, setTab] = useState<TabId>("profile");

    return (
        <div>
            <PageHeader title="Settings" subtitle="Manage your profile and how Infigo HR works for you." />
            <div className="p-6">
                <div className="flex overflow-hidden rounded-lg border border-gray-200 bg-white shadow-card">
                    <nav className="w-56 shrink-0 border-r border-gray-200 py-4">
                        {GROUPS.map((group) => (
                            <div key={group.title} className="mb-4 last:mb-0">
                                <p className="px-4 pb-1 text-xs font-semibold text-gray-400">{group.title}</p>
                                {group.tabs.map((t) => (
                                    <button
                                        key={t.id}
                                        onClick={() => setTab(t.id)}
                                        aria-current={tab === t.id ? "page" : undefined}
                                        className={`block w-full border-l-2 px-4 py-2 text-left text-sm ${
                                            tab === t.id
                                                ? "border-coral bg-[#fdecea] font-semibold text-coral"
                                                : "border-transparent text-navy hover:bg-gray-50"
                                        }`}
                                    >
                                        {t.label}
                                    </button>
                                ))}
                            </div>
                        ))}
                    </nav>

                    <div className="min-w-0 flex-1 p-6">
                        {tab === "profile" && <ProfileTab />}
                        {tab === "security" && <SecurityTab />}
                        {tab === "notifications" && <NotificationsTab />}
                        {tab === "display" && <DisplayTab />}
                    </div>
                </div>
            </div>
        </div>
    );
}

function SecurityTab() {
    return (
        <div className="flex max-w-md flex-col gap-4">
            <h2 className="text-lg font-semibold text-navy">Password & security</h2>
            <div className="rounded-md bg-amber-50 px-4 py-3 text-sm text-amber-700">
                Password changes and two-factor authentication are not available here yet.
                The current backend only exposes sign-in and sign-up endpoints for authentication.
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
            These settings are saved only in this browser. The backend does not have an endpoint for them yet.
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

function DisplayTab() {
    const { prefs, update } = useLocalPreferences();
    return (
        <div className="flex max-w-xl flex-col gap-4">
            <h2 className="text-lg font-semibold text-navy">Tables & display</h2>
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
