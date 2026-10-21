export interface Room {
  id: number;
  name: string;
  meta: string;
  short: string;
  seats: number;
}

export interface RoomBooking {
  id: string;
  roomId: number;
  day: number;
  startMin: number;
  durationMin: number;
  title: string;
  who: string;
}

export type RoomsViewMode = "day" | "week";

export const DURATIONS = [30, 60, 90, 120] as const;
export type Duration = (typeof DURATIONS)[number];