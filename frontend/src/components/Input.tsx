import type { InputHTMLAttributes } from "react";

type InputProps = InputHTMLAttributes<HTMLInputElement> & {
    label: string;
};

export function Input({ label, id, className = "", ...props }: InputProps) {
    return (
        <div className="flex flex-col gap-1">
            <label htmlFor={id} className="text-sm text-gray-700">
                {label}
            </label>
            <input
                id={id}
                className={`rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-navy focus:outline-none ${className}`}
                {...props}
            />
        </div>
    );
}