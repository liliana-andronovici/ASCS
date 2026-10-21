import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { getErrorMessage } from "../../lib/errors";
import { TIME_OFF_TYPES } from "./types";
import type { TimeOffRequestInput } from "./types";
import { useCreateTimeOffRequest } from "./useTimeOff";

type NewRequestPanelProps = {
    open: boolean;
    onClose: () => void;
};

const emptyForm: TimeOffRequestInput = {
    type: "Vacation",
    startDate: "",
    endDate: "",
    note: "",
};

export function NewRequestPanel({ open, onClose }: NewRequestPanelProps) {
    const [form, setForm] = useState<TimeOffRequestInput>(emptyForm);
    const create = useCreateTimeOffRequest();

    useEffect(() => {
        if (!open) return;
        function handleKeyDown(event: KeyboardEvent) {
            if (event.key === "Escape") onClose();
        }
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [open, onClose]);

    function update<K extends keyof TimeOffRequestInput>(key: K, value: TimeOffRequestInput[K]) {
        setForm((previous) => ({ ...previous, [key]: value }));
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        create.mutate(form, {
            onSuccess: () => {
                setForm(emptyForm);
                onClose();
            },
        });
    }

    return (
        <>
            <div
                onClick={onClose}
                className={`fixed inset-0 z-40 bg-black/40 transition-opacity duration-200 ${
                    open ? "opacity-100" : "pointer-events-none opacity-0"
                }`}
            />
            <form
                onSubmit={handleSubmit}
                aria-hidden={!open}
                className={`fixed right-0 top-0 z-50 flex h-full w-full max-w-[380px] flex-col gap-4 overflow-y-auto bg-slate-50 px-6 py-5 shadow-card transition-[transform,visibility] duration-200 ${
                    open ? "translate-x-0" : "invisible translate-x-full"
                }`}
            >
                <div className="flex items-center justify-between">
                    <h2 className="text-lg font-semibold text-navy">New request</h2>
                    <button
                        type="button"
                        onClick={onClose}
                        aria-label="Close"
                        className="text-gray-400 hover:text-gray-600"
                    >
                        ✕
                    </button>
                </div>

                <fieldset className="flex flex-col gap-1">
                    <legend className="mb-1 text-sm text-gray-700">Type</legend>
                    <div className="flex overflow-hidden rounded-md border border-gray-300">
                        {TIME_OFF_TYPES.map((type) => (
                            <label
                                key={type}
                                className={`flex-1 cursor-pointer px-3 py-2 text-center text-sm ${
                                    form.type === type ? "bg-navy text-white" : "bg-white text-navy"
                                }`}
                            >
                                <input
                                    type="radio"
                                    name="time-off-type"
                                    value={type}
                                    checked={form.type === type}
                                    onChange={() => update("type", type)}
                                    className="sr-only"
                                />
                                {type}
                            </label>
                        ))}
                    </div>
                </fieldset>

                <div className="grid grid-cols-2 gap-3">
                    <Input
                        id="time-off-from"
                        label="From"
                        type="date"
                        value={form.startDate}
                        onChange={(event) => update("startDate", event.target.value)}
                        required
                    />
                    <Input
                        id="time-off-to"
                        label="To"
                        type="date"
                        min={form.startDate}
                        value={form.endDate}
                        onChange={(event) => update("endDate", event.target.value)}
                        required
                    />
                </div>

                <div className="flex flex-col gap-1">
                    <label htmlFor="time-off-note" className="text-sm text-gray-700">
                        Note
                    </label>
                    <textarea
                        id="time-off-note"
                        rows={3}
                        value={form.note}
                        onChange={(event) => update("note", event.target.value)}
                        required
                        maxLength={500}
                        className="min-h-[70px] rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                    />
                </div>

                {create.isError && <p className="text-sm text-coral">{getErrorMessage(create.error)}</p>}

                <Button type="submit" disabled={create.isPending}>
                    {create.isPending ? "Submitting…" : "Submit request"}
                </Button>
            </form>
        </>
    );
}
