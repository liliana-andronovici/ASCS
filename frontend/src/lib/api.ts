import { apiUrl } from "./apiUrl";

export class ApiError extends Error {
    status: number;
    constructor(status: number, message: string) {
        super(message);
        this.status = status;
    }
}

type ApiRequestOptions = RequestInit & { auth?: "session" | "public" };

export async function apiFetch<T>(path: string, options?: ApiRequestOptions): Promise<T> {
    const { auth = "session", ...requestOptions } = options ?? {};
    const headers = new Headers(requestOptions.headers);
    if (!headers.has("Content-Type")) {
        headers.set("Content-Type", "application/json");
    }

    const accessToken = auth === "session" ? localStorage.getItem("accessToken") : null;
    if (auth === "public") headers.delete("Authorization");
    if (accessToken && !headers.has("Authorization")) {
        headers.set("Authorization", "Bearer " + accessToken);
    }

    const response = await fetch(apiUrl(import.meta.env.VITE_API_URL, path, import.meta.env.PROD), {
        ...requestOptions,
        headers,
    });

    if (!response.ok) {
        const text = await response.text().catch(() => "");
        throw new ApiError(response.status, text || response.statusText);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    const body = await response.text();
    if (!body.trim()) return undefined as T;
    if (!response.headers.get("Content-Type")?.toLowerCase().includes("json")) {
        throw new ApiError(502, "The API returned a non-JSON response.");
    }
    return JSON.parse(body) as T;
}
