import { Outlet } from "react-router-dom";
import { Sidebar } from "./layout/Sidebar";
import { PageHeader } from "./layout/PageHeader";
import { useLocation } from "react-router-dom";

export default function App() {
    const location = useLocation();
    const title = location.pathname.startsWith("/settings") ? "Settings" : "Good Morning, Y/N";

    return (
        <div className="flex h-screen bg-gray-50 font-sans">
            <Sidebar />
            <main className="flex-1 overflow-y-auto">
                <PageHeader title={title} />
                <Outlet />
            </main>
        </div>
    );
}
