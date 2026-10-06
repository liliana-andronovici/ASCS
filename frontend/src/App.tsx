import { Outlet, useMatch } from "react-router-dom";
import { Sidebar } from "./layout/Sidebar";
import { PageHeader } from "./layout/PageHeader";

export default function App() {
    const isDepartmentsPage = !!useMatch("/departments");

    return (
        <div className="flex h-screen bg-gray-50 font-sans">
            <Sidebar />
            <main className="flex-1 overflow-y-auto">
                {!isDepartmentsPage && <PageHeader title="Good Morning, Y/N" />}
                <Outlet />
            </main>
        </div>
    );
}