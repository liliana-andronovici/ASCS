import { z } from "zod";

// Matches RoomModel and RoomBookingModel returned by the ASCS Rooms API.
export const roomSchema = z.object({
    id: z.number().int().positive(),
    name: z.string().min(1),
    floor: z.number().int(),
    seats: z.number().int().nonnegative(),
    amenities: z.string().nullable().optional(),
    locationId: z.number().int().positive(),
    locationName: z.string().optional(),
});

export const bookingSchema = z.object({
    id: z.number().int().positive(),
    roomId: z.number().int().positive(),
    organizerEmployeeId: z.number().int().positive(),
    organizerName: z.string().optional(),
    title: z.string().min(1),
    startDateTimeUtc: z.iso.datetime({ offset: true }),
    durationMinutes: z.number().int().positive(),
});

export type RoomDto = z.infer<typeof roomSchema>;
export type RoomBookingDto = z.infer<typeof bookingSchema>;

export interface CreateRoomBookingInput {
    roomId: number;
    title: string;
    startDateTimeUtc: string;
    durationMinutes: number;
}
