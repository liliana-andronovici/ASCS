import { Outlet, useLocation, useMatch } from "react-router-dom";
import { Sidebar } from "./layout/Sidebar";
import { PageHeader } from "./layout/PageHeader";

export default function App() {
    const location = useLocation();
    const isOrgChartPage = !!useMatch("/orgChart");
    const hideGreeting = location.pathname.startsWith("/onboarding") || isOrgChartPage;

    return (
        <div className="flex h-screen bg-gray-50 font-sans">
            <Sidebar />
            <main className="min-w-0 flex-1 overflow-y-auto">
                {!hideGreeting && <PageHeader title="Good Morning, Y/N" />}
                <Outlet />
            </main>
        </div>
    );
}
