import { apiFetch } from "../../lib/api";
import type { Department } from "./types";

export function getDepartments(): Promise<Department[]> {
    return apiFetch<Department[]>("/departments");
}