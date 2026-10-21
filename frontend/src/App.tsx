import { Outlet, useLocation } from "react-router-dom";
import { Sidebar } from "./layout/Sidebar";
import { PageHeader } from "./layout/PageHeader";

export default function App() {
    const location = useLocation();
    const isSettingsPage = location.pathname.startsWith("/settings");

    return (
        <div className="flex h-screen bg-gray-50 font-sans">
            <Sidebar />
            <main className="min-w-0 flex-1 overflow-y-auto">
                {isSettingsPage && <PageHeader title="Settings" />}
                <Outlet />
            </main>
        </div>
    );
}
