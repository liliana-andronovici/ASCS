const BASE_URL = import.meta.env.VITE_API_URL ?? "/api";

export async function apiFetch(){ // async e folosit pentru a nu blcoa ecranul si da voie utilizatorului sa foloseasca mouse-ul
    const response = await fetch("/");
}