import { useMemo, useState } from "react";
import { useLocations } from "./useLocations";
import { useEmployeeDirectory } from "../employees/useEmployees";
import { LocationFormModal } from "./LocationFormModal";
import { DeleteLocationModal } from "./DeleteLocationModal";
import { DirectoryTable } from "../../components/DirectoryTable";
import type { Location } from "./types";

export default function LocationsPage() {
    const [formOpen, setFormOpen] = useState(false);
    const [editingLocation, setEditingLocation] = useState<Location | undefined>(undefined);
    const [deletingLocation, setDeletingLocation] = useState<Location | null>(null);

    const { data: locations, isLoading, isError, error } = useLocations();
    const { data: directory } = useEmployeeDirectory();

    const employeeCountByLocation = useMemo(() => {
        const map = new Map<number, number>();
        directory?.items.forEach((e) => {
            if (e.locationId !== null) {
                map.set(e.locationId, (map.get(e.locationId) ?? 0) + 1);
            }
        });
        return map;
    }, [directory]);

    function openCreate() {
        setEditingLocation(undefined);
        setFormOpen(true);
    }

    function openEdit(location: Location) {
        setEditingLocation(location);
        setFormOpen(true);
    }

    if (isLoading) return <div className="p-6">Loading locations...</div>;
    if (isError) return <div className="p-6 text-red-600">Error: {(error as Error).message}</div>;

    return (
        <div className="p-6">
            <DirectoryTable
                title="Locations"
                addLabel="Add location"
                items={locations ?? []}
                emptyMessage="No locations yet. Add a location to get started."
                employeeCount={(location) => employeeCountByLocation.get(location.id) ?? 0}
                onAdd={openCreate}
                onEdit={openEdit}
                onDelete={setDeletingLocation}
            />

            <LocationFormModal
                key={editingLocation?.id ?? "new"}
                open={formOpen}
                onClose={() => setFormOpen(false)}
                location={editingLocation}
            />
            <DeleteLocationModal
                open={!!deletingLocation}
                onClose={() => setDeletingLocation(null)}
                location={deletingLocation}
                employeeCount={deletingLocation ? employeeCountByLocation.get(deletingLocation.id) ?? 0 : 0}
            />
        </div>
    );
}
