import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import { AuthLayer } from "../features/auth/AuthLayer";
import LoginPage from "../features/auth/LoginPage";
import PlaceholderPage from "../pages/PlaceholderPage";
import PeoplePage from "../features/employees/PeoplePage";

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
            { path: "settings", element: <PlaceholderPage title="Settings" /> },
        ],
    },
    {
        element: <AuthLayer />,
        children: [{ path: "sign-in", element: <LoginPage /> }],
    },
]);