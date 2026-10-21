import { ApiError } from "./api";

function readBody(text: string): string | null {
    try {
        const body: unknown = JSON.parse(text);
        if (typeof body !== "object" || body === null) return null;
        if ("message" in body && typeof body.message === "string") return body.message;
        if ("errors" in body && Array.isArray(body.errors)) return body.errors.join(", ");
        return null;
    } catch {
        return null;
    }
}

export function getErrorMessage(error: unknown): string {
    if (error instanceof ApiError) return readBody(error.message) ?? error.message;
    if (error instanceof Error) return error.message;
    return "Something went wrong";
}