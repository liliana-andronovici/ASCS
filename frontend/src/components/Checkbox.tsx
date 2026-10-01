import type { InputHTMLAttributes } from "react";

type CheckboxProps = InputHTMLAttributes<HTMLInputElement>;

export function Checkbox({ className = "", ...props }: CheckboxProps) {
    return (
        <input
            type="checkbox"
            className={`h-4 w-4 rounded border-gray-300 accent-gray-400 focus:outline-none focus:ring-1 focus:ring-gray-300 ${className}`}
            {...props}
        />
    );
}