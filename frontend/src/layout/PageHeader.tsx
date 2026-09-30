type PageHeaderProps = { title: string; subtitle?: string };

export function PageHeader({ title, subtitle }: PageHeaderProps) {
    return (
        <header className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
            <div className="flex items-baseline gap-3">
                <h1 className="text-lg text-[24px] font-semibold text-navy">{title}</h1>
                {subtitle && <span className="text-sm text-gray-400">{subtitle}</span>}
            </div>
        </header>
    );
}