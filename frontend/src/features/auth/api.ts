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

export interface TwoFactorStatus {
    isEnabled: boolean;
    recoveryCodesRemaining: number;
}

export interface TwoFactorSetup {
    setupId: string;
    sharedKey: string;
    authenticatorUri: string;
    expiresAtUtc: string;
}

interface TwoFactorRecovery {
    accessToken: string;
    recoveryCodes: string[];
}

interface TwoFactorManagement {
    password: string;
    code?: string;
    recoveryCode?: string;
}

export function getTwoFactorStatus() {
    return apiFetch<TwoFactorStatus>("/auth/two-factor/status");
}

export function setupTwoFactor(password: string) {
    return apiFetch<TwoFactorSetup>("/auth/two-factor/setup", {
        method: "POST", body: JSON.stringify({ password }),
    });
}

export function enableTwoFactor(setupId: string, code: string) {
    return apiFetch<TwoFactorRecovery>("/auth/two-factor/enable", {
        method: "POST", body: JSON.stringify({ setupId, code }),
    });
}

export function regenerateTwoFactorCodes(input: TwoFactorManagement) {
    return apiFetch<TwoFactorRecovery>("/auth/two-factor/recovery-codes/regenerate", {
        method: "POST", body: JSON.stringify(input),
    });
}

export function disableTwoFactor(input: TwoFactorManagement) {
    return apiFetch<AuthResponse>("/auth/two-factor/disable", {
        method: "POST", body: JSON.stringify(input),
    });
}
