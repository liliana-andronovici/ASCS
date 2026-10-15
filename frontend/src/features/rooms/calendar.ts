import type { RoomDto, RoomBookingDto } from "./contract";
import type { Room, RoomBooking } from "./types/room";

export const DAY_LABELS = ["Mon", "Tue", "Wed", "Thu", "Fri"];
export const START_HOUR = 8;
export const END_HOUR = 19;
export const HOURS = Array.from({ length: END_HOUR - START_HOUR }, (_, index) => `${START_HOUR + index}:00`);
export const ROW_HEIGHT = 60;

export function getWeekDates(weekOffset: number, now = new Date()): Date[] {
    const monday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    monday.setDate(monday.getDate() - (monday.getDay() + 6) % 7 + weekOffset * 7);
    return Array.from({ length: 5 }, (_, index) => {
        const date = new Date(monday);
        date.setDate(date.getDate() + index);
        return date;
    });
}

export function getBookingRange(dates: Date[]) {
    const end = new Date(dates[4]);
    end.setDate(end.getDate() + 1);
    return { fromUtc: dates[0].toISOString(), toUtc: end.toISOString() };
}

export function toBookingStart(date: Date, minutes: number): Date {
    const start = new Date(date);
    start.setHours(Math.floor(minutes / 60), minutes % 60, 0, 0);
    return start;
}

export function toRoom(room: RoomDto): Room {
    return {
        id: room.id, name: room.name, seats: room.seats,
        short: room.name.trim().split(/\s+/).map((part) => part[0]).join("").slice(0, 2).toUpperCase(),
        meta: [`Seats ${room.seats}`, room.amenities, `floor ${room.floor}`].filter(Boolean).join(" · "),
    };
}

export function toCalendarBookings(records: RoomBookingDto[], dates: Date[], roomIds: Set<number>): RoomBooking[] {
    return records.filter((record) => roomIds.has(record.roomId)).flatMap((record) => dates.flatMap((date, day) => {
        const start = new Date(record.startDateTimeUtc).getTime();
        const end = start + record.durationMinutes * 60_000;
        const visibleStart = Math.max(start, toBookingStart(date, START_HOUR * 60).getTime());
        const visibleEnd = Math.min(end, toBookingStart(date, END_HOUR * 60).getTime());
        if (visibleEnd <= visibleStart) return [];
        const localStart = new Date(visibleStart);
        return [{
            id: `${record.id}-${day}`, roomId: record.roomId, day,
            startMin: localStart.getHours() * 60 + localStart.getMinutes(),
            durationMin: (visibleEnd - visibleStart) / 60_000,
            title: record.title, who: record.organizerName || `Employee #${record.organizerEmployeeId}`,
        }];
    }));
}
