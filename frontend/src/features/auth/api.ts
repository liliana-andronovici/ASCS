import { apiFetch } from "../../lib/api";

export interface SignInInput {
    email: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
}

export function signIn(input: SignInInput): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/sign-in", {
        auth: "public",
        method: "POST",
        body: JSON.stringify(input),
    });
}
