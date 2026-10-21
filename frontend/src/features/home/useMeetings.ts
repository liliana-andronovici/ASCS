import { useQuery } from "@tanstack/react-query";
import { getHomeMeetings, HOME_MEETINGS_PATH } from "./api";
import { addDays, startOfDay, weekRange } from "./calendar";
export { buildMeetingsWeek } from "./calculations";

export function useMeetingsThisWeek(now: Date, employeeId: number | null) {
    const { from, to } = weekRange(now);
    const configured = Boolean(HOME_MEETINGS_PATH);
    const query = useQuery({
        queryKey: ["home-meetings", HOME_MEETINGS_PATH, employeeId, from, to],
        queryFn: ({ signal }) => getHomeMeetings(from, to, employeeId!, signal),
        enabled: employeeId !== null, retry: false, staleTime: 60_000,
    });
    return { ...query, configured, needsSignIn: employeeId === null };
}

export function useUpcomingMeetings(now: Date, employeeId: number | null) {
    const from = startOfDay(now).toISOString();
    const middle = addDays(startOfDay(now), 30).toISOString();
    const to = addDays(startOfDay(now), 46).toISOString();
    const query = useQuery({
        queryKey: ["home-upcoming-meetings", HOME_MEETINGS_PATH, employeeId, from, to],
        queryFn: async ({ signal }) => {
            const [first, second] = await Promise.all([
                getHomeMeetings(from, middle, employeeId!, signal),
                getHomeMeetings(middle, to, employeeId!, signal),
            ]);
            return [...new Map([...first, ...second].map((meeting) => [meeting.id, meeting])).values()];
        },
        enabled: employeeId !== null,
        retry: false,
        staleTime: 60_000,
    });
    return { ...query, needsSignIn: employeeId === null };
}
