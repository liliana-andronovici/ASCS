import { useState } from "react";
import type { ReactNode } from "react";
import { Search } from "lucide-react";
import { Button } from "./Button";
import { Checkbox } from "./Checkbox";
import { NameSortButton } from "./NameSortButton";
import type { NameSortDirection } from "./NameSortButton";

type DirectoryItem = { id: number; name: string };

type Props<T extends DirectoryItem> = {
    title: string;
    addLabel: string;
    items: T[];
    emptyMessage: string;
    employeeCount: (item: T) => ReactNode;
    onAdd: () => void;
    onEdit: (item: T) => void;
    onDelete: (item: T) => void;
    children?: ReactNode;
};

const PAGE_SIZE = 10;

export function DirectoryTable<T extends DirectoryItem>({
    title, addLabel, items, emptyMessage, employeeCount, onAdd, onEdit, onDelete, children,
}: Props<T>) {
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [nameSort, setNameSort] = useState<NameSortDirection>(undefined);
    const [selected, setSelected] = useState<Set<number>>(new Set());

    const query = search.trim().toLocaleLowerCase();
    const filtered = items.filter((item) => item.name.toLocaleLowerCase().includes(query));
    if (nameSort) {
        filtered.sort((left, right) => {
            const order = left.name.localeCompare(right.name, undefined, { sensitivity: "base", numeric: true });
            return nameSort === "Ascending" ? order : -order;
        });
    }
    const currentPage = Math.min(page, Math.max(1, Math.ceil(filtered.length / PAGE_SIZE)));
    const offset = (currentPage - 1) * PAGE_SIZE;
    const visibleItems = filtered.slice(offset, offset + PAGE_SIZE);
    const allSelected = visibleItems.length > 0 && visibleItems.every((item) => selected.has(item.id));

    function toggleRow(id: number) {
        setSelected((previous) => {
            const next = new Set(previous);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    }

    function toggleAll() {
        setSelected((previous) => {
            const next = new Set(previous);
            visibleItems.forEach((item) => {
                if (allSelected) next.delete(item.id);
                else next.add(item.id);
            });
            return next;
        });
    }

    return (
        <div className="rounded-lg border border-gray-200 bg-white shadow-card">
            <div className="flex flex-wrap items-center justify-between gap-3 border-b border-gray-200 px-6 py-4">
                <h1 className="flex items-baseline gap-2 text-xl font-bold text-navy">
                    {title} <span className="text-base font-normal text-gray-400">{items.length}</span>
                </h1>
                <Button onClick={onAdd} className="!w-auto px-4">{addLabel}</Button>
            </div>

            <div className="flex flex-wrap items-center justify-between gap-3 px-6 py-4">
                <label className="flex w-full items-center gap-2 rounded-full border border-gray-300 px-4 py-2 text-sm text-gray-500 focus-within:border-navy sm:w-auto">
                    <Search size={16} aria-hidden="true" className="shrink-0" />
                    <input
                        type="search"
                        value={search}
                        onChange={(event) => {
                            setSearch(event.target.value);
                            setPage(1);
                        }}
                        aria-label={"Search " + title.toLowerCase() + " by name"}
                        placeholder="Search by name"
                        className="w-full bg-transparent focus:outline-none sm:w-56"
                    />
                </label>
                <div className="flex overflow-hidden rounded-full border border-gray-300 text-sm font-medium">
                    <span className="bg-coral px-4 py-2 text-white">Table</span>
                    <button type="button" disabled title="Grid view isn't available yet" className="cursor-not-allowed px-4 py-2 text-gray-300">
                        Grid
                    </button>
                </div>
            </div>

            {children && <div className="px-6">{children}</div>}

            <div className="overflow-x-auto">
                <table className="w-full min-w-[560px] text-left text-sm">
                    <caption className="sr-only">{title} and employee counts</caption>
                    <thead>
                        <tr className="border-y border-gray-200 text-gray-500">
                            <th scope="col" className="w-10 py-3 pl-6 pr-4">
                                <Checkbox checked={allSelected} onChange={toggleAll} aria-label={"Select all visible " + title.toLowerCase()} />
                            </th>
                            <th scope="col" aria-sort={nameSort === "Ascending" ? "ascending" : nameSort === "Descending" ? "descending" : "none"} className="py-3 pr-4 font-medium">
                                <NameSortButton
                                    direction={nameSort}
                                    onClick={() => {
                                        setNameSort((previous) => previous === "Ascending" ? "Descending" : "Ascending");
                                        setPage(1);
                                    }}
                                />
                            </th>
                            <th scope="col" className="w-1/3 py-3 pr-4 font-medium">No. of employees</th>
                            <th scope="col" className="w-40 py-3 pr-6 font-medium"><span className="sr-only">Actions</span></th>
                        </tr>
                    </thead>
                    <tbody>
                        {visibleItems.map((item) => (
                            <tr key={item.id} className="group border-b border-gray-100 last:border-0 hover:bg-gray-50">
                                <td className="py-5 pl-6 pr-4">
                                    <Checkbox checked={selected.has(item.id)} onChange={() => toggleRow(item.id)} aria-label={"Select " + item.name} />
                                </td>
                                <td className="py-5 pr-4 font-semibold text-navy">{item.name}</td>
                                <td className="py-5 pr-4 text-gray-700">{employeeCount(item)}</td>
                                <td className="py-5 pr-6">
                                    <div className="flex justify-end gap-3 md:opacity-0 md:group-hover:opacity-100 md:group-focus-within:opacity-100">
                                        <button type="button" onClick={() => onEdit(item)} aria-label={"Edit " + item.name} className="text-xs font-medium text-navy hover:underline focus-visible:underline">
                                            Edit
                                        </button>
                                        <button type="button" onClick={() => onDelete(item)} aria-label={"Remove " + item.name} className="text-xs font-medium text-red-600 hover:underline focus-visible:underline">
                                            Remove
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                        {visibleItems.length === 0 && (
                            <tr>
                                <td colSpan={4} className="px-6 py-8 text-center text-gray-500">
                                    {query ? "No " + title.toLowerCase() + " match your search." : emptyMessage}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <div className="flex flex-wrap items-center justify-between gap-3 px-6 py-4 text-sm text-gray-500">
                <span aria-live="polite">
                    {filtered.length === 0 ? "0 of 0" : `${offset + 1}-${Math.min(offset + PAGE_SIZE, filtered.length)} of ${filtered.length}`}
                </span>
                <div className="flex gap-2">
                    <Button variant="secondary" className="!w-auto px-4" disabled={currentPage <= 1} onClick={() => setPage(currentPage - 1)}>Previous</Button>
                    <Button variant="secondary" className="!w-auto px-4" disabled={offset + PAGE_SIZE >= filtered.length} onClick={() => setPage(currentPage + 1)}>Next</Button>
                </div>
            </div>
        </div>
    );
}
