import { z } from "zod";

export const signUpSchema = z.object({
    firstName: z.string().trim().min(1, "First name is required.").max(100),
    lastName: z.string().trim().min(1, "Last name is required.").max(100),
    email: z.email("Enter a valid email address.").trim().max(150),
    password: z.string().min(12, "Use at least 12 characters.").max(128),
    confirmPassword: z.string(),
    title: z.string().trim().min(1, "Job title is required.").max(150),
}).refine(values => values.password === values.confirmPassword, {
    message: "Passwords do not match.", path: ["confirmPassword"],
});
