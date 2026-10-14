const TOKEN_KEY = "accessToken";

function isUnexpiredToken(token: string): boolean {
    try {
        const parts = token.split(".");
        if (parts.length !== 3 || parts.some((part) => !part)) return false;
        const encoded = parts[1].replace(/-/g, "+").replace(/_/g, "/");
        const payload = JSON.parse(atob(encoded.padEnd(Math.ceil(encoded.length / 4) * 4, "=")));
        // This is a UI expiry check. The backend still validates the signature and permissions.
        return typeof payload.exp === "number" && Number.isFinite(payload.exp) && payload.exp * 1000 > Date.now();
    } catch {
        return false;
    }
}

export function getToken(): string | null {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token && !isUnexpiredToken(token)) {
        clearToken();
        return null;
    }
    return token;
}

export function setToken(token: string): void {
    if (!isUnexpiredToken(token)) {
        clearToken();
        throw new Error("Invalid or expired access token.");
    }
    localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(expectedToken?: string): boolean {
    if (expectedToken !== undefined && localStorage.getItem(TOKEN_KEY) !== expectedToken) return false;
    localStorage.removeItem(TOKEN_KEY);
    return true;
}
