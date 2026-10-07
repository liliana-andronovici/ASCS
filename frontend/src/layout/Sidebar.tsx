import { useState } from "react";
import { Link } from "react-router-dom";
import logoImage from "../assets/logo.svg";
import {
    ChevronRight,
    ChevronLeft,
    Home,
    Users,
    MapPin,
    Network,
    Calendar,
    DoorClosed,
    UserPlus,
    Settings,
} from "lucide-react";


const NAV_ITEMS = [
    { label: "Home", href: "/", icon: Home },
    { label: "People", href: "/people", icon: Users },
    { label: "Locations", href: "/locations", icon: MapPin },
    { label: "Org Chart", href: "/orgChart", icon: Network },
    { label: "Time Off", href: "/timeOff", icon: Calendar },
    { label: "Rooms", href: "/rooms", icon: DoorClosed },
    { label: "Onboarding", href: "/onboarding", icon: UserPlus },
    { label: "Settings", href: "/settings", icon: Settings },
];

export function Sidebar() {
    const [collapsed, setCollapsed] = useState(false);

    return (
        <aside 
            className={`
            sticky top-0 h-screen bg-navy text-white flex flex-col transition-all duration-300 ease-in-out shrink-0 z-30 ${collapsed ? "w-16" : "w-52"}
            `}
        >
            <div 
                className={`
                flex items-center border-b border-white/20 p-4 h-16 ${collapsed ? "justify-center" : "justify-between"}
                `}
            >
                {!collapsed && <img src={logoImage} alt="logo" className="h-8 w-auto"/>}
                <button
                    onClick={() => setCollapsed((v) => !v)}
                    className="text-white/70 hover:text-white p-2 rounded-md hover:bg-coral"
                    title={collapsed ? "open" : "close"}
                >
                    {collapsed ? <ChevronRight size={20}/> : <ChevronLeft size={20}/>}
                </button>
            </div>

            <nav className="flex flex-col gap-1 px-2">
                {NAV_ITEMS.map((item) => {
                    const Icon = item.icon; 

                    return (
                        <Link to={item.href}
                            key={item.href}
                            className={`flex items-center gap-3 rounded-md px-3 py-2 text-sm text-white/80 hover:bg-white/10 ${collapsed ? "justify-center" : ""}`}
                            title={collapsed ? item.label : undefined}
                        >
                            <Icon size={20} />
                            {!collapsed && <span>{item.label}</span>}
                        </Link>
                    );
                })}
            </nav>
        </aside>
    );
}