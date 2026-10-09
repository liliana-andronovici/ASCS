import { useMemo, useState } from "react";
import { PenLine, Plus, Trash2 } from "lucide-react";
import { useLocations } from "./useLocations";
import { useEmployeeDirectory } from "../employees/useEmployees";
import { LocationFormModal } from "./LocationFormModal";
import { DeleteLocationModal } from "./DeleteLocationModal";
import { Button } from "../../components/Button";
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
        <div className="flex min-h-full flex-col bg-white px-6 py-6">
            <div className="mb-5 flex items-center justify-between">
                <h1 className="text-xl font-bold text-navy">Locations</h1>
                <Button onClick={openCreate} className="!w-auto px-4">
                    <span className="flex items-center gap-1.5">
                        <Plus size={15} />
                        Add location
                    </span>
                </Button>
            </div>

            <div className="overflow-hidden rounded-[10px] border border-gray-300">
                <table className="w-full text-left text-[13px]">
                    <thead>
                    <tr className="bg-gray-50">
                        <th className="border-b border-gray-300 px-4 py-3 font-medium">Name</th>
                        <th className="w-[180px] border-b border-gray-300 px-4 py-3 font-medium">
                            No. of employees
                        </th>
                        <th className="w-[90px] border-b border-gray-300 px-4 py-3 text-center font-medium">
                            Modify
                        </th>
                        <th className="w-[90px] border-b border-gray-300 px-4 py-3 text-center font-medium">
                            Delete
                        </th>
                    </tr>
                    </thead>
                    <tbody>
                    {locations?.length === 0 && (
                        <tr>
                            <td colSpan={4} className="px-4 py-8 text-center text-gray-500">
                                No locations yet.
                            </td>
                        </tr>
                    )}
                    {locations?.map((location) => (
                        <tr key={location.id} className="border-b border-gray-100 last:border-0">
                            <td className="px-4 py-3.5 font-semibold text-navy">{location.name}</td>
                            <td className="px-4 py-3.5 text-gray-700">
                                {employeeCountByLocation.get(location.id) ?? 0}
                            </td>
                            <td className="px-4 py-3.5 text-center">
                                <button
                                    title="Modify location"
                                    onClick={() => openEdit(location)}
                                    className="text-navy hover:opacity-70"
                                >
                                    <PenLine size={18} />
                                </button>
                            </td>
                            <td className="px-4 py-3.5 text-center">
                                <button
                                    title="Delete location"
                                    onClick={() => setDeletingLocation(location)}
                                    className="text-[#b93f38] hover:opacity-70"
                                >
                                    <Trash2 size={18} />
                                </button>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>

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