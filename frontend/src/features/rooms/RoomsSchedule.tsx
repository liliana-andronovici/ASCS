import { Fragment, useEffect, useMemo, useState, type ReactNode } from "react";
import { ChevronLeft, ChevronRight, CalendarPlus } from "lucide-react";
import { Button } from "../../components/Button";
import {
    DAY_LABELS,
    START_HOUR,
    HOURS,
    ROW_HEIGHT,
} from "./calendar";
import { END_HOUR, getWeekDates, getBookingRange, toBookingStart, toCalendarBookings } from "./calendar";
import { roomErrorMessage } from "./errors";
import { useCreateRoomBooking, useRoomBookings } from "./useRooms";
import { DURATIONS } from "./types/room.ts";
import type { Room, RoomsViewMode } from "./types/room.ts";

function formatWeekRange(dates: Date[]): string {
    const fmt = (d: Date) => d.toLocaleDateString("en-US", { day: "numeric", month: "short" });
    return `${fmt(dates[0])} – ${fmt(dates[4])}, ${dates[4].getFullYear()}`;
}

function formatDayLabel(date: Date, weekday: string): string {
    return `${weekday}, ${date.toLocaleDateString("en-US", { day: "numeric", month: "short" })}`;
}

function formatTime(minutes: number): string {
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return `${h}:${String(m).padStart(2, "0")}`;
}

function overlaps(aStart: number, aLen: number, bStart: number, bLen: number) {
    return aStart < bStart + bLen && bStart < aStart + aLen;
}

