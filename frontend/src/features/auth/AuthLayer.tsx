import { Outlet, useLocation } from "react-router-dom";

export function AuthLayer() {
    const isInvite = useLocation().pathname === "/accept-invite";
    return (
        <div className="flex min-h-screen items-center justify-center bg-gray-50 p-4">
            <div className={`w-full overflow-hidden shadow-card ${isInvite ? "max-w-[440px] rounded-2xl" : "max-w-[420px] rounded-lg"}`}>
                <div className={`flex items-center justify-center bg-navy ${isInvite ? "h-[120px]" : "p-6"}`}>
                    <span className="text-2xl font-semibold tracking-wide text-white">HR Portal</span>
                </div>
                <div className="bg-white p-8"><Outlet /></div>
            </div>
        </div>
    );
}
