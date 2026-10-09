import { Link } from "react-router-dom";
import { useCurrentEmployee } from "../features/account/useCurrentEmployee";
import { useHomeData } from "../features/home/useHomeData";
import { buildMeetingsWeek, useMeetingsThisWeek } from "../features/home/useMeetings";

function getGreeting(hour: number): string {
    if (hour < 12) return "Good morning";
    if (hour < 18) return "Good afternoon";
    return "Good evening";
}

function formatTime(iso: string): string {
    return new Date(iso).toLocaleTimeString("en-US", { hour: "2-digit", minute: "2-digit", hour12: false });
}

function Stat({ label, value, hint }: { label: string; value: string | number; hint: string }) {
    return (
        <div className="border-r border-gray-100 px-6 py-5 last:border-r-0">
            <p className="text-xs font-semibold uppercase tracking-wider text-gray-400">{label}</p>
            <p className="mt-1 text-3xl font-bold text-navy">{value}</p>
            <p className="text-xs text-gray-500">{hint}</p>
        </div>
    );
}

export default function HomePage() {
    const { employee } = useCurrentEmployee();
    const home = useHomeData();
    const meetingsQuery = useMeetingsThisWeek();
    const meetingDays = buildMeetingsWeek(meetingsQuery.data ?? [], home.now);

    const dateLabel = home.now.toLocaleDateString("en-US", {
        weekday: "long",
        day: "numeric",
        month: "long",
    });
    const weekRange = `${home.week[0].date.toLocaleDateString("en-US", { day: "numeric", month: "short" })} – ${home.week[4].date.toLocaleDateString("en-US", { day: "numeric", month: "short" })}`;
    const show = (v: number) => (home.isLoading ? "..." : v);

    return (
        <div className="flex h-full flex-col">
            <header className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
                <div className="flex items-baseline gap-3">
                    <h1 className="text-[24px] font-semibold text-navy">
                        {getGreeting(home.now.getHours())}, {employee?.firstName ?? "there"}
                    </h1>
                    <span className="text-sm text-gray-400">{dateLabel}</span>
                </div>
                <Link
                    to="/people"
                    className="rounded-md bg-coral px-4 py-2 text-sm font-semibold text-white hover:opacity-90"
                >
                    Add person
                </Link>
            </header>

            <div className="grid grid-cols-1 border-b border-gray-200 sm:grid-cols-3">
                <Stat
                    label="Headcount"
                    value={show(home.headcount)}
                    hint={home.isLoading ? " " : `+${home.joinedThisMonth} this month`}
                />
                <Stat
                    label="Out today"
                    value={home.leaveAvailable ? home.outToday : "–"}
                    hint={home.leaveAvailable ? "Approved time off" : "Time off data not connected yet"}
                />
                <Stat
                    label="Starting this month"
                    value={show(home.startingThisMonth)}
                    hint="New employees in the current month"
                />
            </div>

            {home.isError && (
                <p className="px-6 pt-4 text-sm text-red-600">Couldn't load employees from the server.</p>
            )}

            <div className="grid flex-1 grid-cols-1 lg:grid-cols-3">
                <section className="border-gray-100 px-6 py-5 lg:col-span-2 lg:border-r">
                    <div className="mb-3 flex items-baseline justify-between">
                        <h2 className="text-sm font-semibold text-navy">Meetings this week</h2>
                        <span className="text-xs text-gray-400">{weekRange}</span>
                    </div>
                    <div className="grid grid-cols-5 gap-2">
                        {meetingDays.map((d) => (
                            <div
                                key={d.date.toISOString()}
                                className={`flex min-h-[150px] flex-col gap-1.5 rounded-lg border p-2 ${
                                    d.isToday ? "border-coral bg-[#fdf3f2]" : "border-gray-100 bg-white"
                                }`}
                            >
                                <span className="text-[10px] uppercase tracking-wider text-gray-400">
                                    {d.label} <strong className="text-navy">{d.date.getDate()}</strong>
                                </span>
                                {d.meetings.map((m) => (
                                    <span
                                        key={m.id}
                                        className={`border-l-2 border-coral bg-gray-50 px-1.5 py-1 text-[11px] leading-tight text-navy ${
                                            new Date(m.endUtc) < home.now ? "opacity-50" : ""
                                        }`}
                                    >
                                        <span className="block font-semibold">{formatTime(m.startUtc)}</span>
                                        {m.title}
                                        {m.location && <span className="block text-[10px] text-gray-400">{m.location}</span>}
                                    </span>
                                ))}
                                {!meetingsQuery.isLoading && !meetingsQuery.isError && d.meetings.length === 0 && (
                                    <span className="text-[11px] text-gray-300">No meetings</span>
                                )}
                            </div>
                        ))}
                    </div>
                    {meetingsQuery.isError && (
                        <p className="mt-3 text-xs text-gray-400">
                            Meetings aren't available from the server yet.
                        </p>
                    )}
                </section>

                <section className="px-6 py-5">
                    <h2 className="mb-3 text-sm font-semibold text-navy">Coming up</h2>
                    {home.upcoming.length === 0 && !home.isLoading && (
                        <p className="text-sm text-gray-500">Nothing scheduled in the next 45 days.</p>
                    )}
                    <ul>
                        {home.upcoming.map((u) => (
                            <li
                                key={`${u.date.toISOString()}-${u.who}`}
                                className="flex gap-3 border-b border-gray-100 py-2.5 text-sm"
                            >
                                <span className="w-14 flex-none text-xs font-extrabold text-gray-400">
                                    {u.date.toLocaleDateString("en-US", { day: "numeric", month: "short" })}
                                </span>
                                <span className="flex flex-col">
                                    <span className="text-navy">{u.what}</span>
                                    <span className="text-xs text-gray-500">{u.who}</span>
                                </span>
                            </li>
                        ))}
                    </ul>
                </section>
            </div>
        </div>
    );
}
