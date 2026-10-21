import { Modal } from "../../components/Modal";
import { Button } from "../../components/Button";
import { useDeleteEmployee } from "./useEmployeeMutations";
import type { Employee } from "./types";

type Props = {
    open: boolean;
    onClose: () => void;
    employee: Employee | null;
};

export function DeleteEmployeeModal({ open, onClose, employee }: Props) {
    const deleteMutation = useDeleteEmployee();

    if (!employee) return null;

    function handleConfirm() {
        deleteMutation.mutate(employee!.id, { onSuccess: onClose });
    }

    return (
        <Modal open={open} onClose={onClose} title="Remove employee">
            <p className="text-sm text-gray-600 mb-4">
                Are you sure you want to remove <strong>{employee.firstName} {employee.lastName}</strong>? This can't be undone.
            </p>
            <div className="flex gap-2">
                <Button variant="secondary" onClick={onClose}>
                    Cancel
                </Button>
                <Button onClick={handleConfirm} disabled={deleteMutation.isPending} className="bg-red-600">
                    {deleteMutation.isPending ? "Removing..." : "Remove"}
                </Button>
            </div>
        </Modal>
    );
}