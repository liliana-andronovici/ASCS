import { apiFetch } from "../../lib/api";
import type { Location } from "./types";

export function getLocations() {
    return apiFetch<Location[]>("/Locations");
}

export function createLocation(input: { name: string }): Promise<void> {
    return apiFetch<void>("/Locations", {
        method: "POST",
        body: JSON.stringify(input),
    });
}

export function updateLocation(input: { id: number; name: string }): Promise<void> {
    return apiFetch<void>("/Locations", {
        method: "PUT",
        body: JSON.stringify(input),
    });
}

export function deleteLocation(id: number): Promise<void> {
    return apiFetch<void>(`/Locations/${id}`, {
        method: "DELETE",
    });
}