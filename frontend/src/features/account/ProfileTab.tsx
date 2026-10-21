import { useRef } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useQuery } from "@tanstack/react-query";
import { Camera } from "lucide-react";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { Badge } from "../../components/Badge";
import { apiFetch } from "../../lib/api";
import { setLocalAvatar } from "../../lib/sessions";
import { useCurrentEmployee } from "./useCurrentEmployee";
import { useLocalAvatar } from "./useLocalAvatar";
import { useDepartments } from "../departments/useDepartments";
import { useEmployeeById } from "../employees/useEmployees";
import { useUpdateEmployee } from "../employees/useEmployeeMutations";
import { employeeFormSchema, type EmployeeFormValues } from "../employees/employeeSchema";

function useLocations() {
    return useQuery({
        queryKey: ["locations"],
        queryFn: () => apiFetch<{ id: number; name: string }[]>("/Locations"),
        staleTime: 5 * 60 * 1000,
    });
}

function formatDate(iso: string) {
    return new Date(iso).toLocaleDateString("en-US", { day: "numeric", month: "long", year: "numeric" });
}

function formatTenure(iso: string) {
    const start = new Date(iso);
    const now = new Date();
    let months = (now.getFullYear() - start.getFullYear()) * 12 + (now.getMonth() - start.getMonth());
    if (now.getDate() < start.getDate()) months -= 1;
    if (months < 1) return "Less than a month";
    const years = Math.floor(months / 12);
    const rest = months % 12;
    const parts = [];
    if (years > 0) parts.push(`${years} ${years === 1 ? "year" : "years"}`);
    if (rest > 0) parts.push(`${rest} ${rest === 1 ? "month" : "months"}`);
    return parts.join(" ");
}

function Fact({ label, value }: { label: string; value: string }) {
    return (
        <div>
            <dt className="text-xs text-gray-500">{label}</dt>
            <dd className="mt-0.5 text-sm font-medium text-navy">{value}</dd>
        </div>
    );
}

