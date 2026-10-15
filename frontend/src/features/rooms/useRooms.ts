import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createRoomBooking, getRoomBookings, getRooms } from "./api";

export function useRooms() {
    return useQuery({ queryKey: ["rooms"], queryFn: ({ signal }) => getRooms(signal), retry: false });
}

export function useRoomBookings(fromUtc: string, toUtc: string) {
    return useQuery({
        queryKey: ["room-bookings", fromUtc, toUtc],
        queryFn: ({ signal }) => getRoomBookings(fromUtc, toUtc, signal),
        retry: false, staleTime: 0,
    });
}

export function useCreateRoomBooking() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: createRoomBooking,
        retry: false,
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["room-bookings"] }),
        onError: () => queryClient.invalidateQueries({ queryKey: ["room-bookings"] }),
    });
}
