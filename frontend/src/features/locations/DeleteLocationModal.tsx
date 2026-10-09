import { Modal } from "../../components/Modal";
import { Button } from "../../components/Button";
import { useDeleteLocation } from "./useLocationMutations";
import type { Location } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    location: Location | null;
    employeeCount: number;
};

export function DeleteLocationModal({ open, onClose, location, employeeCount }: Props) {
    const deleteMutation = useDeleteLocation();

    if (!location) return null;

    function handleClose() {
        deleteMutation.reset();
        onClose();
    }

    function handleConfirm() {
        deleteMutation.mutate(location!.id, { onSuccess: handleClose });
    }

    return (
        <Modal open={open} onClose={handleClose} title="Delete location">
            <p className="mb-4 text-sm text-gray-600">
                Are you sure you want to delete <strong>{location.name}</strong>?
                {employeeCount > 0 &&
                    ` It currently has ${employeeCount} employee${employeeCount === 1 ? "" : "s"}.`}
            </p>
            {deleteMutation.isError && (
                <p className="mb-4 text-sm text-red-600">
                    Couldn't delete this location. It may still be assigned to employees.
                </p>
            )}
            <div className="flex gap-2">
                <Button variant="secondary" onClick={handleClose}>
                    Cancel
                </Button>
                <Button onClick={handleConfirm} disabled={deleteMutation.isPending} className="bg-red-600">
                    {deleteMutation.isPending ? "Deleting..." : "Delete"}
                </Button>
            </div>
        </Modal>
    );
}
