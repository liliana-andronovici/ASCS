import { createBrowserRouter, Navigate } from "react-router-dom";
import App from "../App";
import { AuthLayer } from "../features/auth/AuthLayer";
import {
    AcceptInvitePage,
    MagicLinkPage,
    ResetPasswordPage,
    TwoFactorPage,
} from "../features/auth/AuthFlowPages";
import LoginPage from "../features/auth/LoginPage";
import { RequireAuth } from "../features/auth/RequireAuth";
import PlaceholderPage from "../pages/PlaceholderPage";
import HomePage from "../pages/HomePage";
import PeoplePage from "../features/employees/PeoplePage";
import OrgChartPage from "../features/orgChart/OrgChartPage";
import DepartmentsPage from "../features/departments/DepartmentsPage";
import LocationsPage from "../features/locations/LocationsPage";
import SettingsPage from "../features/account/SettingsPage";
import RoomsPage from "../features/rooms/RoomsPage";

export const router = createBrowserRouter([
    {
        element: <RequireAuth />,
        children: [{
            path: "/",
            element: <App />,
            children: [
                { index: true, element: <HomePage /> },
                { path: "people", element: <PeoplePage /> },
                { path: "departments", element: <DepartmentsPage /> },
                { path: "locations", element: <LocationsPage /> },
                { path: "orgChart", element: <OrgChartPage /> },
                { path: "timeOff", element: <PlaceholderPage title="Time off" /> },
                { path: "rooms", element: <RoomsPage /> },
                { path: "onboarding", element: <PlaceholderPage title="Onboarding" /> },
                { path: "settings", element: <SettingsPage /> },
            ],
        }],
    },
    {
        element: <AuthLayer />,
        children: [
            { path: "sign-in", element: <LoginPage /> },
            { path: "two-factor", element: <TwoFactorPage /> },
            { path: "accept-invite", element: <AcceptInvitePage /> },
            { path: "reset-password", element: <ResetPasswordPage /> },
            { path: "forgot-password", element: <Navigate to="/reset-password" replace /> },
            { path: "magic-link", element: <MagicLinkPage /> },
        ],
    },
]);
