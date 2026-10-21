import { z } from "zod";
import { apiFetch } from "../../lib/api";
import { bookingSchema, roomSchema, type CreateRoomBookingInput } from "./contract";

const ROOMS_PATH = (import.meta.env.VITE_ROOMS_API_PATH ?? "/Rooms").replace(/\/$/, "");
const BOOKINGS_PATH = (import.meta.env.VITE_ROOM_BOOKINGS_API_PATH ?? "/RoomBookings").replace(/\/$/, "");
export async function createRoom(input: Pick<z.infer<typeof roomSchema>, "name" | "locationId" | "floor" | "seats">): Promise<void> {
    await apiFetch<unknown>(ROOMS_PATH, { method: "POST", body: JSON.stringify(input) });
}

export async function getRooms(signal?: AbortSignal) {
    return z.array(roomSchema).parse(await apiFetch<unknown>(ROOMS_PATH, { signal }));
}

export async function getRoomBookings(fromUtc: string, toUtc: string, signal?: AbortSignal) {
    const query = new URLSearchParams({ fromUtc, toUtc });
    return z.array(bookingSchema).parse(await apiFetch<unknown>(`${BOOKINGS_PATH}?${query}`, { signal }));
}

export async function createRoomBooking(input: CreateRoomBookingInput): Promise<void> {
    await apiFetch<unknown>(BOOKINGS_PATH, { method: "POST", body: JSON.stringify(input) });
}