export function ProfileTab() {
    const { employee, isLoading, isError } = useCurrentEmployee();
    const photoUrl = useLocalAvatar();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const updateMutation = useUpdateEmployee();
    const departments = useDepartments();
    const locations = useLocations();
    const manager = useEmployeeById(employee?.managerId ?? null);

    const {
        register,
        handleSubmit,
        formState: { errors, isDirty },
    } = useForm<EmployeeFormValues>({
        resolver: zodResolver(employeeFormSchema),
        values: employee
            ? {
                  firstName: employee.firstName,
                  lastName: employee.lastName,
                  email: employee.email,
                  title: employee.title,
                  startedOnUtc: employee.startedOnUtc.slice(0, 10),
              }
            : { firstName: "", lastName: "", email: "", title: "", startedOnUtc: "" },
    });

    if (isLoading) return <div className="text-sm text-gray-500">Loading your profile...</div>;
    if (isError || !employee) return <div className="text-sm text-red-600">Couldn't load your profile from the server.</div>;

    const currentEmployee = employee;
    const departmentName = departments.data?.find((d) => d.id === employee.departmentId)?.name;
    const locationName = locations.data?.find((l) => l.id === employee.locationId)?.name;
    const managerName = manager.data ? `${manager.data.firstName} ${manager.data.lastName}` : undefined;
    const initials = `${employee.firstName[0] ?? ""}${employee.lastName[0] ?? ""}`.toUpperCase();

    function handlePhotoChange(e: React.ChangeEvent<HTMLInputElement>) {
        const file = e.target.files?.[0];
        if (!file) return;
        const reader = new FileReader();
        reader.onload = () => {
            if (typeof reader.result === "string") setLocalAvatar(reader.result);
        };
        reader.readAsDataURL(file);
    }

    function onSubmit(values: EmployeeFormValues) {
        const updated = {
            ...currentEmployee,
            ...values,
            startedOnUtc: new Date(`${values.startedOnUtc}T00:00:00.000Z`).toISOString(),
        };
        updateMutation.mutate({
            id: currentEmployee.id,
            input: {
                ...values,
                startedOnUtc: updated.startedOnUtc,
                departmentId: currentEmployee.departmentId,
                locationId: currentEmployee.locationId,
                managerId: currentEmployee.managerId,
                status: currentEmployee.status,
            },
        });
    }

    return (
        <div className="flex max-w-3xl flex-col gap-6">
            <section className="overflow-hidden rounded-lg border border-gray-200">
                <div className="h-24 bg-navy" />
                <div className="flex flex-wrap items-end gap-4 px-6 pb-5">
                    <div className="relative -mt-12">
                        {photoUrl ? (
                            <img src={photoUrl} alt="" className="h-24 w-24 rounded-full border-4 border-white object-cover" />
                        ) : (
                            <div className="flex h-24 w-24 items-center justify-center rounded-full border-4 border-white bg-[#dfe5ec] text-3xl font-semibold text-navy">
                                {initials}
                            </div>
                        )}
                        <input ref={fileInputRef} type="file" accept="image/*" onChange={handlePhotoChange} className="hidden" />
                        <button
                            type="button"
                            onClick={() => fileInputRef.current?.click()}
                            title="Change photo"
                            aria-label="Change photo"
                            className="absolute bottom-0 right-0 flex h-8 w-8 items-center justify-center rounded-full border border-gray-200 bg-white text-navy shadow-card hover:bg-gray-50"
                        >
                            <Camera size={15} />
                        </button>
                    </div>
                    <div className="min-w-0 flex-1 pt-3">
                        <h2 className="truncate text-2xl font-bold text-navy">
                            {employee.firstName} {employee.lastName}
                        </h2>
                        <p className="text-sm text-gray-500">{employee.title}</p>
                    </div>
                    <Badge variant={employee.status === "Active" ? "active" : "inactive"}>{employee.status}</Badge>
                </div>
                <dl className="grid grid-cols-2 gap-x-6 gap-y-4 border-t border-gray-100 px-6 py-5 sm:grid-cols-3">
                    <Fact label="Department" value={departmentName ?? "Not assigned"} />
                    <Fact label="Location" value={locationName ?? "Not assigned"} />
                    <Fact label="Manager" value={managerName ?? "Not assigned"} />
                    <Fact label="Work email" value={employee.email} />
                    <Fact label="Started" value={formatDate(employee.startedOnUtc)} />
                    <Fact label="With Infigo" value={formatTenure(employee.startedOnUtc)} />
                </dl>
            </section>

            <section className="rounded-lg border border-gray-200 p-6">
                <h3 className="text-base font-semibold text-navy">Edit your details</h3>
                <p className="mb-4 text-sm text-gray-500">Department, location and manager are set by HR.</p>

                <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
                    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                        <div>
                            <Input id="firstName" label="First name" {...register("firstName")} />
                            {errors.firstName && <p className="mt-1 text-sm text-red-600">{errors.firstName.message}</p>}
                        </div>
                        <div>
                            <Input id="lastName" label="Last name" {...register("lastName")} />
                            {errors.lastName && <p className="mt-1 text-sm text-red-600">{errors.lastName.message}</p>}
                        </div>
                        <div>
                            <Input id="email" label="Work email" type="email" {...register("email")} />
                            {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
                        </div>
                        <div>
                            <Input id="title" label="Job title" {...register("title")} />
                            {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title.message}</p>}
                        </div>
                        <div>
                            <Input id="startedOnUtc" label="Start date" type="date" {...register("startedOnUtc")} />
                            {errors.startedOnUtc && <p className="mt-1 text-sm text-red-600">{errors.startedOnUtc.message}</p>}
                        </div>
                    </div>

                    {updateMutation.isError && (
                        <p className="text-sm text-red-600">Couldn't save your changes. Please try again.</p>
                    )}
                    {updateMutation.isSuccess && <p className="text-sm text-green-700">Changes saved.</p>}

                    <Button
                        type="submit"
                        className="!w-auto self-start px-4"
                        disabled={!isDirty || updateMutation.isPending}
                    >
                        {updateMutation.isPending ? "Saving..." : "Save changes"}
                    </Button>
                </form>
            </section>
        </div>
    );
}
