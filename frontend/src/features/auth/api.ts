import { apiFetch } from "../../lib/api";

export interface SignInInput {
    email: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
}

export function signIn(input: SignInInput): Promise<AuthResponse> {
    // Local demo login mirrors the mock API so sign-in works if its server is unavailable.
    if (import.meta.env.DEV) {
        const demoUsers: Record<string, number> = {
            "test@infigo.ro": 1,
            "ana.popescu@infigo.ro": 2,
        };
        const email = input.email.trim().toLowerCase();
        const employeeId = demoUsers[email];

        if (employeeId) {
            const payload = btoa(JSON.stringify({ sub: String(employeeId), email }))
                .replace(/=/g, "")
                .replace(/\+/g, "-")
                .replace(/\//g, "_");

            return Promise.resolve({
                accessToken: `eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.${payload}.mock`,
            });
        }
    }

    return apiFetch<AuthResponse>("/auth/sign-in", {
        method: "POST",
        body: JSON.stringify(input),
    });
}
