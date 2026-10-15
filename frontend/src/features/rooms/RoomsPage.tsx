import { useState } from "react";
import { RoomsSchedule } from "./RoomsSchedule";
import { toRoom } from "./calendar";
import { roomErrorMessage } from "./errors";
import { useRooms } from "./useRooms";

export default function RoomsPage() {
    const query = useRooms();
    const [location, setLocation] = useState("all");
    if (query.isPending) return <div className="p-6" role="status"><h1 className="text-xl font-bold text-navy">Meeting rooms</h1><p className="mt-4 text-sm text-gray-500">Loading meeting rooms...</p></div>;
    if (query.isError) return <div className="p-6"><h1 className="text-xl font-bold text-navy">Meeting rooms</h1><p role="alert" className="my-4 text-sm text-red-600">{roomErrorMessage(query.error)}</p><button onClick={() => query.refetch()} disabled={query.isFetching} className="text-sm text-coral underline">{query.isFetching ? "Retrying..." : "Retry"}</button></div>;
    const records = query.data;
    if (!records.length) return <div className="p-6"><h1 className="text-xl font-bold text-navy">Meeting rooms</h1><p role="status" className="mt-4 text-sm text-gray-500">No meeting rooms have been configured yet.</p></div>;
    const locations = new Map(records.map((room) => [String(room.locationId), room.locationName || `Location #${room.locationId}`]));
    const effectiveLocation = location === "all" || locations.has(location) ? location : "all";
    const rooms = records.filter((room) => effectiveLocation === "all" || String(room.locationId) === effectiveLocation).map(toRoom);
    return <RoomsSchedule key={effectiveLocation} rooms={rooms} officeSelector={
        <label className="flex items-center gap-2 text-sm text-gray-500">Office
            <select aria-label="Office" value={effectiveLocation} onChange={(event) => setLocation(event.target.value)} className="rounded-full border border-gray-300 bg-white px-3 py-2 text-navy">
                <option value="all">All offices</option>
                {[...locations].map(([id, name]) => <option key={id} value={id}>{name}</option>)}
            </select>
        </label>
    } />;
}
