import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import { AuthLayer } from "../features/auth/AuthLayer";
import LoginPage from "../features/auth/LoginPage";
import PlaceholderPage from "../pages/PlaceholderPage";
import HomePage from "../pages/HomePage";
import PeoplePage from "../features/employees/PeoplePage";
import RoomsPage from "../features/rooms/RoomsPage";
import SettingsPage from "../features/account/SettingsPage";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            { index: true, element: <HomePage /> },
            { path: "people", element: <PeoplePage /> },
            { path: "orgChart", element: <PlaceholderPage title="Org chart" /> },
            { path: "timeOff", element: <PlaceholderPage title="Time off" /> },
            { path: "rooms", element: <RoomsPage /> },
            { path: "onboarding", element: <PlaceholderPage title="Onboarding" /> },
            { path: "settings", element: <SettingsPage /> },
        ],
    },
    {
        element: <AuthLayer />,
        children: [{ path: "sign-in", element: <LoginPage /> }],
    },
]);