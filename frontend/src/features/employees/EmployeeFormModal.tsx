import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { useCreateEmployee, useUpdateEmployee } from "./useEmployeeMutations";
import { employeeFormSchema, type EmployeeFormValues } from "./employeeSchema";
import type { Employee } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    employee?: Employee;
};

export function EmployeeFormModal({ open, onClose, employee }: Props) {
    const isEditing = !!employee;
    const createMutation = useCreateEmployee();
    const updateMutation = useUpdateEmployee();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<EmployeeFormValues>({
        resolver: zodResolver(employeeFormSchema),
        defaultValues: {
            firstName: employee?.firstName ?? "",
            lastName: employee?.lastName ?? "",
            email: employee?.email ?? "",
            title: employee?.title ?? "",
            startedOnUtc: employee?.startedOnUtc?.slice(0, 10) ?? "",
        },
    });

    const isPending = createMutation.isPending || updateMutation.isPending;
    const serverError = createMutation.error ?? updateMutation.error;

    function onSubmit(values: EmployeeFormValues) {
        const dateOnly = values.startedOnUtc || new Date().toISOString().slice(0, 10);
        const payload = {
            ...values,
            startedOnUtc: new Date(`${dateOnly}T00:00:00.000Z`).toISOString(),
            departmentId: employee?.departmentId ?? null,
            locationId: employee?.locationId ?? null,
            managerId: employee?.managerId ?? null,
            status: employee?.status ?? ("Active" as const),
        };

        if (isEditing) {
            updateMutation.mutate({ id: employee.id, input: payload }, { onSuccess: onClose });
        } else {
            createMutation.mutate(payload, { onSuccess: onClose });
        }
    }

    return (
        <Modal open={open} onClose={onClose} title={isEditing ? "Edit employee" : "Add person"}>
            <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
                <div>
                    <Input label="First name" {...register("firstName")} />
                    {errors.firstName && <p className="text-sm text-red-600 mt-1">{errors.firstName.message}</p>}
                </div>
                <div>
                    <Input label="Last name" {...register("lastName")} />
                    {errors.lastName && <p className="text-sm text-red-600 mt-1">{errors.lastName.message}</p>}
                </div>
                <div>
                    <Input label="Email" type="email" {...register("email")} />
                    {errors.email && <p className="text-sm text-red-600 mt-1">{errors.email.message}</p>}
                </div>
                <div>
                    <Input label="Title" {...register("title")} />
                    {errors.title && <p className="text-sm text-red-600 mt-1">{errors.title.message}</p>}
                </div>
                <div>
                    <Input label="Start date" type="date" {...register("startedOnUtc")} />
                    {errors.startedOnUtc && <p className="text-sm text-red-600 mt-1">{errors.startedOnUtc.message}</p>}
                </div>

                {serverError && (
                    <p className="text-sm text-red-600">
                        {(serverError as Error).message || "Something went wrong. Please try again."}
                    </p>
                )}

                <div className="flex gap-2 pt-2">
                    <Button type="button" variant="secondary" onClick={onClose}>
                        Cancel
                    </Button>
                    <Button type="submit" disabled={isPending}>
                        {isPending ? "Saving..." : isEditing ? "Save changes" : "Add person"}
                    </Button>
                </div>
            </form>
        </Modal>
    );
}