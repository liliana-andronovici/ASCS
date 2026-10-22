import type { ButtonHTMLAttributes } from "react";

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
    variant?: "primary" | "secondary";
    fullWidth?: boolean;
};

export function Button({ variant = "primary", fullWidth = true, className = "", ...props }: ButtonProps) {
    const base =
        "rounded-md px-4 py-2.5 text-sm font-medium transition-opacity hover:opacity-90 disabled:opacity-50 disabled:cursor-not-allowed";

    const variants = {
        primary: "bg-coral text-white",
        secondary: "border border-gray-300 bg-white text-navy",
    };

    return <button className={`${base} ${fullWidth ? "w-full" : "w-auto shrink-0"} ${variants[variant]} ${className}`} {...props} />;
}
