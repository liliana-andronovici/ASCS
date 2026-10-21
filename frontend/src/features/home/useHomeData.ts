import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getHomeEmployees, getHomeOutToday, HOME_LEAVE_PATH } from "./api";
import { buildUpcoming, buildWeek, employeeStats } from "./calculations";

export function useHomeData() {
    const [now, setNow] = useState(() => new Date());
    useEffect(() => {
        const timer = window.setInterval(() => setNow(new Date()), 30_000);
        return () => window.clearInterval(timer);
    }, []);
    const directory = useQuery({ queryKey: ["employees", "home-directory"], queryFn: ({ signal }) => getHomeEmployees(signal), retry: false, staleTime: 60_000 });
    const date = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-${String(now.getDate()).padStart(2, "0")}`;
    const leave = useQuery({
        queryKey: ["home-leave", HOME_LEAVE_PATH, date],
        queryFn: ({ signal }) => getHomeOutToday(date, signal),
        retry: false, staleTime: 60_000,
    });
    const employees = directory.data ?? [];
    const week = buildWeek([], employees, now);
    return {
        isLoading: directory.isPending, isError: directory.isError, directoryReady: directory.isSuccess,
        retryEmployees: () => directory.refetch(), isRefreshingEmployees: directory.isFetching,
        ...employeeStats(employees, now), week, now,
        upcoming: directory.isSuccess ? buildUpcoming(employees, now) : [],
        leaveConfigured: true, leaveAvailable: leave.isSuccess,
        leaveLoading: leave.isPending,
        leaveError: leave.isError, retryLeave: () => leave.refetch(),
        outToday: leave.data?.count ?? 0,
    };
}
