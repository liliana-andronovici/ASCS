import { Link } from "react-router-dom";
import { useCurrentEmployee } from "../features/home/useCurrentEmployee";
import { useHomeData } from "../features/home/useHomeData";
import { buildMeetingsWeek, useMeetingsThisWeek, useUpcomingMeetings } from "../features/home/useMeetings";
import { useUpcomingOnboardingTasks } from "../features/onboarding/useOnboarding";

function getGreeting(hour: number): string {
    if (hour < 12) return "Good morning";
    if (hour < 18) return "Good afternoon";
    return "Good evening";
}

function formatTime(iso: string): string {
    return new Date(iso).toLocaleTimeString("en-US", { hour: "2-digit", minute: "2-digit", hour12: false });
}

function Stat({ label, value, hint }: { label: string; value: string | number; hint: string }) {
    return <div className="border-r border-gray-100 px-6 py-5 last:border-r-0">
        <p className="text-xs font-semibold uppercase tracking-wider text-gray-400">{label}</p>
        <p className="mt-1 text-3xl font-bold text-navy">{value}</p>
        <p className="text-xs text-gray-500">{hint}</p>
    </div>;
}

export default function HomePage() {
    const { id: employeeId, employee } = useCurrentEmployee();
    const home = useHomeData();
    const meetings = useMeetingsThisWeek(home.now, employeeId);
    const upcomingMeetings = useUpcomingMeetings(home.now, employeeId);
    const upcomingTasks = useUpcomingOnboardingTasks(employeeId);
    const meetingDays = buildMeetingsWeek(meetings.isSuccess ? meetings.data : [], home.now);
    const upcoming: { id: string; date: Date; what: string; who: string; href?: string }[] = [
        ...home.upcoming,
        ...(upcomingMeetings.data ?? [])
            .filter((meeting) => Date.parse(meeting.startUtc) >= home.now.getTime())
            .map((meeting) => ({
                id: `meeting-${meeting.id}`,
                date: new Date(meeting.startUtc),
                what: meeting.title,
                who: meeting.location ?? "Meeting",
            })),
        ...(upcomingTasks.data ?? []).filter((task) => task.employeeId === employeeId).map((task) => ({
            id: `task-${task.id}`,
            date: new Date(`${task.dueDate}T12:00:00`),
            what: task.title,
            who: "Your onboarding task",
            href: `/onboarding/${employeeId}`,
        })),
    ].sort((a, b) => a.date.getTime() - b.date.getTime());
    const dateLabel = home.now.toLocaleDateString("en-US", { weekday: "long", day: "numeric", month: "long" });
    const weekRange = `${home.week[0].date.toLocaleDateString("en-US", { day: "numeric", month: "short" })} – ${home.week[4].date.toLocaleDateString("en-US", { day: "numeric", month: "short" })}`;
    const show = (value: number) => home.isLoading ? "..." : home.directoryReady ? value : "–";
    const employeeHint = home.isLoading ? "Loading employees..." : home.isError ? "Employee data unavailable" : `+${home.joinedThisMonth} this month`;
    const leaveHint = !home.leaveConfigured ? "Time off is not available in the current backend"
        : home.leaveLoading ? "Loading time off..." : home.leaveError ? "Unable to load time off" : "Approved time off";

    return (
        <div className="flex h-full flex-col">
            <header className="flex flex-wrap items-center justify-between gap-3 border-b border-gray-200 px-6 py-4">
                <div className="flex flex-wrap items-baseline gap-3">
                    <h1 className="text-[24px] font-semibold text-navy">{getGreeting(home.now.getHours())}, {employee?.firstName ?? "there"}</h1>
                    <span className="text-sm text-gray-400">{dateLabel}</span>
                </div>
                <Link to="/people" className="rounded-md bg-coral px-4 py-2 text-sm font-semibold text-white hover:opacity-90">View people</Link>
            </header>
            <div className="grid grid-cols-1 border-b border-gray-200 sm:grid-cols-3">
                <Stat label="Headcount" value={show(home.headcount)} hint={employeeHint} />
                <Stat label="Out today" value={home.leaveLoading ? "..." : home.leaveAvailable ? home.outToday : "–"} hint={leaveHint} />
                <Stat label="Starting this month" value={show(home.startingThisMonth)} hint={home.isError ? "Employee data unavailable" : "New employees in the current month"} />
            </div>
            {home.isError && <p role="alert" className="px-6 pt-4 text-sm text-red-600">
                Couldn't load the complete employee directory. <button onClick={home.retryEmployees} disabled={home.isRefreshingEmployees} className="underline">{home.isRefreshingEmployees ? "Retrying..." : "Retry"}</button>
            </p>}
            {home.leaveError && <p role="alert" className="px-6 pt-4 text-sm text-red-600">Couldn't load time off. <button onClick={home.retryLeave} className="underline">Retry time off</button></p>}
            <div className="grid flex-1 grid-cols-1 lg:grid-cols-3">
                <section className="min-w-0 border-gray-100 px-6 py-5 lg:col-span-2 lg:border-r">
                    <div className="mb-3 flex flex-wrap items-baseline justify-between gap-2">
                        <h2 className="text-sm font-semibold text-navy">Your meetings this week</h2>
                        <span className="text-xs text-gray-400">{weekRange}</span>
                    </div>
                    <div className="overflow-x-auto">
                        <div className="grid min-w-[440px] grid-cols-5 gap-2">
                            {meetingDays.map((day) => <div key={day.date.toISOString()} className={`flex min-h-[150px] min-w-0 flex-col gap-1.5 rounded-lg border p-2 ${day.isToday ? "border-coral bg-[#fdf3f2]" : "border-gray-100 bg-white"}`}>
                                <span className="text-[10px] uppercase tracking-wider text-gray-400">{day.label} <strong className="text-navy">{day.date.getDate()}</strong></span>
                                {day.meetings.map((meeting) => <span key={meeting.id} className={`break-words border-l-2 border-coral bg-gray-50 px-1.5 py-1 text-[11px] leading-tight text-navy ${Date.parse(meeting.endUtc) < home.now.getTime() ? "opacity-50" : ""}`}>
                                    <span className="block font-semibold">{Date.parse(meeting.startUtc) < day.date.getTime() ? "Continues" : formatTime(meeting.startUtc)}</span>
                                    {meeting.title}
                                    {meeting.location && <span className="block text-[10px] text-gray-400">{meeting.location}</span>}
                                </span>)}
                                {meetings.isSuccess && day.meetings.length === 0 && <span className="text-[11px] text-gray-400">No meetings</span>}
                            </div>)}
                        </div>
                    </div>
                    {!meetings.configured && <p className="mt-3 text-xs text-gray-500">Meetings are not available in the current backend.</p>}
                    {meetings.needsSignIn && <p className="mt-3 text-sm text-gray-500">Sign in to see your meetings.</p>}
                    {meetings.configured && !meetings.needsSignIn && meetings.isPending && <p role="status" className="mt-3 text-sm text-gray-500">Loading meetings...</p>}
                    {meetings.isError && <p role="alert" className="mt-3 text-sm text-red-600">Couldn't load meetings. <button onClick={() => meetings.refetch()} disabled={meetings.isFetching} className="underline">Retry meetings</button></p>}
                </section>
                <section className="px-6 py-5">
                    <h2 className="mb-3 text-sm font-semibold text-navy">Coming up</h2>
                    {(home.isLoading || (!upcomingMeetings.needsSignIn && (upcomingMeetings.isPending || upcomingTasks.isPending))) && <p role="status" className="text-sm text-gray-500">Loading upcoming events...</p>}
                    {home.isError && <p className="text-sm text-gray-500">Employee milestones are unavailable until employee data loads.</p>}
                    {upcomingMeetings.needsSignIn && <p className="text-sm text-gray-500">Sign in to see your upcoming meetings.</p>}
                    {upcomingMeetings.isError && <p role="alert" className="text-sm text-red-600">Could not load upcoming meetings. <button onClick={() => upcomingMeetings.refetch()} className="underline">Retry</button></p>}
                    {upcomingTasks.isError && <p role="alert" className="text-sm text-red-600">Could not load upcoming tasks. <button onClick={() => upcomingTasks.refetch()} className="underline">Retry</button></p>}
                    {home.directoryReady && upcomingMeetings.isSuccess && upcomingTasks.isSuccess && upcoming.length === 0 && <p className="text-sm text-gray-500">Nothing scheduled in the next 45 days.</p>}
                    <ul className="max-h-[520px] overflow-y-auto">{upcoming.map((item) => <li key={item.id} className="flex gap-3 border-b border-gray-100 py-2.5 text-sm">
                        <span className="w-14 flex-none text-xs font-extrabold text-gray-400">{item.date.toLocaleDateString("en-US", { day: "numeric", month: "short" })}</span>
                        <span className="flex min-w-0 flex-col"><span className="text-navy">{item.href ? <Link to={item.href}>{item.what}</Link> : item.what}</span><span className="break-words text-xs text-gray-500">{item.who}</span></span>
                    </li>)}</ul>
                </section>
            </div>
        </div>
    );
}
