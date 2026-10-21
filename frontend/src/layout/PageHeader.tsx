import type { ReactNode } from "react";

type PageHeaderProps = { title: string; children?: ReactNode };

export function PageHeader({ title, children }: PageHeaderProps) {
    return (
        <header className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
            <h1 className="text-lg text-[24px] font-semibold text-navy">{title}</h1>
            {children}
        </header>
    );
}