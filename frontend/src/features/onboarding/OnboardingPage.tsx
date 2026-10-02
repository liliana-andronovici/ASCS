import { useState } from "react";
import { Avatar } from "../../components/Avatar";
import { Button } from "../../components/Button";
import { PageHeader } from "../../layout/PageHeader";
import { getErrorMessage } from "../../lib/errors";
import { AddTaskModal } from "./AddTaskModal";
import { ChecklistColumn } from "./ChecklistColumn";
import { useOnboardingChecklist, useToggleOnboardingTask } from "./useOnboarding";
import { getTaskStatus } from "./taskStatus";

function formatStartDate(iso: string) {
    return new Date(iso).toLocaleDateString("en-US", {
        weekday: "short",
        day: "numeric",
        month: "short",
    });
}

export default function OnboardingPage() {
    const [addTaskOpen, setAddTaskOpen] = useState(false);
    const checklist = useOnboardingChecklist();
    const toggle = useToggleOnboardingTask();

    if (checklist.isPending) {
        return <div className="p-6 text-sm text-gray-500">Loading onboarding…</div>;
    }

    if (checklist.isError) {
        return <div className="p-6 text-sm text-coral">{getErrorMessage(checklist.error)}</div>;
    }

    const data = checklist.data;
    const allTasks = data.sections.flatMap((s) => s.tasks);
    const doneCount = allTasks.filter((t) => getTaskStatus(t) === "done").length;
    const overdueCount = allTasks.filter((t) => getTaskStatus(t) === "overdue").length;
    const totalCount = allTasks.length;
    const donePct = totalCount ? (doneCount / totalCount) * 100 : 0;
    const overduePct = totalCount ? (overdueCount / totalCount) * 100 : 0;
    const togglingId = toggle.isPending ? toggle.variables : null;

    return (
        <div className="flex min-h-0 flex-1 flex-col">
            <PageHeader title="Onboarding">
                <div className="flex gap-2">
                    <Button
                        type="button"
                        variant="secondary"
                        className="!w-auto px-4"
                        disabled
                        title="Templates aren't built yet"
                    >
                        Change template
                    </Button>
                    <Button
                        type="button"
                        className="!w-auto px-4"
                        onClick={() => setAddTaskOpen(true)}
                    >
                        Add task
                    </Button>
                </div>
            </PageHeader>

            <div className="p-6">
                <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-card">
                    <div className="flex flex-wrap items-start justify-between gap-4 border-b border-gray-200 pb-5">
                        <div className="flex items-start gap-3">
                            <Avatar firstName={data.firstName} lastName={data.lastName} />
                            <div>
                                <div className="text-xs font-semibold uppercase tracking-wide text-gray-400">
                                    Starts {formatStartDate(data.startsOn)} · {data.department} · {data.location}
                                </div>
                                <div className="text-lg font-semibold text-navy">
                                    {data.firstName} {data.lastName}
                                </div>
                                <div className="text-sm text-gray-500">
                                    {data.title} · manager {data.managerName} · buddy {data.buddyName}
                                </div>
                            </div>
                        </div>

                        <div className="flex flex-col items-end gap-1">
                            <div className="flex items-baseline gap-3 text-sm">
                                <span className="text-navy">
                                    {doneCount} of {totalCount} tasks done
                                </span>
                                {overdueCount > 0 && (
                                    <span className="font-medium text-coral">{overdueCount} overdue</span>
                                )}
                            </div>
                            <div className="h-1.5 w-48 overflow-hidden rounded-full bg-gray-100">
                                <div className="flex h-full">
                                    <div className="h-full bg-navy" style={{ width: `${donePct}%` }} />
                                    <div className="h-full bg-coral" style={{ width: `${overduePct}%` }} />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-8 pt-5 md:grid-cols-2 lg:grid-cols-4">
                        {data.sections.map((section) => (
                            <ChecklistColumn
                                key={section.key}
                                section={section}
                                onToggle={(taskId) => toggle.mutate(taskId)}
                                togglingId={togglingId}
                            />
                        ))}
                    </div>

                    {toggle.isError && (
                        <p className="mt-4 text-sm text-coral">{getErrorMessage(toggle.error)}</p>
                    )}
                </div>
            </div>

            <AddTaskModal open={addTaskOpen} onClose={() => setAddTaskOpen(false)} sections={data.sections} />
        </div>
    );
}