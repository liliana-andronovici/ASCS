import type { ReactNode } from "react";

export type BadgeVariant = "active" | "onLeave" | "starting" | "inactive" | "neutral";

type BadgeProps = {
    variant?: BadgeVariant;
    children: ReactNode;
};

const VARIANT_STYLES: Record<BadgeVariant, string> = {
    active: "bg-[#fbe3e1] text-coral",
    onLeave: "border border-coral bg-white text-coral",
    starting: "bg-[#e7ebf0] text-navy",
    inactive: "bg-gray-100 text-gray-500",
    neutral: "bg-gray-100 text-gray-600",
};

export function Badge({ variant = "neutral", children }: BadgeProps) {
    return (
        <span
            className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold whitespace-nowrap ${VARIANT_STYLES[variant]}`}
        >
            {children}
        </span>
    );
}