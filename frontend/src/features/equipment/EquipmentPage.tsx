import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { Modal } from "../../components/Modal";
import { Select } from "../../components/Select";
import { ApiError } from "../../lib/api";
import { createEquipment, deleteEquipment, getEquipment, getEquipmentTypes, queueEquipmentType, updateEquipment, type Equipment } from "./api";

function errorMessage(error: Error | null): string {
    if (error instanceof ApiError) {
        try {
            const response = JSON.parse(error.message) as { message?: string; errors?: string[] };
            return response.message ?? response.errors?.join(" ") ?? "Could not save asset.";
        } catch {
            return error.message;
        }
    }
    return "Could not save asset.";
}

export default function EquipmentPage() {
    const queryClient = useQueryClient();
    const [name, setName] = useState("");
    const [code, setCode] = useState("");
    const [equipmentTypeId, setEquipmentTypeId] = useState("");
    const [newTypeName, setNewTypeName] = useState("");
    const [typeNotice, setTypeNotice] = useState("");
    const [editing, setEditing] = useState<Equipment | null>(null);
    const [deleting, setDeleting] = useState<Equipment | null>(null);
    const [notice, setNotice] = useState("");
    const equipmentQuery = useQuery({ queryKey: ["equipment"], queryFn: getEquipment });
    const typeQuery = useQuery({ queryKey: ["equipmentTypes"], queryFn: getEquipmentTypes });
    const saveMutation = useMutation({
        mutationFn: async () => {
            const input = { name: name.trim(), code: code.trim(), equipmentTypeId: Number(equipmentTypeId) };
            if (editing) await updateEquipment(editing.id, input);
            else await createEquipment(input);
        },
        onSuccess: async () => {
            setNotice(editing ? "Asset updated." : "Asset added.");
            setEditing(null);
            setName("");
            setCode("");
            setEquipmentTypeId("");
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ["equipment"] }),
                queryClient.invalidateQueries({ queryKey: ["employees"] }),
            ]);
        },
    });
    const typeMutation = useMutation({
        mutationFn: () => queueEquipmentType(newTypeName.trim()),
        onSuccess: ({ jobId }) => {
            setTypeNotice(`Type creation queued (job ${jobId}). Refresh the type list after it finishes.`);
            setNewTypeName("");
        },
    });
    const deleteMutation = useMutation({
        mutationFn: (id: number) => deleteEquipment(id),
        onSuccess: async () => {
            setNotice("Asset deleted.");
            setDeleting(null);
            await queryClient.invalidateQueries({ queryKey: ["equipment"] });
        },
    });

    function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!name.trim() || !code.trim() || !equipmentTypeId) return;
        setNotice("");
        saveMutation.mutate();
    }

    return (
        <div className="p-6">
            <div className="rounded-xl border border-gray-200 bg-white shadow-card">
                <div className="border-b border-gray-200 px-6 py-5">
                    <h1 className="text-xl font-bold text-navy">Assets</h1>
                    <p className="mt-1 text-sm text-gray-500">Manage individual assets here. Assign them from an employee's Assets tab.</p>
                </div>
                <div className="grid gap-8 p-6 lg:grid-cols-2">
                    <form onSubmit={submit} className="flex min-w-0 flex-col gap-4">
                        <h2 className="text-base font-semibold text-navy">{editing ? "Edit asset" : "Add asset"}</h2>
                        <Input id="equipment-name" label="Name" value={name} onChange={(event) => setName(event.target.value)} maxLength={150} required disabled={saveMutation.isPending} placeholder="Logitech MX Master" />
                        <Input id="equipment-code" label="Code" value={code} onChange={(event) => setCode(event.target.value)} maxLength={80} required disabled={saveMutation.isPending} placeholder="MOU-001" />
                        <Select id="equipment-type" label="Type" value={equipmentTypeId} onChange={(event) => setEquipmentTypeId(event.target.value)} required disabled={typeQuery.isPending || typeQuery.isError || saveMutation.isPending} options={[
                            { value: "", label: "Select a type" },
                            ...(typeQuery.data ?? []).map((type) => ({ value: String(type.id), label: type.name })),
                        ]} />
                        {typeQuery.isError && <p role="alert" className="text-sm text-red-600">Could not load asset types.</p>}
                        {saveMutation.isError && <p role="alert" className="text-sm text-red-600">{errorMessage(saveMutation.error)}</p>}
                        {notice && <p role="status" className="text-sm text-green-700">{notice}</p>}
                        <div className="flex gap-3">
                            <Button type="submit" disabled={saveMutation.isPending || !name.trim() || !code.trim() || !equipmentTypeId}>{saveMutation.isPending ? "Saving..." : editing ? "Save changes" : "Add asset"}</Button>
                            {editing && <Button type="button" variant="secondary" onClick={() => { setEditing(null); setName(""); setCode(""); setEquipmentTypeId(""); saveMutation.reset(); }}>Cancel</Button>}
                        </div>
                    </form>
                    <section aria-label="Asset catalog" className="min-w-0">
                        <h2 className="text-base font-semibold text-navy">Assets</h2>
                        {equipmentQuery.isPending && <p role="status" className="mt-4 text-sm text-gray-500">Loading assets...</p>}
                        {equipmentQuery.isError && <p role="alert" className="mt-4 text-sm text-red-600">Could not load assets.</p>}
                        {equipmentQuery.data?.length === 0 && <p className="mt-4 text-sm text-gray-500">No assets yet. Add a named item with its sticker code and type.</p>}
                        {equipmentQuery.data && equipmentQuery.data.length > 0 && (
                            <div className="mt-4 overflow-x-auto">
                                <table className="min-w-full text-left text-sm">
                                    <thead className="border-b border-gray-200 text-gray-500"><tr><th scope="col" className="py-3 pr-5 font-medium">Name</th><th scope="col" className="py-3 pr-5 font-medium">Code</th><th scope="col" className="py-3 pr-5 font-medium">Type</th><th scope="col" className="py-3 pr-5 font-medium">Status</th><th scope="col" className="py-3 font-medium">Actions</th></tr></thead>
                                    <tbody>{equipmentQuery.data.map((item) => (
                                        <tr key={item.id} className="border-b border-gray-100 last:border-0">
                                            <td className="py-3 pr-5 font-medium text-navy">{item.name}</td>
                                            <td className="py-3 pr-5 text-gray-700">{item.code}</td>
                                            <td className="py-3 pr-5 text-gray-700">{item.typeName}</td>
                                            <td className="py-3 pr-5 text-gray-700">{item.isAssigned ? "Assigned" : "Available"}</td>
                                            <td className="flex gap-4 py-3">
                                                <button type="button" className="font-medium text-coral hover:underline" onClick={() => { setEditing(item); setName(item.name); setCode(item.code); setEquipmentTypeId(String(item.equipmentTypeId)); setNotice(""); saveMutation.reset(); }}>Edit</button>
                                                <button type="button" className="font-medium text-red-600 hover:underline" onClick={() => { setDeleting(item); deleteMutation.reset(); }}>Delete</button>
                                            </td>
                                        </tr>
                                    ))}</tbody>
                                </table>
                            </div>
                        )}
                    </section>
                </div>
                <form onSubmit={(event) => { event.preventDefault(); if (newTypeName.trim()) typeMutation.mutate(); }} className="flex flex-col gap-3 border-t border-gray-200 px-6 py-5 sm:flex-row sm:items-end">
                    <div className="sm:w-72"><Input id="new-equipment-type" label="Add a new type" value={newTypeName} onChange={(event) => { setNewTypeName(event.target.value); setTypeNotice(""); }} maxLength={80} required placeholder="Webcam" /></div>
                    <Button type="submit" className="sm:w-auto" disabled={!newTypeName.trim() || typeMutation.isPending}>{typeMutation.isPending ? "Queuing..." : "Add type"}</Button>
                    <Button type="button" variant="secondary" className="sm:w-auto" onClick={() => typeQuery.refetch()}>Refresh types</Button>
                    {typeMutation.isError && <p role="alert" className="text-sm text-red-600">{errorMessage(typeMutation.error)}</p>}
                    {typeNotice && <p role="status" className="text-sm text-green-700">{typeNotice}</p>}
                </form>
            </div>
            <Modal open={deleting !== null} onClose={() => setDeleting(null)} title="Delete asset">
                <p className="text-sm text-gray-700">Delete {deleting?.name}? Assets with assignment history cannot be deleted.</p>
                {deleteMutation.isError && <p role="alert" className="mt-3 text-sm text-red-600">{errorMessage(deleteMutation.error)}</p>}
                <div className="mt-5 flex gap-3">
                    <Button type="button" variant="secondary" onClick={() => setDeleting(null)}>Cancel</Button>
                    <Button type="button" onClick={() => deleting && deleteMutation.mutate(deleting.id)} disabled={deleteMutation.isPending}>{deleteMutation.isPending ? "Deleting..." : "Delete"}</Button>
                </div>
            </Modal>
        </div>
    );
}