export function RoomsSchedule({ rooms, officeSelector }: { rooms: Room[]; officeSelector: ReactNode }) {
    const [now, setNow] = useState(() => Date.now());
    useEffect(() => {
        const timer = window.setInterval(() => setNow(Date.now()), 30_000);
        return () => window.clearInterval(timer);
    }, []);
    const today = new Date(now);
    const todayDayIndex = Math.min((today.getDay() + 6) % 7, 4);
    today.setHours(0, 0, 0, 0);
    const todayStart = today.getTime();
    const [view, setView] = useState<RoomsViewMode>("week");
    const [weekOffset, setWeekOffset] = useState(0);
    const [preferredRoomId, setSelectedRoomId] = useState(rooms[0].id);
    const [selectedDay, setSelectedDay] = useState(todayDayIndex);
    const [draftOpen, setDraftOpen] = useState(false);
    const [draftStart, setDraftStart] = useState(14 * 60);
    const [draftDuration, setDraftDuration] = useState<(typeof DURATIONS)[number]>(60);
    const [title, setTitle] = useState("");
    const [justBooked, setJustBooked] = useState(false);

    const weekDates = useMemo(() => getWeekDates(weekOffset, new Date(todayStart)), [weekOffset, todayStart]);
    const selectedRoom = rooms.find((r) => r.id === preferredRoomId) ?? rooms[0];
    const selectedRoomId = selectedRoom.id;
    const { fromUtc, toUtc } = getBookingRange(weekDates);
    const bookingQuery = useRoomBookings(fromUtc, toUtc);
    const createBooking = useCreateRoomBooking();
    const bookings = useMemo(() => toCalendarBookings(bookingQuery.data ?? [], weekDates, new Set(rooms.map((room) => room.id))), [bookingQuery.data, weekDates, rooms]);
    const scheduleReady = bookingQuery.isSuccess && !bookingQuery.isFetching;
    const draftDate = toBookingStart(weekDates[selectedDay], draftStart);
    const invalidTime = draftStart < START_HOUR * 60 || draftStart + draftDuration > END_HOUR * 60 || draftDate.getTime() <= now;
    const draftEnd = draftStart + draftDuration;

    const clash = bookings.some(
        (b) =>
            b.roomId === selectedRoomId &&
            b.day === selectedDay &&
            overlaps(draftStart, draftDuration, b.startMin, b.durationMin)
    );

    function goToday() {
        if (createBooking.isPending) return;
        setDraftOpen(false);
        setJustBooked(false);
        createBooking.reset();
        setWeekOffset(0);
        setSelectedDay(todayDayIndex);
    }

    function goPrev() {
        if (createBooking.isPending) return;
        setDraftOpen(false);
        setJustBooked(false);
        createBooking.reset();
        if (view === "week") {
            setWeekOffset((w) => w - 1);
            return;
        }
        if (selectedDay > 0) {
            setSelectedDay((d) => d - 1);
        } else {
            setWeekOffset((w) => w - 1);
            setSelectedDay(4);
        }
    }

    function goNext() {
        if (createBooking.isPending) return;
        setDraftOpen(false);
        setJustBooked(false);
        createBooking.reset();
        if (view === "week") {
            setWeekOffset((w) => w + 1);
            return;
        }
        if (selectedDay < 4) {
            setSelectedDay((d) => d + 1);
        } else {
            setWeekOffset((w) => w + 1);
            setSelectedDay(0);
        }
    }

    function openDraft(day: number, hourLabel: string, roomId?: number) {
        if (!scheduleReady || createBooking.isPending) return;
        createBooking.reset();
        setJustBooked(false);
        setSelectedDay(day);
        setDraftStart(parseInt(hourLabel, 10) * 60);
        if (roomId !== undefined) setSelectedRoomId(roomId);
        setDraftOpen(true);
    }

    async function handleBook() {
        if (draftDate.getTime() <= Date.now()) { setNow(Date.now()); return; }
        if (clash || invalidTime || !title.trim() || !scheduleReady || createBooking.isPending) return;
        setJustBooked(false);
        try {
            await createBooking.mutateAsync({
                roomId: selectedRoomId, title: title.trim(),
                startDateTimeUtc: draftDate.toISOString(), durationMinutes: draftDuration,
            });
            setJustBooked(true);
            setDraftOpen(false);
            setTitle("");
        } catch { /* The mutation error is displayed next to the booking form. */ }
    }

    const roomFreePercent = useMemo(() => {
        const totalMinutesInWeek = 5 * HOURS.length * 60;
        const map = new Map<number, number>();
        rooms.forEach((room) => {
            const busy = bookings
                .filter((b) => b.roomId === room.id)
                .reduce((sum, b) => sum + b.durationMin, 0);
            map.set(room.id, Math.max(0, Math.round(100 - (busy / totalMinutesInWeek) * 100)));
        });
        return map;
    }, [bookings, rooms]);

    const columnCount = view === "week" ? 5 : rooms.length;

    return (
        <div className="p-2 sm:p-4 lg:p-6">
            <div className="flex min-w-0 flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-card xl:flex-row">
                <div className="flex min-w-0 flex-1 flex-col">
                    <div className="flex flex-wrap items-center gap-2 border-b border-gray-200 px-3 py-3 sm:gap-3 sm:px-6 sm:py-4">
                        <h1 className="text-xl font-bold text-navy">Meeting rooms</h1>

                        <div className="flex items-center gap-1">
                            <button
                                aria-label="Previous period" onClick={goPrev}
                                className="rounded-md border border-gray-300 p-1.5 text-gray-500 hover:bg-gray-50"
                            >
                                <ChevronLeft size={14} />
                            </button>
                            <span className="px-2 text-sm font-semibold text-navy">
                                {view === "week"
                                    ? formatWeekRange(weekDates)
                                    : formatDayLabel(weekDates[selectedDay], DAY_LABELS[selectedDay])}
                            </span>
                            <button
                                aria-label="Next period" onClick={goNext}
                                className="rounded-md border border-gray-300 p-1.5 text-gray-500 hover:bg-gray-50"
                            >
                                <ChevronRight size={14} />
                            </button>
                            <button
                                onClick={goToday}
                                className="ml-1 text-sm font-medium text-coral hover:underline"
                            >
                                Today
                            </button>
                        </div>

                        <span className="flex-1" />

                        <div className="flex overflow-hidden rounded-full border border-gray-300 text-sm font-medium">
                            <button
                                onClick={() => setView("day")}
                                className={`px-4 py-2 ${view === "day" ? "bg-coral text-white" : "text-gray-500"}`}
                            >
                                Day
                            </button>
                            <button
                                onClick={() => setView("week")}
                                className={`px-4 py-2 ${view === "week" ? "bg-coral text-white" : "text-gray-500"}`}
                            >
                                Week
                            </button>
                        </div>

                        {officeSelector}

                        <Button
                            onClick={() => {
                                setJustBooked(false);
                                createBooking.reset();
                                setDraftOpen(true);
                            }}
                            disabled={!scheduleReady || createBooking.isPending}
                            className="!w-auto px-4"
                        >
                            <span className="flex items-center gap-1.5">
                                <CalendarPlus size={16} /> New booking
                            </span>
                        </Button>
                    </div>

                    {view === "week" && (
                        <div className="flex flex-wrap items-center gap-2 border-b border-gray-200 px-3 py-2 sm:px-4">
                            <span className="mr-1 text-xs uppercase tracking-wide text-gray-400">Room</span>
                            {rooms.map((room) => (
                                <button
                                    key={room.id}
                                    onClick={() => setSelectedRoomId(room.id)}
                                    className={`rounded-full border px-2 py-1 text-xs font-semibold sm:px-3 sm:text-sm ${
                                        room.id === selectedRoomId
                                            ? "border-navy bg-navy text-white"
                                            : "border-gray-300 bg-white text-navy"
                                    }`}
                                >
                                    {room.name}
                                    <span className="ml-1.5 hidden font-normal opacity-70 sm:inline">
                                        {scheduleReady ? `${roomFreePercent.get(room.id)}% free` : "Availability unavailable"}
                                    </span>
                                </button>
                            ))}
                        </div>
                    )}

                    <div className="px-4 py-2 text-sm" aria-live="polite">
                        {bookingQuery.isFetching && <p role="status">Loading schedule...</p>}
                        {bookingQuery.isError && <div role="alert" className="text-red-600">{roomErrorMessage(bookingQuery.error)} <button onClick={() => bookingQuery.refetch()} disabled={bookingQuery.isFetching} className="underline">Retry</button></div>}
                        {scheduleReady && bookings.length === 0 && <p className="text-gray-500">No bookings for these rooms this week.</p>}
                    </div>
                    <div className="overflow-x-auto border-b border-gray-200">
                    <div
                        className="grid min-w-[520px] shrink-0 bg-gray-50"
                        style={{ gridTemplateColumns: `56px repeat(${columnCount}, 1fr)` }}
                    >
                        <div />
                        {view === "week"
                            ? DAY_LABELS.map((label, i) => (
                                <div key={label} className="flex items-center gap-2 border-l border-gray-200 px-3 py-2">
                                    <span className="text-xs uppercase tracking-wide text-gray-400">{label}</span>
                                    <span
                                        className={`flex h-7 w-7 items-center justify-center rounded-full text-sm font-bold ${
                                            weekDates[i].getTime() === todayStart
                                                ? "bg-coral text-white"
                                                : "text-navy"
                                        }`}
                                    >
                                          {weekDates[i].getDate()}
                                      </span>
                                </div>
                            ))
                            : rooms.map((room) => (
                                <div key={room.id} className="flex flex-col gap-0.5 border-l border-gray-200 px-3 py-2">
                                    <span className="text-sm font-bold text-navy">{room.name}</span>
                                    <span className="text-xs text-gray-400">{room.meta}</span>
                                </div>
                            ))}
                    </div>
                    </div>

                    <div className="relative flex-1 overflow-auto">
                        <div
                            className="grid min-w-[520px]"
                            style={{
                                gridTemplateColumns: `56px repeat(${columnCount}, 1fr)`,
                                gridAutoRows: `${ROW_HEIGHT}px`,
                            }}
                        >
                            {HOURS.map((hourLabel) => (
                                <Fragment key={hourLabel}>
                                    <div className="border-t border-gray-100 pr-2 pt-0.5 text-right text-xs text-gray-400">
                                        {hourLabel}
                                    </div>
                                    {view === "week"
                                        ? DAY_LABELS.map((_, day) => (
                                            <div
                                                key={day}
                                                onClick={() => openDraft(day, hourLabel)}
                                                className={`cursor-pointer border-l border-t border-gray-100 hover:bg-gray-50 ${
                                                    day === selectedDay ? "bg-[#fbfcfd]" : ""
                                                }`}
                                            />
                                        ))
                                        : rooms.map((room) => (
                                            <div
                                                key={room.id}
                                                onClick={() => openDraft(selectedDay, hourLabel, room.id)}
                                                className="cursor-pointer border-l border-t border-gray-100 hover:bg-gray-50"
                                            />
                                        ))}
                                </Fragment>
                            ))}
                        </div>

                        {(view === "week"
                                ? bookings.filter((b) => b.roomId === selectedRoomId)
                                : bookings.filter((b) => b.day === selectedDay)
                        ).map((b) => {
                            const columnIndex = view === "week" ? b.day : rooms.findIndex((r) => r.id === b.roomId);
                            return (
                                <div
                                    key={b.id}
                                    className="absolute flex flex-col gap-0.5 overflow-hidden rounded-md bg-[#e6ecf2] px-2 py-1 text-xs text-navy shadow-sm"
                                    style={{
                                        left: `calc(56px + (100% - 56px) * ${columnIndex} / ${columnCount} + 3px)`,
                                        width: `calc((100% - 56px) / ${columnCount} - 6px)`,
                                        top: `${((b.startMin - START_HOUR * 60) / 60) * ROW_HEIGHT + 2}px`,
                                        height: `${(b.durationMin / 60) * ROW_HEIGHT - 4}px`,
                                    }}
                                >
                                    <span className="truncate font-bold">{b.title}</span>
                                    {b.durationMin >= 45 && (
                                        <span className="truncate opacity-80">
                                            {formatTime(b.startMin)}–{formatTime(b.startMin + b.durationMin)} · {b.who}
                                        </span>
                                    )}
                                </div>
                            );
                        })}

                        {draftOpen && !invalidTime &&
                            (() => {
                                const columnIndex =
                                    view === "week" ? selectedDay : rooms.findIndex((r) => r.id === selectedRoomId);
                                return (
                                    <div
                                        className={`absolute flex flex-col gap-0.5 overflow-hidden rounded-md px-2 py-1 text-xs shadow-md ${
                                            clash
                                                ? "border border-coral bg-[#fdecea] text-coral"
                                                : "border border-navy bg-navy text-white"
                                        }`}
                                        style={{
                                            left: `calc(56px + (100% - 56px) * ${columnIndex} / ${columnCount} + 3px)`,
                                            width: `calc((100% - 56px) / ${columnCount} - 6px)`,
                                            top: `${((draftStart - START_HOUR * 60) / 60) * ROW_HEIGHT + 2}px`,
                                            height: `${(draftDuration / 60) * ROW_HEIGHT - 4}px`,
                                        }}
                                    >
                                        <span className="truncate font-bold">{title || "New booking"}</span>
                                        {draftDuration >= 45 && (
                                            <span className="truncate opacity-80">
                                                {formatTime(draftStart)}–{formatTime(draftEnd)} · You
                                            </span>
                                        )}
                                    </div>
                                );
                            })()}
                    </div>
                </div>

                <fieldset disabled={createBooking.isPending} className="flex w-full min-w-0 flex-col gap-3.5 border-t border-gray-200 bg-[#f8fafc] p-3 sm:p-5 xl:w-[340px] xl:shrink-0 xl:border-l xl:border-t-0">
                    {draftOpen ? (
                        <>
                            <div className="flex items-baseline justify-between">
                                <h4 className="text-lg font-bold text-navy">New booking</h4>
                                <button onClick={() => { setDraftOpen(false); createBooking.reset(); }} className="text-xs text-gray-400 hover:underline">
                                    Cancel
                                </button>
                            </div>

                            <div className="flex items-center gap-3 rounded-md border border-gray-200 bg-white p-3">
                                <span className="flex h-10 w-10 items-center justify-center rounded-md bg-navy text-xs font-bold text-white">
                                    {selectedRoom.short}
                                </span>
                                <span className="text-sm leading-tight">
                                    <strong className="block">{selectedRoom.name}</strong>
                                    <span className="text-gray-500">{selectedRoom.meta}</span>
                                </span>
                            </div>

                            {view === "day" && (
                                <div className="flex flex-wrap gap-1.5">
                                    {rooms.map((room) => (
                                        <button
                                            key={room.id}
                                            onClick={() => setSelectedRoomId(room.id)}
                                            className={`rounded-full border px-3 py-1 text-xs font-semibold ${
                                                room.id === selectedRoomId
                                                    ? "border-navy bg-navy text-white"
                                                    : "border-gray-300 text-navy"
                                            }`}
                                        >
                                            {room.name}
                                        </button>
                                    ))}
                                </div>
                            )}

                            <div className="flex flex-col gap-1">
                                <label htmlFor="booking-title" className="text-sm text-gray-700">Title</label>
                                <input
                                    id="booking-title"
                                    maxLength={200}
                                    value={title}
                                    onChange={(e) => setTitle(e.target.value)}
                                    className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-3">
                                <div className="flex flex-col gap-1">
                                    <label htmlFor="booking-day" className="text-sm text-gray-700">Day</label>
                                    <select
                                        id="booking-day"
                                        value={selectedDay}
                                        onChange={(e) => setSelectedDay(Number(e.target.value))}
                                        className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                                    >
                                        {DAY_LABELS.map((label, i) => (
                                            <option key={label} value={i}>
                                                {formatDayLabel(weekDates[i], label)}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="flex flex-col gap-1">
                                    <label htmlFor="booking-start" className="text-sm text-gray-700">Starts</label>
                                    <select
                                        id="booking-start"
                                        value={draftStart}
                                        onChange={(e) => setDraftStart(Number(e.target.value))}
                                        className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                                    >
                                        {HOURS.map((hourLabel) => {
                                            const minutes = parseInt(hourLabel, 10) * 60;
                                            return (
                                                <option key={hourLabel} value={minutes}>
                                                    {hourLabel}
                                                </option>
                                            );
                                        })}
                                    </select>
                                </div>
                            </div>

                            <div className="flex flex-col gap-1">
                                <label className="text-sm text-gray-700">Duration</label>
                                <div className="flex overflow-hidden rounded-md border border-gray-300 text-sm">
                                    {DURATIONS.map((d) => (
                                        <button
                                            key={d}
                                            onClick={() => setDraftDuration(d)}
                                            className={`flex-1 border-l border-gray-300 py-1.5 first:border-l-0 ${
                                                d === draftDuration ? "bg-coral text-white" : "text-gray-600"
                                            }`}
                                        >
                                            {d < 60 ? `${d} min` : `${d / 60} h`}
                                        </button>
                                    ))}
                                </div>
                            </div>

                            <div className="flex items-center justify-between rounded-md border border-gray-200 bg-white px-3 py-2 text-sm">
                                <span>
                                    {formatTime(draftStart)} – {formatTime(draftEnd)}
                                </span>
                                <span className={`font-semibold ${clash ? "text-coral" : "text-green-700"}`}>
                                    {!scheduleReady ? "Unavailable" : invalidTime ? "Invalid time" : clash ? "Room busy" : "Available"}
                                </span>
                            </div>

                            {invalidTime && <p role="alert" className="text-sm text-red-600">Choose a future time ending by {END_HOUR}:00.</p>}
                            {createBooking.isError && <p role="alert" className="text-sm text-red-600">{roomErrorMessage(createBooking.error)}</p>}
                            <Button onClick={handleBook} disabled={clash || invalidTime || !title.trim() || !scheduleReady || createBooking.isPending} className="mt-auto">
                                {createBooking.isPending ? "Booking..." : `Book ${selectedRoom.name}`}
                            </Button>
                        </>
                    ) : (
                        <div className="flex flex-1 flex-col items-center justify-center gap-3 px-4 text-center">
                            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-[#e6ecf2] text-navy">
                                <CalendarPlus size={20} />
                            </div>
                            <p className="text-sm text-gray-500">
                                Select an available time slot in the schedule or click New booking to create a reservation.
                            </p>
                            {justBooked && (
                                <div className="rounded-md bg-green-50 px-3 py-2 text-sm text-green-700">
                                    Booked, added to {selectedRoom.name}'s schedule.
                                </div>
                            )}
                        </div>
                    )}
                </fieldset>
            </div>
        </div>
    );
}
