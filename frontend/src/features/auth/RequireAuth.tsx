import { Navigate, Outlet } from "react-router-dom";
import { getToken } from "../../lib/sessions";

export function RequireAuth() {
    if (!getToken()) {
        return <Navigate to="/sign-in" replace />;
    }
    return <Outlet />;
}
