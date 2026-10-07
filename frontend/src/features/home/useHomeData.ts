import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "../../lib/api";
import { useEmployeeDirectory } from "../employees/useEmployees";
import type { Employee } from "../employees/types";

// ---- Time off ----------------------------------------------------------
// Backendul din main are doar tabela LeaveRequest, fara endpoint. Cand PR-ul
// de time-off intra in main, schimba doar ruta de mai jos (si forma raspunsului
// in LeaveRequest, daca difera).
const LEAVE_ENDPOINT = "/leave-requests";

export type LeaveType = "Vacation" | "Sick" | "Remote";

export interface LeaveRequest {
    id: number;
    employeeId: number;
    startDateUtc: string;
    endDateUtc: string;
    type: LeaveType;
    status: "Pending" | "Approved" | "Declined";
}

export interface DayColumn {
    date: Date;
    label: string;
    isToday: boolean;
    out: { id: number; name: string; type: LeaveType }[];
}

export interface UpcomingItem {
    date: Date;
    what: string;
    who: string;
}

const DAY_MS = 24 * 60 * 60 * 1000;

function startOfDay(d: Date): Date {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

function mondayOf(d: Date): Date {
    const day = startOfDay(d);
    const offset = (day.getDay() + 6) % 7; // luni = 0
    return new Date(day.getTime() - offset * DAY_MS);
}

export function useLeaveThisWeek() {
    const monday = mondayOf(new Date());
    const friday = new Date(monday.getTime() + 4 * DAY_MS);

    return useQuery({
        queryKey: ["leave", monday.toISOString()],
        queryFn: () =>
            apiFetch<LeaveRequest[]>(
                `${LEAVE_ENDPOINT}?from=${monday.toISOString()}&to=${friday.toISOString()}`
            ),
        retry: false,
        staleTime: 60 * 1000,
    });
}

// ---- Date derivate din angajati -----------------------------------------

function fullName(e: Employee): string {
    return `${e.firstName} ${e.lastName}`;
}

export function buildWeek(leave: LeaveRequest[], employees: Employee[], now: Date): DayColumn[] {
    const monday = mondayOf(now);
    const today = startOfDay(now).getTime();
    const names = new Map(employees.map((e) => [e.id, fullName(e)]));

    return Array.from({ length: 5 }, (_, i) => {
        const date = new Date(monday.getTime() + i * DAY_MS);
        const t = date.getTime();
        const out = leave
            .filter((l) => l.status === "Approved")
            .filter((l) => startOfDay(new Date(l.startDateUtc)).getTime() <= t)
            .filter((l) => startOfDay(new Date(l.endDateUtc)).getTime() >= t)
            .map((l) => ({
                id: l.id,
                name: names.get(l.employeeId) ?? `Employee #${l.employeeId}`,
                type: l.type,
            }));
        return {
            date,
            label: date.toLocaleDateString("en-US", { weekday: "short" }),
            isToday: t === today,
            out,
        };
    });
}

export function buildUpcoming(employees: Employee[], now: Date, days = 45): UpcomingItem[] {
    const today = startOfDay(now);
    const limit = today.getTime() + days * DAY_MS;
    const items: UpcomingItem[] = [];

    for (const e of employees) {
        if (e.status !== "Active") continue;
        const started = new Date(e.startedOnUtc);

        // incepe in viitor
        if (startOfDay(started).getTime() >= today.getTime() && started.getTime() <= limit) {
            items.push({ date: started, what: "Starts at Infigo", who: `${fullName(e)} · ${e.title}` });
            continue;
        }

        // aniversare de munca (urmatoarea aparitie a zilei de inceput)
        let next = new Date(today.getFullYear(), started.getMonth(), started.getDate());
        if (next.getTime() < today.getTime()) {
            next = new Date(today.getFullYear() + 1, started.getMonth(), started.getDate());
        }
        const years = next.getFullYear() - started.getFullYear();
        if (years >= 1 && next.getTime() <= limit) {
            items.push({
                date: next,
                what: `${years}-year work anniversary`,
                who: fullName(e),
            });
        }
    }

    return items.sort((a, b) => a.date.getTime() - b.date.getTime()).slice(0, 6);
}

export function useHomeData() {
    const directory = useEmployeeDirectory();
    const leave = useLeaveThisWeek();

    const now = new Date();
    const employees = directory.data?.items ?? [];
    const active = employees.filter((e) => e.status === "Active");

    const monthStart = new Date(now.getFullYear(), now.getMonth(), 1).getTime();
    const monthEnd = new Date(now.getFullYear(), now.getMonth() + 1, 1).getTime();
    const startedInMonth = active.filter((e) => {
        const t = new Date(e.startedOnUtc).getTime();
        return t >= monthStart && t < monthEnd;
    });
    const joinedSoFar = startedInMonth.filter((e) => new Date(e.startedOnUtc).getTime() <= now.getTime());

    const leaveAvailable = leave.isSuccess;
    const week = buildWeek(leave.data ?? [], employees, now);
    const outToday = week.find((d) => d.isToday)?.out.length ?? 0;

    return {
        isLoading: directory.isLoading,
        isError: directory.isError,
        headcount: active.length,
        joinedThisMonth: joinedSoFar.length,
        startingThisMonth: startedInMonth.length,
        leaveAvailable,
        outToday,
        week,
        upcoming: buildUpcoming(employees, now),
        now,
    };
}
