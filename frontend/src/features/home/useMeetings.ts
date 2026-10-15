import { useQuery } from "@tanstack/react-query";
import { getHomeMeetings, HOME_MEETINGS_PATH } from "./api";
import { weekRange } from "./calendar";
export { buildMeetingsWeek } from "./calculations";

export function useMeetingsThisWeek(now: Date) {
    const { from, to } = weekRange(now);
    const configured = Boolean(HOME_MEETINGS_PATH);
    const query = useQuery({
        queryKey: ["home-meetings", HOME_MEETINGS_PATH, from, to],
        queryFn: ({ signal }) => getHomeMeetings(from, to, signal),
        retry: false, staleTime: 60_000,
    });
    return { ...query, configured };
}
