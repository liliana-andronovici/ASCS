import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "../../lib/api";

// The backend does not expose a meetings endpoint yet. Keep this query pointed
// at the real API so the UI shows the server state instead of fabricated data.
const MEETINGS_ENDPOINT = "/meetings";

export interface Meeting {
    id: number;
    title: string;
    startUtc: string;
    endUtc: string;
    location: string | null;
}

export interface MeetingDay {
    date: Date;
    label: string;
    isToday: boolean;
    meetings: Meeting[];
}

const DAY_MS = 24 * 60 * 60 * 1000;

function startOfDay(d: Date): Date {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

function mondayOf(d: Date): Date {
    const day = startOfDay(d);
    return new Date(day.getTime() - ((day.getDay() + 6) % 7) * DAY_MS);
}

export function useMeetingsThisWeek() {
    const monday = mondayOf(new Date());
    const friday = new Date(monday.getTime() + 4 * DAY_MS);
    friday.setHours(23, 59, 59, 999);

    return useQuery({
        queryKey: ["meetings", monday.toISOString()],
        queryFn: () =>
            apiFetch<Meeting[]>(`${MEETINGS_ENDPOINT}?from=${monday.toISOString()}&to=${friday.toISOString()}`),
        retry: false,
        staleTime: 60 * 1000,
    });
}

export function buildMeetingsWeek(meetings: Meeting[], now: Date): MeetingDay[] {
    const monday = mondayOf(now);
    const today = startOfDay(now).getTime();

    return Array.from({ length: 5 }, (_, i) => {
        const date = new Date(monday.getTime() + i * DAY_MS);
        const t = date.getTime();
        return {
            date,
            label: date.toLocaleDateString("en-US", { weekday: "short" }),
            isToday: t === today,
            meetings: meetings
                .filter((m) => startOfDay(new Date(m.startUtc)).getTime() === t)
                .sort((a, b) => new Date(a.startUtc).getTime() - new Date(b.startUtc).getTime()),
        };
    });
}
