import { useState } from "react";
import { Button } from "../../components/Button";
import { PageHeader } from "../../layout/PageHeader";
import { getErrorMessage } from "../../lib/errors";
import { BalanceCards } from "./BalanceCards";
import { formatShortDate } from "./dates";
import { NewRequestPanel } from "./NewRequestPanel";
import { RequestsTable } from "./RequestsTable";
import {
    useTimeOffOverview,
    useTimeOffRequests,
    useWithdrawTimeOffRequest,
} from "./useTimeOff";

export default function TimeOffPage() {
    const [panelOpen, setPanelOpen] = useState(false);
    const overview = useTimeOffOverview();
    const requests = useTimeOffRequests();
    const withdraw = useWithdrawTimeOffRequest();

    if (overview.isPending) {
        return <div className="p-6 text-sm text-gray-500">Loading time off…</div>;
    }

    if (overview.isError) {
        return <div className="p-6 text-sm text-coral">{getErrorMessage(overview.error)}</div>;
    }

    const { policyName, resetsOn, approverName, balances } = overview.data;
    const withdrawingId = withdraw.isPending ? withdraw.variables : null;

    return (
        <div className="flex min-h-0 flex-1 flex-col">
            <PageHeader title="Time off">
                <div className="flex items-center gap-4">
                    <span className="text-sm text-gray-500">
                        Policy: {policyName} · resets {formatShortDate(resetsOn)}
                    </span>
                    <div className="w-44">
                        <Button type="button" onClick={() => setPanelOpen(true)}>
                            Request time off
                        </Button>
                    </div>
                </div>
            </PageHeader>

            <div className="flex min-w-0 flex-1 flex-col">
                <BalanceCards balances={balances} />
                <section className="flex-1 px-6 py-5">
                    <h2 className="mb-2 text-base font-semibold text-navy">Your requests</h2>
                    {requests.isPending && <p className="text-sm text-gray-500">Loading requests…</p>}
                    {requests.isError && (
                        <p className="text-sm text-coral">{getErrorMessage(requests.error)}</p>
                    )}
                    {requests.data && (
                        <RequestsTable
                            requests={requests.data}
                            withdrawingId={withdrawingId}
                            onWithdraw={(id) => withdraw.mutate(id)}
                        />
                    )}
                    {withdraw.isError && (
                        <p className="mt-2 text-sm text-coral">{getErrorMessage(withdraw.error)}</p>
                    )}
                </section>
            </div>

            <NewRequestPanel
                open={panelOpen}
                onClose={() => setPanelOpen(false)}
                approverName={approverName}
            />
        </div>
    );
}