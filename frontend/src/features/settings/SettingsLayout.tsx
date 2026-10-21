import { NavLink, Outlet } from "react-router-dom";

const sections = [
    { label: "Personal", href: "/settings/personal" },
    { label: "Company", href: "/settings/company" },
    { label: "Roles & permissions", href: "/settings/roles-permissions" },
];

export default function SettingsLayout() {
    return (
        <div className="flex min-h-[calc(100vh-73px)] flex-col bg-[#f8fafc] md:flex-row">
            <nav
                aria-label="Settings sections"
                className="flex shrink-0 gap-1 overflow-x-auto border-b border-[#e9edf2] bg-white p-3 md:w-[200px] md:flex-col md:overflow-visible md:border-r md:border-b-0"
            >
                {sections.map((section) => (
                    <NavLink
                        key={section.href}
                        to={section.href}
                        className={({ isActive }) =>
                            `whitespace-nowrap rounded-lg px-3 py-2 text-sm ${
                                isActive
                                    ? "bg-[#fdecea] font-semibold text-[#b93f38]"
                                    : "text-[#16232e] hover:bg-slate-100"
                            }`
                        }
                    >
                        {section.label}
                    </NavLink>
                ))}
            </nav>
            <div className="min-w-0 flex-1">
                <Outlet />
            </div>
        </div>
    );
}
