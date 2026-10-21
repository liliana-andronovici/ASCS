import { useQuery } from "@tanstack/react-query";
import { getEmployeeById } from "../employees/api";

function currentEmployeeId(): number | null {
    const token = localStorage.getItem("accessToken");
    if (!token) return null;
    try {
        const payload = JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
        const id = Number(payload.sub);
        return Number.isInteger(id) && id > 0 ? id : null;
    } catch {
        return null;
    }
}

export function useCurrentEmployee() {
    const id = currentEmployeeId();
    const query = useQuery({
        queryKey: ["employees", "home-current", id],
        queryFn: () => getEmployeeById(id!),
        enabled: id !== null,
    });
    return { id, employee: query.data };
}
