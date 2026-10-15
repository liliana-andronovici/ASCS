const TOKEN_KEY = "infigohr_token";
const AVATAR_KEY = "infigohr_avatar";

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(): void {
    localStorage.removeItem(TOKEN_KEY);
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
        if (!employeeId || Number.isNaN(employeeId)) return null;
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

/**
 * Profile photos have no backend endpoint, so they are stored only in
 * this browser using localStorage. subscribeLocalAvatar lets the
 * sidebar (AccountMenu) update immediately when the photo changes in
 * Settings, without reloading the page.
 */
export function getLocalAvatar(): string | null {
    return localStorage.getItem(AVATAR_KEY);
}

export function setLocalAvatar(dataUrl: string): void {
    localStorage.setItem(AVATAR_KEY, dataUrl);
    avatarListeners.forEach((listen) => listen());
}

export function subscribeLocalAvatar(listener: Listener): () => void {
    avatarListeners.add(listener);
    return () => avatarListeners.delete(listener);
}