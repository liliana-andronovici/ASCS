import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getHomeEmployees, getHomeLeave, HOME_LEAVE_PATH } from "./api";
import { absentEmployees, buildUpcoming, buildWeek, employeeStats } from "./calculations";
import { weekRange } from "./calendar";

export function useHomeData() {
    const [now, setNow] = useState(() => new Date());
    useEffect(() => {
        const timer = window.setInterval(() => setNow(new Date()), 30_000);
        return () => window.clearInterval(timer);
    }, []);
    const directory = useQuery({ queryKey: ["employees", "home-directory"], queryFn: ({ signal }) => getHomeEmployees(signal), retry: false, staleTime: 60_000 });
    const { from, to } = weekRange(now, true);
    const leave = useQuery({
        queryKey: ["home-leave", HOME_LEAVE_PATH, from, to],
        queryFn: ({ signal }) => getHomeLeave(from, to, signal),
        retry: false, staleTime: 60_000,
    });
    const employees = directory.data ?? [];
    const week = buildWeek(leave.isSuccess ? leave.data : [], employees, now);
    return {
        isLoading: directory.isPending, isError: directory.isError, directoryReady: directory.isSuccess,
        retryEmployees: () => directory.refetch(), isRefreshingEmployees: directory.isFetching,
        ...employeeStats(employees, now), week, now,
        upcoming: directory.isSuccess ? buildUpcoming(employees, now) : [],
        leaveConfigured: true, leaveAvailable: leave.isSuccess,
        leaveLoading: leave.isPending,
        leaveError: leave.isError, retryLeave: () => leave.refetch(),
        outToday: absentEmployees(leave.isSuccess ? leave.data : [], employees, now).length,
    };
}
