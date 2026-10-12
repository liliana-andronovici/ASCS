import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { useCreateDepartment, useUpdateDepartment } from "./useDepartmentMutations";
import { departmentFormSchema, type DepartmentFormValues } from "./departmentSchema";
import type { Department } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    department?: Department;
};

export function DepartmentFormModal({ open, onClose, department }: Props) {
    const isEditing = !!department;
    const createMutation = useCreateDepartment();
    const updateMutation = useUpdateDepartment();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<DepartmentFormValues>({
        resolver: zodResolver(departmentFormSchema),
        defaultValues: { name: department?.name ?? "" },
    });

    const isPending = createMutation.isPending || updateMutation.isPending;
    const serverError = createMutation.error ?? updateMutation.error;

    function handleClose() {
        if (!isPending) onClose();
    }

    function onSubmit(values: DepartmentFormValues) {
        if (isPending) return;

        if (department) {
            updateMutation.mutate({ id: department.id, input: values }, { onSuccess: onClose });
        } else {
            createMutation.mutate(values, { onSuccess: onClose });
        }
    }

    return (
        <Modal open={open} onClose={handleClose} title={isEditing ? "Edit department" : "Add department"}>
            <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
                <div>
                    <Input
                        id="department-name"
                        label="Name"
                        autoFocus
                        disabled={isPending}
                        aria-invalid={!!errors.name}
                        aria-describedby={errors.name ? "department-name-error" : undefined}
                        {...register("name")}
                    />
                    {errors.name && (
                        <p id="department-name-error" className="text-sm text-red-600 mt-1">
                            {errors.name.message}
                        </p>
                    )}
                </div>

                {serverError && (
                    <p role="alert" className="text-sm text-red-600">
                        {serverError.message || "Something went wrong. Please try again."}
                    </p>
                )}

                <div className="flex gap-2 pt-2">
                    <Button type="button" variant="secondary" onClick={handleClose} disabled={isPending}>
                        Cancel
                    </Button>
                    <Button type="submit" disabled={isPending}>
                        {isPending ? "Saving..." : isEditing ? "Save changes" : "Add department"}
                    </Button>
                </div>
            </form>
        </Modal>
    );
}
