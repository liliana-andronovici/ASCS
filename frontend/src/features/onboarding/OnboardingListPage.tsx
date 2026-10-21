import { useNavigate } from "react-router-dom";
import { Avatar } from "../../components/Avatar";
import { PageHeader } from "../../layout/PageHeader";
import { getErrorMessage } from "../../lib/errors";
import { useOnboardingSummaries } from "./useOnboarding";

function formatStartDate(iso: string) {
    return new Date(iso).toLocaleDateString("en-US", { day: "numeric", month: "short", year: "numeric" });
}

export default function OnboardingListPage() {
    const navigate = useNavigate();
    const { data, isPending, isError, error } = useOnboardingSummaries();

    if (isPending) return <div className="p-6 text-sm text-gray-500">Loading onboarding…</div>;
    if (isError) return <div className="p-6 text-sm text-coral">{getErrorMessage(error)}</div>;

    return (
        <div className="p-6">
            <PageHeader title="Onboarding" />
            <div className="mt-4 rounded-lg border border-gray-200 bg-white shadow-card">
                <table className="w-full text-left text-sm">
                    <thead>
                    <tr className="border-b border-gray-200 text-gray-500">
                        <th className="py-3 pl-6 font-medium">Name</th>
                        <th className="py-3 font-medium">Department</th>
                        <th className="py-3 font-medium">Starts</th>
                        <th className="py-3 pr-6 font-medium">Progress</th>
                    </tr>
                    </thead>
                    <tbody>
                    {data.map((row) => (
                        <tr
                            key={row.employeeId}
                            onClick={() => navigate(`/onboarding/team/${row.employeeId}`)}
                            className="cursor-pointer border-b border-gray-100 last:border-0 hover:bg-gray-50"
                        >
                            <td className="py-3 pl-6">
                                <div className="flex items-center gap-3">
                                    <Avatar firstName={row.firstName} lastName={row.lastName} />
                                    <div>
                                        <div className="font-semibold text-navy">
                                            {row.firstName} {row.lastName}
                                        </div>
                                        <div className="text-gray-500">{row.title}</div>
                                    </div>
                                </div>
                            </td>
                            <td className="py-3 text-gray-700">{row.department}</td>
                            <td className="py-3 text-gray-500">{formatStartDate(row.startsOn)}</td>
                            <td className="py-3 pr-6">
                                {row.isComplete ? (
                                    <span className="text-sm font-medium text-navy">Complete</span>
                                ) : (
                                    <span className="text-sm text-gray-500">
                                            {row.completedTaskCount}/{row.totalTaskCount} done
                                        </span>
                                )}
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
