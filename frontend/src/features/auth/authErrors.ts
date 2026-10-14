import { ApiError } from "../../lib/api";

export function authErrorMessage(error: unknown, fallback: string): string {
    if (!(error instanceof ApiError)) return fallback;
    if (error.status === 429) return "Too many attempts. Please wait before trying again.";
    if (error.status === 400) {
        try {
            const body: { errors?: unknown; message?: unknown } = JSON.parse(error.message);
            if (Array.isArray(body.errors) && body.errors.every((item) => typeof item === "string")) {
                return body.errors.join(" ") || fallback;
            }
            if (typeof body.message === "string") return body.message;
        } catch { /* Use a friendly fallback for non-JSON responses. */ }
    }
    return fallback;
}
