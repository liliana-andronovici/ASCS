import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { User, LogOut } from "lucide-react";
import { useCurrentEmployee } from "./useCurrentEmployee";
import { useLocalAvatar } from "./useLocalAvatar";
import { clearToken, decodeToken, getToken } from "../../lib/sessions";

type Props = { collapsed: boolean };

export function AccountMenu({ collapsed }: Props) {
    const { employee, isLoading, isError } = useCurrentEmployee();
    const token = getToken();
    const accountEmail = token ? decodeToken(token)?.email : undefined;
    const photoUrl = useLocalAvatar();
    const [open, setOpen] = useState(false);
    const ref = useRef<HTMLDivElement>(null);
    const navigate = useNavigate();
    const displayName = employee
        ? `${employee.firstName} ${employee.lastName}`
        : isLoading
          ? "Loading profile..."
          : accountEmail || "Account unavailable";
    const initials = employee
        ? `${employee.firstName[0] ?? ""}${employee.lastName[0] ?? ""}`.toUpperCase()
        : accountEmail?.slice(0, 2).toUpperCase() || "?";

    useEffect(() => {
        function handleClick(e: MouseEvent) {
            if (ref.current && !ref.current.contains(e.target as Node)) {
                setOpen(false);
            }
        }
        document.addEventListener("mousedown", handleClick);
        return () => document.removeEventListener("mousedown", handleClick);
    }, []);

    function handleSignOut() {
        clearToken();
        setOpen(false);
        navigate("/sign-in");
    }

    return (
        <div ref={ref} className="relative mt-auto border-t border-white/10 p-3">
            {open && (
                <div className="absolute bottom-full left-3 mb-2 w-48 overflow-hidden rounded-md border border-gray-200 bg-white py-1 text-sm text-navy shadow-card">
                    <button
                        onClick={() => {
                            setOpen(false);
                            navigate("/settings");
                        }}
                        className="flex w-full items-center gap-2 px-3 py-2 text-left hover:bg-gray-50"
                    >
                        <User size={15} /> My profile
                    </button>
                    <button
                        onClick={handleSignOut}
                        className="flex w-full items-center gap-2 px-3 py-2 text-left text-red-600 hover:bg-gray-50"
                    >
                        <LogOut size={15} /> Sign out
                    </button>
                </div>
            )}

            <button
                onClick={() => setOpen((v) => !v)}
                className={`flex w-full items-center gap-3 rounded-md p-1 hover:bg-white/10 ${
                    collapsed ? "justify-center" : ""
                }`}
                title={collapsed ? displayName : undefined}
            >
                {photoUrl ? (
                    <img src={photoUrl} alt="" className="h-8 w-8 shrink-0 rounded-full object-cover" />
                ) : (
                    <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-coral text-xs font-bold text-white">
                        {initials}
                    </span>
                )}
                {!collapsed && (
                    <span className="flex min-w-0 flex-col text-left leading-tight">
                        <span className="truncate text-sm font-semibold text-white">
                            {displayName}
                        </span>
                        <span className="truncate text-xs text-white/60">
                            {employee?.title ??
                                (isLoading
                                    ? "Loading profile..."
                                    : isError && accountEmail
                                      ? "Profile details unavailable"
                                      : "")}
                        </span>
                    </span>
                )}
            </button>
        </div>
    );
}
