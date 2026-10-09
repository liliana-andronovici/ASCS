import { Fragment, useMemo, useState } from "react";
import { ChevronLeft, ChevronRight, CalendarPlus } from "lucide-react";
import { Button } from "../../components/Button";
import {
    ROOMS,
    DAY_LABELS,
    INITIAL_BOOKINGS,
    START_HOUR,
    HOURS,
    ROW_HEIGHT,
} from "./mockData";
import { DURATIONS } from "./types/room.ts";
import type { RoomBooking, RoomsViewMode } from "./types/room.ts";

const TODAY_DAY_INDEX = 2; // Miercuri, in saptamana de referinta (weekOffset 0)
const BASE_MONDAY = new Date(2026, 8, 1); // 1 Sep 2026, luni de referinta pentru acest set de date exemplu

function getWeekDates(weekOffset: number): Date[] {
    return Array.from({ length: 5 }, (_, i) => {
        const d = new Date(BASE_MONDAY);
        d.setDate(d.getDate() + weekOffset * 7 + i);
        return d;
    });
}

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

export default function RoomsPage() {
    const [view, setView] = useState<RoomsViewMode>("week");
    const [weekOffset, setWeekOffset] = useState(0);
    const [selectedRoomId, setSelectedRoomId] = useState(ROOMS[0].id);
    const [selectedDay, setSelectedDay] = useState(TODAY_DAY_INDEX);
    const [draftOpen, setDraftOpen] = useState(false);
    const [draftStart, setDraftStart] = useState(14 * 60);
    const [draftDuration, setDraftDuration] = useState<(typeof DURATIONS)[number]>(60);
    const [title, setTitle] = useState("Design review — onboarding flow");
    const [bookings, setBookings] = useState<RoomBooking[]>(INITIAL_BOOKINGS);
    const [justBooked, setJustBooked] = useState(false);

    const weekDates = useMemo(() => getWeekDates(weekOffset), [weekOffset]);
    const selectedRoom = ROOMS.find((r) => r.id === selectedRoomId)!;
    const draftEnd = draftStart + draftDuration;

    const clash = bookings.some(
        (b) =>
            b.roomId === selectedRoomId &&
            b.day === selectedDay &&
            overlaps(draftStart, draftDuration, b.startMin, b.durationMin)
    );

    function goToday() {
        setWeekOffset(0);
        setSelectedDay(TODAY_DAY_INDEX);
    }

    function goPrev() {
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
        setJustBooked(false);
        setSelectedDay(day);
        setDraftStart(parseInt(hourLabel, 10) * 60);
        if (roomId !== undefined) setSelectedRoomId(roomId);
        setDraftOpen(true);
    }

    function handleBook() {
        if (clash) return;
        const booking: RoomBooking = {
            id: `local-${Date.now()}`,
            roomId: selectedRoomId,
            day: selectedDay,
            startMin: draftStart,
            durationMin: draftDuration,
            title,
            who: "You",
        };
        setBookings((prev) => [...prev, booking]);
        setJustBooked(true);
        setDraftOpen(false);
    }

    const roomFreePercent = useMemo(() => {
        const totalMinutesInWeek = 5 * 10 * 60;
        const map = new Map<number, number>();
        ROOMS.forEach((room) => {
            const busy = bookings
                .filter((b) => b.roomId === room.id)
                .reduce((sum, b) => sum + b.durationMin, 0);
            map.set(room.id, Math.max(0, Math.round(100 - (busy / totalMinutesInWeek) * 100)));
        });
        return map;
    }, [bookings]);

    const columnCount = view === "week" ? 5 : ROOMS.length;

    return (
        <div className="p-2 sm:p-4 lg:p-6">
            <div className="flex min-w-0 flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-card xl:flex-row">
                <div className="flex min-w-0 flex-1 flex-col">
                    <div className="flex flex-wrap items-center gap-2 border-b border-gray-200 px-3 py-3 sm:gap-3 sm:px-6 sm:py-4">
                        <h1 className="text-xl font-bold text-navy">Meeting rooms</h1>

                        <div className="flex items-center gap-1">
                            <button
                                onClick={goPrev}
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
                                onClick={goNext}
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

                        <span
                            title="Only one office is set up in this preview"
                            className="rounded-full border border-gray-200 px-4 py-2 text-sm text-gray-400"
                        >
                            Office <span className="font-semibold">Chișinău</span>
                        </span>

                        <Button
                            onClick={() => {
                                setJustBooked(false);
                                setDraftOpen(true);
                            }}
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
                            {ROOMS.map((room) => (
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
                                        {roomFreePercent.get(room.id)}% free
                                    </span>
                                </button>
                            ))}
                        </div>
                    )}

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
                                            i === TODAY_DAY_INDEX && weekOffset === 0
                                                ? "bg-coral text-white"
                                                : "text-navy"
                                        }`}
                                    >
                                          {weekDates[i].getDate()}
                                      </span>
                                </div>
                            ))
                            : ROOMS.map((room) => (
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
                                        : ROOMS.map((room) => (
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
                            const columnIndex = view === "week" ? b.day : ROOMS.findIndex((r) => r.id === b.roomId);
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

                        {draftOpen &&
                            (() => {
                                const columnIndex =
                                    view === "week" ? selectedDay : ROOMS.findIndex((r) => r.id === selectedRoomId);
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

                <aside className="flex w-full min-w-0 flex-col gap-3.5 border-t border-gray-200 bg-[#f8fafc] p-3 sm:p-5 xl:w-[340px] xl:shrink-0 xl:border-l xl:border-t-0">
                    {draftOpen ? (
                        <>
                            <div className="flex items-baseline justify-between">
                                <h4 className="text-lg font-bold text-navy">New booking</h4>
                                <button onClick={() => setDraftOpen(false)} className="text-xs text-gray-400 hover:underline">
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
                                    {ROOMS.map((room) => (
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
                                <label className="text-sm text-gray-700">Title</label>
                                <input
                                    value={title}
                                    onChange={(e) => setTitle(e.target.value)}
                                    className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none"
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-3">
                                <div className="flex flex-col gap-1">
                                    <label className="text-sm text-gray-700">Day</label>
                                    <select
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
                                    <label className="text-sm text-gray-700">Starts</label>
                                    <select
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
                                    {clash ? "Room busy" : "Available"}
                                </span>
                            </div>

                            <Button onClick={handleBook} disabled={clash} className="mt-auto">
                                Book {selectedRoom.name}
                            </Button>
                        </>
                    ) : (
                        <div className="flex flex-1 flex-col items-center justify-center gap-3 px-4 text-center">
                            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-[#e6ecf2] text-navy">
                                <CalendarPlus size={20} />
                            </div>
                            <p className="text-sm text-gray-500">
                                Selectează un interval liber în program sau apasă New booking ca să creezi o rezervare.
                            </p>
                            {justBooked && (
                                <div className="rounded-md bg-green-50 px-3 py-2 text-sm text-green-700">
                                    Booked, added to {selectedRoom.name}'s schedule.
                                </div>
                            )}
                        </div>
                    )}
                </aside>
            </div>
        </div>
    );
}