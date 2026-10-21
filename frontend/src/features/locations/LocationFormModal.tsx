import { useState } from "react";
import { Modal } from "../../components/Modal";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { useLocations } from "./useLocations";
import { useCreateLocation, useUpdateLocation } from "./useLocationMutations";
import type { Location } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    location?: Location;
};

export function LocationFormModal({ open, onClose, location }: Props) {
    const [name, setName] = useState(location?.name ?? "");
    const [validationError, setValidationError] = useState<string | null>(null);

    const { data: locations } = useLocations();
    const createMutation = useCreateLocation();
    const updateMutation = useUpdateLocation();

    const isPending = createMutation.isPending || updateMutation.isPending;
    const saveFailed = createMutation.isError || updateMutation.isError;

    function handleClose() {
        setName(location?.name ?? "");
        setValidationError(null);
        createMutation.reset();
        updateMutation.reset();
        onClose();
    }

    function handleSubmit() {
        const trimmed = name.trim();
        if (!trimmed) {
            setValidationError("Name is required.");
            return;
        }
        const exists = locations?.some(
            (l) => l.id !== location?.id && l.name.toLowerCase() === trimmed.toLowerCase()
        );
        if (exists) {
            setValidationError("This location already exists.");
            return;
        }
        setValidationError(null);

        const onSuccess = () => {
            setName(location ? trimmed : "");
            onClose();
        };

        if (location) {
            updateMutation.mutate({ id: location.id, name: trimmed }, { onSuccess });
        } else {
            createMutation.mutate({ name: trimmed }, { onSuccess });
        }
    }

    const message = validationError ?? (saveFailed ? "Couldn't save the location. Please try again." : null);

    return (
        <Modal open={open} onClose={handleClose} title={location ? "Rename location" : "Add location"}>
            <Input
                id="location-name"
                label="Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                onKeyDown={(e) => {
                    if (e.key === "Enter") handleSubmit();
                }}
                autoFocus
            />
            {message && <p className="mt-2 text-sm text-red-600">{message}</p>}
            <div className="mt-4 flex gap-2">
                <Button variant="secondary" onClick={handleClose}>
                    Cancel
                </Button>
                <Button onClick={handleSubmit} disabled={isPending}>
                    {isPending ? "Saving..." : "Save"}
                </Button>
            </div>
        </Modal>
    );
}
