import { useState } from "react";
import type { FormEvent } from "react";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { getErrorMessage } from "../../lib/errors";
import { useAddOnboardingTaskFor } from "./useOnboarding";
import type { OnboardingSection } from "./types";

type AddTaskModalProps = {
    employeeId: number;
    open: boolean;
    onClose: () => void;
    sections: OnboardingSection[];
};

export function AddTaskModal({ employeeId, open, onClose, sections }: AddTaskModalProps) {
    const addTask = useAddOnboardingTaskFor(employeeId);
    const [sectionKey, setSectionKey] = useState<OnboardingSection["key"]>(sections[0]?.key ?? "BeforeDayOne");
    const [title, setTitle] = useState("");
    const [note, setNote] = useState("");
    const [dueDate, setDueDate] = useState("");

    function reset() {
        setTitle("");
        setNote("");
        setDueDate("");
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        addTask.mutate(
            { sectionKey, title, note: note || undefined, dueDate },
            { onSuccess: () => { reset(); onClose(); } }
        );
    }

    return (
        <Modal open={open} onClose={onClose} title="Add task">
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <div className="flex flex-col gap-1">
                    <label htmlFor="task-section" className="text-sm text-gray-700">Section</label>
                    <select
                        id="task-section"
                        value={sectionKey}
                        onChange={(event) => setSectionKey(event.target.value as OnboardingSection["key"])}
                        className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                    >
                        {sections.map((section) => (
                            <option key={section.key} value={section.key}>{section.title}</option>
                        ))}
                    </select>
                </div>

                <Input id="task-title" label="Task" value={title} onChange={(event) => setTitle(event.target.value)} required />
                <Input id="task-note" label="Note (optional)" value={note} onChange={(event) => setNote(event.target.value)} placeholder="e.g. Tue 15 Sep 9:00" />
                <Input id="task-due-date" label="Due date (optional)" type="date" value={dueDate} onChange={(event) => setDueDate(event.target.value)} />

                {addTask.isError && <p className="text-sm text-coral">{getErrorMessage(addTask.error)}</p>}

                <Button type="submit" disabled={addTask.isPending}>
                    {addTask.isPending ? "Adding…" : "Add task"}
                </Button>
            </form>
        </Modal>
    );
}
