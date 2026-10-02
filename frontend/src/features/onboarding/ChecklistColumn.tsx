import { Checkbox } from "../../components/Checkbox";
import { getTaskDisplayNote, getTaskStatus } from "./taskStatus";
import type { OnboardingSection } from "./types";

type ChecklistColumnProps = {
    section: OnboardingSection;
    onToggle: (taskId: number) => void;
    togglingId: number | null;
};

export function ChecklistColumn({ section, onToggle, togglingId }: ChecklistColumnProps) {
    const done = section.tasks.filter((t) => getTaskStatus(t) === "done").length;

    return (
        <div className="flex flex-col gap-3">
            <div className="flex items-baseline justify-between">
                <h3 className="text-sm font-semibold text-navy">{section.title}</h3>
                <span className="text-xs text-gray-500">
                    {done}/{section.tasks.length}
                </span>
            </div>

            <ul className="flex flex-col gap-3">
                {section.tasks.map((task) => {
                    const status = getTaskStatus(task);
                    const note = getTaskDisplayNote(task);
                    return (
                        <li key={task.id} className="flex items-start gap-2.5">
                            <Checkbox
                                checked={status === "done"}
                                disabled={togglingId === task.id}
                                onChange={() => onToggle(task.id)}
                                className="mt-0.5"
                            />
                            <div className="flex flex-col">
                                <span
                                    className={`text-sm ${
                                        status === "done" ? "text-gray-400 line-through" : "text-navy"
                                    }`}
                                >
                                    {task.title}
                                </span>
                                {note && (
                                    <span className={`text-xs ${status === "overdue" ? "text-coral" : "text-gray-500"}`}>
                                        {note}
                                    </span>
                                )}
                            </div>
                        </li>
                    );
                })}
            </ul>
        </div>
    );
}