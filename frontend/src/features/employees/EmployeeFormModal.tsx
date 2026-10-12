import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { Select } from "../../components/Select";
import { useDepartments } from "../departments/useDepartments";
import { useLocations } from "../locations/useLocations";
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
    const [departmentId, setDepartmentId] = useState<number | null>(employee?.departmentId ?? null);
    const [locationId, setLocationId] = useState<number | null>(employee?.locationId ?? null);
    const { data: departments, isLoading: isDepartmentsLoading, isError: isDepartmentsError } = useDepartments();
    const { data: locations, isLoading: isLocationsLoading, isError: isLocationsError } = useLocations();
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
            departmentId,
            locationId,
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
                    <Select
                        id="employee-department"
                        label="Department"
                        value={departmentId ?? ""}
                        onChange={(event) => setDepartmentId(event.target.value ? Number(event.target.value) : null)}
                        disabled={isDepartmentsLoading || isDepartmentsError || !departments || isPending}
                        aria-describedby={isDepartmentsError ? "employee-department-error" : undefined}
                        options={[
                            { value: "", label: isDepartmentsLoading ? "Loading departments..." : "No department" },
                            ...(departments ?? []).map((department) => ({
                                value: String(department.id),
                                label: department.name,
                            })),
                        ]}
                    />
                    {isDepartmentsError && (
                        <p id="employee-department-error" role="alert" className="text-sm text-red-600 mt-1">
                            {isEditing ? "Couldn't load departments. Please try again to change the department." : "Couldn't load departments. You can add this person without a department."}
                        </p>
                    )}
                </div>
                <div>
                    <Select
                        id="employee-location"
                        label="Location"
                        value={locationId ?? ""}
                        onChange={(event) => setLocationId(event.target.value ? Number(event.target.value) : null)}
                        disabled={isLocationsLoading || isLocationsError || !locations || isPending}
                        aria-describedby={isLocationsError ? "employee-location-error" : undefined}
                        options={[
                            { value: "", label: isLocationsLoading ? "Loading locations..." : "No location" },
                            ...(locations ?? []).map((location) => ({
                                value: String(location.id),
                                label: location.name,
                            })),
                        ]}
                    />
                    {isLocationsError && (
                        <p id="employee-location-error" role="alert" className="text-sm text-red-600 mt-1">
                            {isEditing ? "Couldn't load locations. Please try again to change the location." : "Couldn't load locations. You can add this person without a location."}
                        </p>
                    )}
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