import { apiFetch } from "../../lib/api";

export interface SignInInput {
    email: string;
    password: string;
}

export interface SignUpInput extends SignInInput {
    firstName: string;
    lastName: string;
    title: string;
}

export function signUp(input: SignUpInput): Promise<void> {
    // The existing API creates the account and returns 201 with no access token.
    return apiFetch<void>("/auth/sign-up", { auth: "public", method: "POST", body: JSON.stringify(input) });
}

export interface AuthResponse {
    accessToken?: string | null;
    requiresTwoFactor?: boolean;
    challengeToken?: string | null;
    challengeExpiresAtUtc?: string | null;
}

export function signIn(input: SignInInput): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/sign-in", { auth: "public", method: "POST", body: JSON.stringify(input) });
}

export function requestPasswordReset(email: string): Promise<void> {
    return apiFetch<void>("/auth/password-reset/request", { auth: "public", method: "POST", body: JSON.stringify({ email }) });
}

export function verifyPasswordReset(email: string, code: string): Promise<{ resetToken: string }> {
    return apiFetch<{ resetToken: string }>("/auth/password-reset/verify", {
        auth: "public", method: "POST", body: JSON.stringify({ email, code }),
    });
}

export function completePasswordReset(input: {
    email: string;
    resetToken: string;
    newPassword: string;
    confirmPassword: string;
}): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/password-reset/complete", { auth: "public", method: "POST", body: JSON.stringify(input) });
}

export function verifyTwoFactor(input: {
    challengeToken: string;
    code?: string;
    recoveryCode?: string;
}): Promise<AuthResponse> {
    return apiFetch<AuthResponse>("/auth/two-factor/verify", { auth: "public", method: "POST", body: JSON.stringify(input) });
}
