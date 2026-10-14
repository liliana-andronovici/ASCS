import { z } from "zod";

export const departmentFormSchema = z.object({
    name: z.string().trim().min(1, "Name is required").max(150, "Name must be 150 characters or fewer"),
});

export type DepartmentFormValues = z.infer<typeof departmentFormSchema>;
