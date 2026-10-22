const TOKEN_KEY = "accessToken";
const LEGACY_TOKEN_KEY = "infigohr_token";
const AVATAR_KEY = "infigohr_avatar";

function isUnexpiredToken(token: string): boolean {
    try {
        const parts = token.split(".");
        if (parts.length !== 3 || parts.some(part => !part)) return false;
        const encoded = parts[1].replace(/-/g, "+").replace(/_/g, "/");
        const payload = JSON.parse(atob(encoded.padEnd(Math.ceil(encoded.length / 4) * 4, "=")));
        // UI expiry check only; the backend validates the signature and permissions.
        return typeof payload.exp === "number" && Number.isFinite(payload.exp) && payload.exp * 1000 > Date.now();
    } catch { return false; }
}

export function getToken(): string | null {
    const token = localStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(LEGACY_TOKEN_KEY);
    if (token && !isUnexpiredToken(token)) {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(LEGACY_TOKEN_KEY);
        return null;
    }
    return token;
}

export function setToken(token: string): void {
    if (!isUnexpiredToken(token)) throw new Error("Invalid or expired access token.");
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.removeItem(LEGACY_TOKEN_KEY);
}

export function clearToken(expectedToken?: string): boolean {
    if (expectedToken !== undefined && getToken() !== expectedToken) return false;
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(LEGACY_TOKEN_KEY);
    return true;
}

export interface DecodedToken {
    employeeId: number;
    email: string;
}

export function decodeToken(token: string): DecodedToken | null {
    try {
        const payload = token.split(".")[1];
        const json = JSON.parse(atob(payload.replace(/-/g, "+").replace(/_/g, "/")));
        const employeeId = Number(json.sub);
        if (!Number.isInteger(employeeId) || employeeId <= 0) return null;
        return { employeeId, email: json.email ?? "" };
    } catch {
        return null;
    }
}

export function getCurrentEmployeeId(): number | null {
    const token = getToken();
    if (!token) return null;
    return decodeToken(token)?.employeeId ?? null;
}

type Listener = () => void;
const avatarListeners = new Set<Listener>();

export function getLocalAvatar(): string | null {
    const employeeId = getCurrentEmployeeId();
    return employeeId === null ? null : localStorage.getItem(`${AVATAR_KEY}_${employeeId}`);
}

export function setLocalAvatar(dataUrl: string): void {
    const employeeId = getCurrentEmployeeId();
    if (employeeId === null) return;
    localStorage.setItem(`${AVATAR_KEY}_${employeeId}`, dataUrl);
    avatarListeners.forEach((listen) => listen());
}

export function subscribeLocalAvatar(listener: Listener): () => void {
    avatarListeners.add(listener);
    return () => avatarListeners.delete(listener);
}
