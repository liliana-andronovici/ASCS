import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import { Navigate } from "react-router-dom";
import { AuthLayer } from "../features/auth/AuthLayer";
import LoginPage from "../features/auth/LoginPage";
import PlaceholderPage from "../pages/PlaceholderPage";
import PeoplePage from "../features/employees/PeoplePage";
import RolesPermissionsPage from "../features/roles/RolesPermissionsPage";
import SettingsLayout from "../features/settings/SettingsLayout";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            { index: true, element: <PlaceholderPage title="Home" /> },
            { path: "people", element: <PeoplePage /> },
            { path: "orgChart", element: <PlaceholderPage title="Org chart" /> },
            { path: "timeOff", element: <PlaceholderPage title="Time off" /> },
            { path: "rooms", element: <PlaceholderPage title="Rooms" /> },
            { path: "onboarding", element: <PlaceholderPage title="Onboarding" /> },
            {
                path: "settings",
                element: <SettingsLayout />,
                children: [
                    { index: true, element: <Navigate to="roles-permissions" replace /> },
                    { path: "company", element: <PlaceholderPage title="Company" /> },
                    { path: "roles-permissions", element: <RolesPermissionsPage /> },
                    { path: "time-off-policies", element: <PlaceholderPage title="Time off policies" /> },
                    { path: "authentication", element: <PlaceholderPage title="Authentication" /> },
                    { path: "integrations", element: <PlaceholderPage title="Integrations" /> },
                    { path: "billing", element: <PlaceholderPage title="Billing" /> },
                ],
            },
        ],
    },
    {
        element: <AuthLayer />,
        children: [{ path: "sign-in", element: <LoginPage /> }],
    },
]);
