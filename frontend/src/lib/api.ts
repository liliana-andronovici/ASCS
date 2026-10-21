import { clearToken, getToken } from "./sessions";

const BASE_URL = (import.meta.env.VITE_API_URL?.trim() ||
    (import.meta.env.PROD ? "https://ascs-bk.onrender.com/api" : "/api")).replace(/\/+$/, "");

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

    const token = auth === "session" ? getToken() : null;
    if (auth === "public") headers.delete("Authorization");
    if (token && !headers.has("Authorization")) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    const response = await fetch(`${BASE_URL}/${path.replace(/^\/+/, "")}`, {
        ...requestOptions,
        headers,
    });

    if (!response.ok) {
        // Clear an expired or invalid session and return to sign in.
        if (response.status === 401 && token &&
            headers.get("Authorization") === `Bearer ${token}` && clearToken(token)) {
            window.location.assign("/sign-in");
        }

        const text = await response.text().catch(() => "");
        throw new ApiError(response.status, text || response.statusText);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    const body = await response.text();
    return body ? JSON.parse(body) as T : undefined as T;
}
