import { formatDateRange } from "./dates";
import type { TimeOffRequest, TimeOffStatus } from "./types";

type RequestsTableProps = {
    requests: TimeOffRequest[];
    deletingId: number | null;
    onDelete: (id: number) => void;
};

const statusStyles: Record<TimeOffStatus, string> = {
    Approved: "bg-coral/10 text-coral",
    Pending: "border border-gray-300 text-navy",
    Declined: "bg-gray-100 text-gray-600",
};

const headers = ["Type", "Dates", "Note", "Status"];

export function RequestsTable({ requests, deletingId, onDelete }: RequestsTableProps) {
    if (requests.length === 0) {
        return <p className="text-sm text-gray-500">No requests yet.</p>;
    }

    return (
        <table className="w-full text-left text-sm">
            <thead>
            <tr className="border-b border-gray-200 text-xs uppercase tracking-wide text-gray-500">
                {headers.map((header) => (
                    <th key={header} className="py-2 font-medium">
                        {header}
                    </th>
                ))}
                <th />
            </tr>
            </thead>
            <tbody>
            {requests.map((request) => (
                <tr key={request.id} className="border-b border-gray-100">
                    <td className="py-3 font-semibold text-navy">{request.type}</td>
                    <td className="py-3">{formatDateRange(request.startDateUtc, request.endDateUtc)}</td>
                    <td className="py-3">{request.note}</td>
                    <td className="py-3">
                            <span
                                className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${statusStyles[request.status]}`}
                            >
                                {request.status}
                            </span>
                    </td>
                    <td className="py-3 text-right">
                        {request.status === "Pending" && (
                            <button
                                type="button"
                                onClick={() => onDelete(request.id)}
                                disabled={deletingId === request.id}
                                className="text-xs text-coral hover:underline disabled:opacity-50"
                            >
                                Delete
                            </button>
                        )}
                    </td>
                </tr>
            ))}
            </tbody>
        </table>
    );
}
