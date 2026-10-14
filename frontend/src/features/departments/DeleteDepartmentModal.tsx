import { Modal } from "../../components/Modal";
import { Button } from "../../components/Button";
import { useDeleteDepartment } from "./useDepartmentMutations";
import type { Department } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    department: Department | null;
};

export function DeleteDepartmentModal({ open, onClose, department }: Props) {
    const deleteMutation = useDeleteDepartment();

    if (!department) return null;

    function handleClose() {
        if (!deleteMutation.isPending) onClose();
    }

    function handleConfirm() {
        if (!department || deleteMutation.isPending) return;
        deleteMutation.mutate(department.id, { onSuccess: onClose });
    }

    return (
        <Modal open={open} onClose={handleClose} title="Delete department">
            <p className="text-sm text-gray-600 mb-4">
                Are you sure you want to delete <strong>{department.name}</strong>? This can't be undone.
            </p>
            {deleteMutation.error && (
                <p role="alert" className="text-sm text-red-600 mb-4">
                    {deleteMutation.error.message || "Something went wrong. Please try again."}
                </p>
            )}
            <div className="flex gap-2">
                <Button variant="secondary" onClick={handleClose} disabled={deleteMutation.isPending}>
                    Cancel
                </Button>
                <Button onClick={handleConfirm} disabled={deleteMutation.isPending}>
                    {deleteMutation.isPending ? "Deleting..." : "Delete"}
                </Button>
            </div>
        </Modal>
    );
}
