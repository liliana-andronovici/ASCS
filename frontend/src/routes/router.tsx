import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import { AuthLayer } from "../features/auth/AuthLayer";
import LoginPage from "../features/auth/LoginPage";
import PlaceholderPage from "../pages/PlaceholderPage";
import PeoplePage from "../features/employees/PeoplePage";
import TimeOffPage from "../features/timeoff/TimeOffPage";
import OnboardingPage from "../features/onboarding/OnboardingPage";
import OnboardingLandingPage from "../features/onboarding/OnboardingLandingPage";
import OnboardingListPage from "../features/onboarding/OnboardingListPage";
import OnboardingDetailPage from "../features/onboarding/OnboardingDetailPage";
import LocationsPage from "../features/locations/LocationsPage";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            { index: true, element: <PlaceholderPage title="Home" /> },
            { path: "people", element: <PeoplePage /> },
            { path: "locations", element: <LocationsPage /> },
            { path: "orgChart", element: <PlaceholderPage title="Org chart" /> },
            { path: "timeOff", element: <TimeOffPage /> },
            { path: "rooms", element: <PlaceholderPage title="Rooms" /> },
            { path: "settings", element: <PlaceholderPage title="Settings" /> },
            { path: "onboarding", element: <OnboardingLandingPage /> },
            { path: "onboarding/:employeeId", element: <OnboardingPage /> },
            { path: "onboarding/team", element: <OnboardingListPage /> },
            { path: "onboarding/team/:employeeId", element: <OnboardingDetailPage /> },
        ],
    },
    {
        element: <AuthLayer />,
        children: [{ path: "sign-in", element: <LoginPage /> }],
    },
]);
