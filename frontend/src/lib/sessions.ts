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
 * Poza de profil nu are un endpoint real in backend, deci o tinem doar in
 * acest browser, in localStorage. subscribeLocalAvatar exista ca sa poata
 * si sidebar-ul (AccountMenu) sa se actualizeze imediat cand o schimbi din
 * Settings, fara sa fie nevoie de refresh la pagina.
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