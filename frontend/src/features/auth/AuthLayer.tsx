import { Outlet } from "react-router-dom";
import logoImage from "../../assets/logo.svg";

export function AuthLayer() {
    return( 
        <div className="flex min-h-screen items-center justify-center bg-gray-50 p-4">
        <div className="w-full max-w-[420px] overflow-hidden rounded-lg shadow-card">
            <div className="flex items-center justify-center bg-navy p-6">
                <img src={logoImage} alt="Infigo HR" className="h-12 w-auto" />
            </div>
            <div className="bg-white p-8">
                <Outlet />
            </div>
        </div>
    </div>);
}
