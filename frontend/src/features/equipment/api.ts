import { apiFetch } from "../../lib/api";

export interface Equipment {
    id: number;
    name: string;
    code: string;
    equipmentTypeId: number;
    typeName: string;
    isAssigned: boolean;
}

export interface EquipmentType {
    id: number;
    name: string;
}

export interface EquipmentInput {
    name: string;
    code: string;
    equipmentTypeId: number;
}

export function getEquipment(): Promise<Equipment[]> {
    return apiFetch<Equipment[]>("/Equipment");
}

export function getEquipmentTypes(): Promise<EquipmentType[]> {
    return apiFetch<EquipmentType[]>("/EquipmentTypes");
}

export function queueEquipmentType(name: string): Promise<{ jobId: string }> {
    return apiFetch<{ jobId: string }>("/EquipmentTypeJobs", { method: "POST", body: JSON.stringify({ name }) });
}

export function createEquipment(input: EquipmentInput): Promise<number> {
    return apiFetch<number>("/Equipment", { method: "POST", body: JSON.stringify(input) });
}

export function updateEquipment(id: number, input: EquipmentInput): Promise<void> {
    return apiFetch<void>(`/Equipment/${id}`, { method: "PUT", body: JSON.stringify(input) });
}

export function deleteEquipment(id: number): Promise<void> {
    return apiFetch<void>(`/Equipment/${id}`, { method: "DELETE" });
}
