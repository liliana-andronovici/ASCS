import { useQuery } from "@tanstack/react-query";
import { getLocations } from "./api";

export function useLocations() {
    return useQuery({
        queryKey: ["locations"],
        queryFn: getLocations,
        staleTime: 5 * 60 * 1000,
    });
}
