import { useState } from "react";
import { Button } from "../../components/Button";
import { PageHeader } from "../../layout/PageHeader";
import { getErrorMessage } from "../../lib/errors";
import { NewRequestPanel } from "./NewRequestPanel";
import { RequestsTable } from "./RequestsTable";
import { useDeleteTimeOffRequest, useTimeOffRequests } from "./useTimeOff";

export default function TimeOffPage() {
    const [panelOpen, setPanelOpen] = useState(false);
    const requests = useTimeOffRequests();
    const remove = useDeleteTimeOffRequest();
    const deletingId = remove.isPending ? remove.variables : null;

    return (
        <div className="flex min-h-0 flex-1 flex-col">
            <PageHeader title="Time off">
                <div className="w-44">
                    <Button type="button" onClick={() => setPanelOpen(true)}>
                        Request time off
                    </Button>
                </div>
            </PageHeader>

            <div className="flex min-w-0 flex-1 flex-col">
                <section className="flex-1 px-6 py-5">
                    <h2 className="mb-2 text-base font-semibold text-navy">Your requests</h2>
                    {requests.isPending && <p className="text-sm text-gray-500">Loading requests…</p>}
                    {requests.isError && (
                        <p className="text-sm text-coral">{getErrorMessage(requests.error)}</p>
                    )}
                    {requests.data && (
                        <RequestsTable
                            requests={requests.data}
                            deletingId={deletingId}
                            onDelete={(id) => remove.mutate(id)}
                        />
                    )}
                    {remove.isError && (
                        <p className="mt-2 text-sm text-coral">{getErrorMessage(remove.error)}</p>
                    )}
                </section>
            </div>

            <NewRequestPanel open={panelOpen} onClose={() => setPanelOpen(false)} />
        </div>
    );
}
