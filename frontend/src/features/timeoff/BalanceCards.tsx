import type { TimeOffBalance } from "./types";

type BalanceCardsProps = { balances: TimeOffBalance[] };

function toPercent(value: number, total: number | null): number {
    if (!total) return 0;
    return Math.min(100, (value / total) * 100);
}

export function BalanceCards({ balances }: BalanceCardsProps) {
    return (
        <div className="grid grid-cols-1 border-b border-gray-200 md:grid-cols-3">
            {balances.map((balance) => (
                <div
                    key={balance.type}
                    className="border-gray-200 px-6 py-5 md:border-r md:last:border-r-0"
                >
                    <p className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                        {balance.label}
                    </p>
                    <p className="mt-1 text-3xl font-bold text-navy">
                        {balance.headline.value}
                        <span className="ml-1 text-sm font-normal text-gray-500">
                            {balance.headline.label}
                        </span>
                    </p>
                    <div className="mt-3 flex h-1.5 overflow-hidden rounded-full bg-gray-200">
                        <span
                            className="bg-navy"
                            style={{ width: `${toPercent(balance.used, balance.total)}%` }}
                        />
                        <span
                            className="bg-coral"
                            style={{ width: `${toPercent(balance.pending, balance.total)}%` }}
                        />
                    </div>
                    <p className="mt-2 text-xs text-gray-500">{balance.caption}</p>
                </div>
            ))}
        </div>
    );
}