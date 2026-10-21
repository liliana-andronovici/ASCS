const BASE_URL = import.meta.env.VITE_API_URL ??
    (import.meta.env.PROD ? "https://ascs-bk.onrender.com/api" : "/api");

export class ApiError extends Error {
    status: number;
    constructor(status: number, message: string) {
        super(message);
        this.status = status;
    }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
    const headers = new Headers(options?.headers);
    if (!headers.has("Content-Type")) {
        headers.set("Content-Type", "application/json");
    }

    const accessToken = localStorage.getItem("accessToken");
    if (accessToken && !headers.has("Authorization")) {
        headers.set("Authorization", "Bearer " + accessToken);
    }

    const response = await fetch(`${BASE_URL}${path}`, {
        ...options,
        headers,
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
