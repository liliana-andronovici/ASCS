const BASE_URL = import.meta.env.VITE_API_URL ?? "/api";

export class ApiError extends Error {
    status: number;
    constructor(status: number, message: string) {
        super(message);
        this.status = status;
    }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
    const response = await fetch(`${BASE_URL}${path}`, {
        headers: { "Content-Type": "application/json", ...options?.headers },
        ...options,
    });

    if (!response.ok) {
        const text = await response.text().catch(() => "");
        throw new ApiError(response.status, text || response.statusText);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json() as Promise<T>;
}