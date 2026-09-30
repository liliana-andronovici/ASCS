import type { Room, RoomBooking } from "./types/room.ts";


export const ROOMS: Room[] = [
    { id: 0, name: "Atrium", meta: "Seats 12 · screen · floor 2", short: "AT", seats: 12 },
    { id: 1, name: "Loft", meta: "Seats 6 · whiteboard · floor 3", short: "LO", seats: 6 },
    { id: 2, name: "Nook", meta: "Seats 3 · phone booth", short: "NK", seats: 3 },
    { id: 3, name: "Terrace", meta: "Seats 8 · video · floor 4", short: "TE", seats: 8 },
];

export const DAY_LABELS = ["Mon", "Tue", "Wed", "Thu", "Fri"];
export const DAY_NUMBERS = [1, 2, 3, 4, 5];

export const INITIAL_BOOKINGS: RoomBooking[] = [
    { id: "b1", roomId: 0, day: 0, startMin: 9 * 60, durationMin: 60, title: "Leadership sync", who: "Radu" },
    { id: "b2", roomId: 0, day: 1, startMin: 14 * 60, durationMin: 60, title: "All hands prep", who: "Radu" },
    { id: "b3", roomId: 0, day: 2, startMin: 10 * 60, durationMin: 120, title: "Board deck", who: "Radu" },
    { id: "b4", roomId: 0, day: 3, startMin: 11 * 60, durationMin: 90, title: "Q4 roadmap", who: "Elena" },
    { id: "b5", roomId: 0, day: 3, startMin: 15 * 60, durationMin: 60, title: "Candidate interview", who: "Ana" },
    { id: "b6", roomId: 0, day: 4, startMin: 16 * 60, durationMin: 60, title: "Town hall", who: "Ana" },
    { id: "b7", roomId: 1, day: 0, startMin: 10 * 60, durationMin: 30, title: "Standup", who: "Ioana" },
    { id: "b8", roomId: 1, day: 0, startMin: 13 * 60, durationMin: 90, title: "Onboarding kickoff", who: "Elena" },
    { id: "b9", roomId: 1, day: 1, startMin: 10 * 60, durationMin: 30, title: "Standup", who: "Ioana" },
    { id: "b10", roomId: 1, day: 1, startMin: 15 * 60, durationMin: 60, title: "Retro", who: "Andrei" },
    { id: "b11", roomId: 1, day: 2, startMin: 9 * 60, durationMin: 120, title: "Hiring committee", who: "Ana" },
    { id: "b12", roomId: 1, day: 3, startMin: 10 * 60, durationMin: 30, title: "Standup", who: "Ioana" },
    { id: "b13", roomId: 1, day: 3, startMin: 16 * 60, durationMin: 120, title: "Sprint planning", who: "Andrei" },
    { id: "b14", roomId: 1, day: 4, startMin: 10 * 60, durationMin: 30, title: "Standup", who: "Ioana" },
    { id: "b15", roomId: 1, day: 4, startMin: 11 * 60, durationMin: 60, title: "Vendor call", who: "Laura" },
    { id: "b16", roomId: 1, day: 4, startMin: 14 * 60, durationMin: 120, title: "Design crit", who: "Elena" },
    { id: "b17", roomId: 2, day: 0, startMin: 11 * 60, durationMin: 30, title: "1:1 Emil", who: "Ioana" },
    { id: "b18", roomId: 2, day: 2, startMin: 15 * 60, durationMin: 30, title: "1:1 Lucia", who: "Ioana" },
    { id: "b19", roomId: 2, day: 3, startMin: 13 * 60, durationMin: 30, title: "1:1 Sorin", who: "Cristina" },
    { id: "b20", roomId: 3, day: 0, startMin: 9 * 60 + 30, durationMin: 60, title: "Care ops weekly", who: "Cristina" },
    { id: "b21", roomId: 3, day: 1, startMin: 13 * 60, durationMin: 60, title: "Client demo", who: "Sorin" },
    { id: "b22", roomId: 3, day: 3, startMin: 9 * 60 + 30, durationMin: 60, title: "Care ops weekly", who: "Cristina" },
    { id: "b23", roomId: 3, day: 3, startMin: 14 * 60, durationMin: 60, title: "Payroll close", who: "Laura" },
];

export const START_HOUR = 8;
export const HOURS = Array.from({ length: 11 }, (_, i) => `${START_HOUR + i}:00`);
export const ROW_HEIGHT = 60;