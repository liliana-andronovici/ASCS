import { PageHeader } from "../layout/PageHeader";
import { useCurrentEmployee } from "../features/account/useCurrentEmployee";

function getGreeting(hour: number): string {
    if (hour < 12) return "Good morning";
    if (hour < 18) return "Good afternoon";
    return "Good evening";
}

export default function HomePage() {
    const { employee } = useCurrentEmployee();
    const now = new Date();
    const greeting = getGreeting(now.getHours());
    const dateLabel = now.toLocaleDateString("en-US", {
        weekday: "long",
        day: "numeric",
        month: "long",
    });

    return (
        <>
            <PageHeader title={`${greeting}, ${employee.firstName}`} subtitle={dateLabel} />
            <div className="p-6 text-sm text-gray-500">Home dashboard, Coming soon ...</div>
        </>
    );
}