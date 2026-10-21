export function startOfDay(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

export function addDays(date: Date, count: number): Date {
    const next = new Date(date);
    next.setDate(next.getDate() + count);
    return next;
}

export function mondayOf(date: Date): Date {
    return addDays(startOfDay(date), -((date.getDay() + 6) % 7));
}

export function weekRange(now: Date, includeWeekend = false) {
    const monday = mondayOf(now);
    return { from: monday.toISOString(), to: addDays(monday, includeWeekend ? 7 : 5).toISOString() };
}
