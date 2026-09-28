import type { SelectHTMLAttributes } from "react";

type SelectProps = SelectHTMLAttributes<HTMLSelectElement> & {
    label: string;
    options: { value: string; label: string }[];
};

export function Select({ label, id, options, className = "", ...props }: SelectProps) {
    return (
        <div className="flex flex-col gap-1">
            <label htmlFor={id} className="text-sm text-gray-700">
                {label}
            </label>
            <select
                id={id}
                className={`rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none ${className}`}
                {...props}
            >
                {options.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                        {opt.label}
                    </option>
                ))}
            </select>
        </div>
    );
}