import type { Employee } from "../employees/types";
import type { LeaveRequest, Meeting } from "./contracts";
import { addDays, mondayOf, startOfDay } from "./calendar";

export function absentEmployees(leave: LeaveRequest[], employees: Employee[], now: Date) {
    const date = startOfDay(now);
    const names = new Map(employees.map((employee) => [employee.id, `${employee.firstName} ${employee.lastName}`]));
    const out = new Map<number, { id: number; name: string; type: LeaveRequest["type"] }>();
    for (const request of leave) {
        if (request.status !== "Approved" || startOfDay(new Date(request.startDateUtc)) > date || startOfDay(new Date(request.endDateUtc)) < date) continue;
        out.set(request.employeeId, { id: request.employeeId, name: names.get(request.employeeId) ?? `Employee #${request.employeeId}`, type: request.type });
    }
    return [...out.values()];
}

export function buildWeek(leave: LeaveRequest[], employees: Employee[], now: Date) {
    const monday = mondayOf(now);
    return Array.from({ length: 5 }, (_, index) => {
        const date = addDays(monday, index);
        return { date, label: date.toLocaleDateString("en-US", { weekday: "short" }), isToday: date.getTime() === startOfDay(now).getTime(), out: absentEmployees(leave, employees, date) };
    });
}

export function buildUpcoming(employees: Employee[], now: Date, days = 45) {
    const today = startOfDay(now);
    const limit = addDays(today, days).getTime();
    const items: { id: string; date: Date; what: string; who: string }[] = [];
    for (const employee of employees) {
        if (employee.status !== "Active") continue;
        const started = startOfDay(new Date(employee.startedOnUtc));
        const name = `${employee.firstName} ${employee.lastName}`;
        if (started.getTime() >= today.getTime() && started.getTime() <= limit) {
            items.push({ id: `start-${employee.id}`, date: started, what: "Starts work", who: `${name} · ${employee.title}` });
            continue;
        }
        let next = new Date(today.getFullYear(), started.getMonth(), started.getDate());
        if (next < today) next = new Date(today.getFullYear() + 1, started.getMonth(), started.getDate());
        const years = next.getFullYear() - started.getFullYear();
        if (years >= 1 && next.getTime() <= limit) items.push({ id: `anniversary-${employee.id}`, date: next, what: `${years}-year work anniversary`, who: name });
    }
    return items.sort((a, b) => a.date.getTime() - b.date.getTime());
}

export function employeeStats(employees: Employee[], now: Date) {
    const today = startOfDay(now).getTime();
    const monthStart = new Date(now.getFullYear(), now.getMonth(), 1).getTime();
    const monthEnd = new Date(now.getFullYear(), now.getMonth() + 1, 1).getTime();
    const active = employees.filter((employee) => employee.status === "Active");
    const startedThisMonth = active.filter((employee) => {
        const start = startOfDay(new Date(employee.startedOnUtc)).getTime();
        return start >= monthStart && start < monthEnd;
    });
    return {
        headcount: active.filter((employee) => startOfDay(new Date(employee.startedOnUtc)).getTime() <= today).length,
        joinedThisMonth: startedThisMonth.filter((employee) => startOfDay(new Date(employee.startedOnUtc)).getTime() <= today).length,
        startingThisMonth: startedThisMonth.length,
    };
}

export function buildMeetingsWeek(meetings: Meeting[], now: Date) {
    const monday = mondayOf(now);
    return Array.from({ length: 5 }, (_, index) => {
        const date = addDays(monday, index);
        const next = addDays(date, 1);
        return {
            date, label: date.toLocaleDateString("en-US", { weekday: "short" }), isToday: date.getTime() === startOfDay(now).getTime(),
            meetings: meetings.filter((meeting) => Date.parse(meeting.startUtc) < next.getTime() && Date.parse(meeting.endUtc) > date.getTime())
                .sort((a, b) => Date.parse(a.startUtc) - Date.parse(b.startUtc)),
        };
    });
}
