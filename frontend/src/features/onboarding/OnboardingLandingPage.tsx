import { PageHeader } from "../../layout/PageHeader";

export default function OnboardingLandingPage() {
    return (
        <div className="p-6">
            <PageHeader title="Onboarding" />
            <p className="mt-4 text-sm text-gray-600">
                Open your employee-specific onboarding link to view your checklist.
            </p>
        </div>
    );
}
