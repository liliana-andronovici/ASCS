import { apiFetch } from "../../lib/api";

export interface SignInInput {
    email: string;
    password: string;
}

export interface AuthResponse {
    accessToken?: string | null;
    requiresTwoFactor?: boolean;
    challengeToken?: string | null;
    challengeExpiresAtUtc?: string | null;
}

export function signIn(input: SignInInput): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/sign-in", { method: "POST", body: JSON.stringify(input) });
}

export function requestPasswordReset(email: string): Promise<void> {
    return apiFetch<void>("/auth/password-reset/request", { method: "POST", body: JSON.stringify({ email }) });
}

export function verifyPasswordReset(email: string, code: string): Promise<{ resetToken: string }> {
    return apiFetch<{ resetToken: string }>("/auth/password-reset/verify", {
        method: "POST", body: JSON.stringify({ email, code }),
    });
}

export function completePasswordReset(input: {
    email: string;
    resetToken: string;
    newPassword: string;
    confirmPassword: string;
}): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/password-reset/complete", { method: "POST", body: JSON.stringify(input) });
}

export function verifyTwoFactor(input: {
    challengeToken: string;
    code?: string;
    recoveryCode?: string;
}): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/two-factor/verify", { method: "POST", body: JSON.stringify(input) });
}
