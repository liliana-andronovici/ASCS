import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createRoom } from "./api";
import { getLocations } from "../locations/api";
import { Link } from "react-router-dom";

export function RoomSetup({ onClose }: { onClose?: () => void }) {
    const offices = useQuery({ queryKey: ["locations"], queryFn: getLocations });
    const client = useQueryClient();
    const [name, setName] = useState("");
    const [office, setOffice] = useState("");
    const [floor, setFloor] = useState(0);
    const [seats, setSeats] = useState(6);
    const selectedOffice = office || String(offices.data?.[0]?.id ?? "");
    const create = useMutation({
        mutationFn: () => createRoom({
            name: name.trim(), locationId: Number(selectedOffice), floor, seats,
        }),
        onSuccess: async () => {
            await client.invalidateQueries({ queryKey: ["rooms"] });
            onClose?.();
        },
    });
    function submit(event: FormEvent) {
        event.preventDefault();
        if (!create.isPending && name.trim() && selectedOffice) create.mutate();
    }
    return <section className="m-4 rounded-lg border border-gray-200 bg-white p-5">
        <h2 className="text-lg font-semibold text-navy">Add meeting room</h2>
        {offices.isPending ? <p role="status">Loading offices...</p> : offices.isError ?
            <p role="alert">Unable to load offices. <button onClick={() => offices.refetch()} className="underline">Retry</button></p> :
            !offices.data?.length ? <p className="mt-3 text-sm text-gray-600">Create an office in <Link to="/locations" className="text-coral underline">Locations</Link>, then return here to add its meeting rooms.</p> :
            <form onSubmit={submit} className="mt-4 flex max-w-xl flex-col gap-3">
                <label>Room name<input required maxLength={150} value={name} onChange={event => setName(event.target.value)} className="mt-1 block w-full rounded border border-gray-300 p-2" /></label>
                <label>Office<select value={selectedOffice} onChange={event => setOffice(event.target.value)} className="mt-1 block w-full rounded border border-gray-300 p-2">{offices.data.map(location => <option key={location.id} value={location.id}>{location.name}</option>)}</select></label>
                <label>Floor<input type="number" required min={-20} max={200} value={floor} onChange={event => setFloor(Number(event.target.value))} className="ml-3 rounded border border-gray-300 p-2" /></label>
                <label>Seats<input type="number" required min={1} max={1000} value={seats} onChange={event => setSeats(Number(event.target.value))} className="ml-3 rounded border border-gray-300 p-2" /></label>
                {create.isError && <p role="alert" className="text-sm text-red-600">Could not add the room. Check its details and ensure the name is unique within the office.</p>}
                <button disabled={create.isPending || !name.trim()} className="rounded bg-coral px-4 py-2 text-white disabled:opacity-50">{create.isPending ? "Saving..." : "Save room"}</button>
            </form>}
        {onClose && <button onClick={onClose} disabled={create.isPending} className="mt-3 text-sm underline">Cancel</button>}
    </section>;
}
