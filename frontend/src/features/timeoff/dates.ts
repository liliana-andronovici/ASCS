const dayMonth = new Intl.DateTimeFormat("en-GB", { day: "numeric", month: "short", timeZone: "UTC" });
const full = new Intl.DateTimeFormat("en-GB", { day: "numeric", month: "short", year: "numeric", timeZone: "UTC" });

export function formatShortDate(value: string): string {
    return dayMonth.format(new Date(value));
}

export function formatDateRange(startDate: string, endDate: string): string {
    const start = new Date(startDate);
    const end = new Date(endDate);

    if (start.getTime() === end.getTime()) return full.format(start);

    const sameMonth =
        start.getUTCMonth() === end.getUTCMonth() && start.getUTCFullYear() === end.getUTCFullYear();

    if (sameMonth) return `${start.getUTCDate()}–${full.format(end)}`;
    return `${dayMonth.format(start)} – ${full.format(end)}`;
}