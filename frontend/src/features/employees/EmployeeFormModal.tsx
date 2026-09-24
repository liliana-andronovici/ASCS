import { useState } from "react";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Select } from "../../components/Select";
import { Button } from "../../components/Button";
import { useCreateEmployee, useUpdateEmployee } from "./useEmployeeMutations";
import type { Employee } from "./types";
import type { CreateEmployeeInput } from "./api";

type Props = {
    open: boolean;
    onClose: () => void;
    employee?: Employee;
};

export function EmployeeFormModal({ open, onClose, employee }: Props) {
    const isEditing = !!employee;
    const createMutation = useCreateEmployee();
    const updateMutation = useUpdateEmployee();

    const [form, setForm] = useState<CreateEmployeeInput>({
        firstName: employee?.firstName ?? "",
        lastName: employee?.lastName ?? "",
        email: employee?.email ?? "",
        title: employee?.title ?? "",
        startedOnUtc: employee?.startedOnUtc?.slice(0, 10) ?? new Date().toISOString().slice(0, 10),
        departmentId: employee?.departmentId ?? null,
        locationId: employee?.locationId ?? null,
        managerId: employee?.managerId ?? null,
        status: employee?.status ?? "Active",
    });

    const isPending = createMutation.isPending || updateMutation.isPending;
    const activeError = createMutation.error ?? updateMutation.error;

    function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (isEditing) {
            updateMutation.mutate({ id: employee.id, input: form }, { onSuccess: onClose });
        } else {
            createMutation.mutate(form, { onSuccess: onClose });
        }
    }

    return (
        <Modal open={open} onClose={onClose} title={isEditing ? "Edit employee" : "Add person"}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <Input
                    label="First name"
                    value={form.firstName}
                    onChange={(e) => setForm({ ...form, firstName: e.target.value })}
                    required
                />
                <Input
                    label="Last name"
                    value={form.lastName}
                    onChange={(e) => setForm({ ...form, lastName: e.target.value })}
                    required
                />
                <Input
                    label="Email"
                    type="email"
                    value={form.email}
                    onChange={(e) => setForm({ ...form, email: e.target.value })}
                    required
                />
                <Input
                    label="Title"
                    value={form.title}
                    onChange={(e) => setForm({ ...form, title: e.target.value })}
                    required
                />
                <Select
                    label="Status"
                    value={form.status}
                    onChange={(e) => setForm({ ...form, status: e.target.value as "Active" | "Inactive" })}
                    options={[
                        { value: "Active", label: "Active" },
                        { value: "Inactive", label: "Inactive" },
                    ]}
                />
                <div className="flex gap-2 pt-2">
                    <Button type="button" variant="secondary" onClick={onClose}>
                        Cancel
                    </Button>
                    <Button type="submit" disabled={isPending}>
                        {isPending ? "Saving..." : isEditing ? "Save changes" : "Add person"}
                    </Button>
                    {activeError && (
                        <p className="text-sm text-red-600">
                            {(activeError as Error).message || "Something went wrong. Please try again."}
                        </p>
                    )}
                </div>
            </form>
        </Modal>
    );
}