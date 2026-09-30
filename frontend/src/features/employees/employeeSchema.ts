import { z } from "zod";

const nameRegex = /^[A-Za-z\s'-]+$/;

export const employeeFormSchema = z.object({
    firstName: z
        .string()
        .min(1, "First name is required")
        .regex(nameRegex, "First name can only contain letters"),
    lastName: z
        .string()
        .min(1, "Last name is required")
        .regex(nameRegex, "Last name can only contain letters"),
    email: z.string().min(1, "Email is required").email("Enter a valid email address"),
    title: z.string().min(1, "Title is required"),
    startedOnUtc: z.string().min(1, "Start date is required"),
});

export type EmployeeFormValues = z.infer<typeof employeeFormSchema>;