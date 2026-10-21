export function apiUrl(base: string | undefined, path: string, production: boolean): string {
    const configured = base?.trim() || (production ? "https://ascs-bk.onrender.com/api" : "/api");
    return `${configured.replace(/\/+$/, "")}/${path.replace(/^\/+/, "")}`;
}
