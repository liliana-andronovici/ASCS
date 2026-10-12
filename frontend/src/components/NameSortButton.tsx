import { ArrowUpDown } from "lucide-react";

export type NameSortDirection = "Ascending" | "Descending" | undefined;

type Props = {
    direction: NameSortDirection;
    onClick: () => void;
};

export function NameSortButton({ direction, onClick }: Props) {
    const label = direction === "Ascending" ? "Sort by name Z to A" : "Sort by name A to Z";

    return (
        <span className="inline-flex items-center gap-2">
            Name
            <button
                type="button"
                onClick={onClick}
                aria-label={label}
                title={label}
                className={`inline-flex h-7 w-7 items-center justify-center rounded-sm border-0 bg-transparent shadow-none hover:text-navy focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-coral ${direction ? "text-coral" : "text-gray-400"}`}
            >
                <ArrowUpDown size={14} aria-hidden="true" />
            </button>
        </span>
    );
}
