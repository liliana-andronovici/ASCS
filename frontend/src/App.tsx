import { Outlet } from "react-router-dom";
import { Sidebar } from "./layout/Sidebar";

export default function App() {
    return (
        <div className="flex h-screen bg-gray-50 font-sans">
            <Sidebar />
            <main className="flex-1 overflow-y-auto">
                <Outlet />
            </main>
        </div>
    );
}