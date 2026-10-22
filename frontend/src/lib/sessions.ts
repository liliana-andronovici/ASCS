const TOKEN_KEY = "accessToken";
const AVATAR_KEY = "hr_portal_avatar";

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(expectedToken?: string): boolean {
    if (expectedToken !== undefined && getToken() !== expectedToken) return false;
    localStorage.removeItem(TOKEN_KEY);
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
