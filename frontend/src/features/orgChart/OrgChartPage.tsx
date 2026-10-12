import { useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { Minus, Plus } from "lucide-react";
import { Button } from "../../components/Button";
import { ApiError } from "../../lib/api";
import { EmployeeCard, OrgChartBranch } from "./OrgChartBranch";
import { useOrgChart, type OrgChartView } from "./useOrgChart";
import type { OrgChartEmployee } from "./types";
import "./orgChart.css";

const VIEWS: { value: OrgChartView; label: string }[] = [
    { value: "reporting", label: "Reporting line" },
    { value: "departments", label: "Departments" },
    { value: "locations", label: "Locations" },
];

export default function OrgChartPage() {
    const [view, setView] = useState<OrgChartView>("reporting");
    const [zoom, setZoom] = useState(100);
    const canvasRef = useRef<HTMLElement>(null);
    const contentRef = useRef<HTMLDivElement>(null);
    const manualZoomRef = useRef(false);
    const dragRef = useRef<{ id: number; x: number; y: number; left: number; top: number; moved: boolean } | null>(null);
    const suppressClickRef = useRef(false);
    const [isDragging, setIsDragging] = useState(false);

    useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        function handleWheel(event: WheelEvent) {
            // Touchpad pinch is delivered as a Ctrl+wheel event.
            // Ordinary two-finger scrolling keeps the native canvas scrolling.
            if (!event.ctrlKey) return;
            event.preventDefault();
            manualZoomRef.current = true;
            const delta = event.deltaY * (event.deltaMode === 1 ? 16 : event.deltaMode === 2 ? canvas!.clientHeight : 1);
            setZoom((value) => Math.max(1, Math.min(150, value * Math.exp(-delta * 0.01))));
        }

        canvas.addEventListener("wheel", handleWheel, { passive: false });
        return () => canvas.removeEventListener("wheel", handleWheel);
    }, []);
    const { hierarchy, employees, departments, locations } = useOrgChart(view);
    const directory = view === "departments" ? departments : locations;
    const isLoading = view === "reporting" ? hierarchy.isPending : employees.isPending || directory.isPending;
    const error = view === "reporting" ? hierarchy.error : employees.error ?? directory.error;

    const groups = useMemo(() => {
        if (view === "reporting") return [];
        const entries = new Map<number | null, { id: number | null; name: string; employees: OrgChartEmployee[] }>();
        const items = view === "departments" ? departments.data : locations.data;
        items?.forEach((item) => entries.set(item.id, { id: item.id, name: item.name, employees: [] }));
        employees.data?.forEach((employee) => {
            const id = view === "departments" ? employee.departmentId : employee.locationId;
            if (!entries.has(id)) {
                const name = id === null
                    ? (view === "departments" ? "No department" : "No location")
                    : (view === "departments" ? "Department #" : "Location #") + id;
                entries.set(id, { id, name, employees: [] });
            }
            entries.get(id)!.employees.push({ ...employee, directReports: [] });
        });
        return [...entries.values()];
    }, [view, departments.data, locations.data, employees.data]);

    useEffect(() => {
        const canvas = canvasRef.current;
        const content = contentRef.current;
        if (!canvas || !content || isLoading || error) return;

        let frame = 0;
        function fitChart() {
            cancelAnimationFrame(frame);
            frame = requestAnimationFrame(() => {
                if (manualZoomRef.current) return;
                const style = getComputedStyle(canvas!);
                const width = canvas!.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight);
                const height = canvas!.clientHeight - parseFloat(style.paddingTop) - parseFloat(style.paddingBottom);
                if (width <= 0 || height <= 0 || !content!.offsetWidth || !content!.offsetHeight) return;
                const scale = Math.min(1, width / content!.offsetWidth, height / content!.offsetHeight);
                setZoom(Math.max(1, Math.floor(scale * 100)));
                canvas!.scrollTo(0, 0);
            });
        }

        fitChart();
        const observer = new ResizeObserver(fitChart);
        observer.observe(canvas);
        observer.observe(content);
        return () => {
            cancelAnimationFrame(frame);
            observer.disconnect();
        };
    }, [view, isLoading, error, hierarchy.data, groups]);

    function retry() {
        if (view === "reporting") {
            void hierarchy.refetch();
        } else {
            void employees.refetch();
            void directory.refetch();
        }
    }

    return (
        <div className="flex h-full min-h-0 flex-col bg-white">
            <header className="flex flex-wrap items-center justify-between gap-4 border-b border-gray-200 px-6 py-4">
                <h1 className="text-2xl font-semibold text-navy">Org chart</h1>
                <div className="flex flex-wrap items-center gap-3">
                    <div role="group" aria-label="Org chart view" className="flex overflow-hidden rounded-md border border-gray-200">
                        {VIEWS.map((item) => (
                            <button
                                key={item.value}
                                type="button"
                                aria-pressed={view === item.value}
                                onClick={() => { manualZoomRef.current = false; setView(item.value); }}
                                className={"cursor-pointer border-r border-gray-200 px-4 py-3 text-sm last:border-r-0 transition-colors " +
                                    (view === item.value ? "bg-gray-100 font-semibold text-navy" : "text-gray-600 hover:bg-gray-50")}
                            >
                                {item.label}
                            </button>
                        ))}
                    </div>
                    <Button variant="secondary" className="!w-auto !p-3 cursor-pointer" aria-label="Zoom in" disabled={zoom >= 150} onClick={() => { manualZoomRef.current = true; setZoom((value) => Math.min(150, value + 10)); }}>
                        <Plus size={20} />
                    </Button>
                    <Button variant="secondary" className="!w-auto !p-3 cursor-pointer" aria-label="Zoom out" disabled={zoom <= 1} onClick={() => { manualZoomRef.current = true; setZoom((value) => Math.max(1, value - 10)); }}>
                        <Minus size={20} />
                    </Button>
                    <span className="sr-only" role="status">Zoom {Math.round(zoom)}%</span>
                </div>
            </header>
            <section
                ref={canvasRef}
                aria-label="Organization chart"
                tabIndex={0}
                className={"org-chart-canvas min-h-0 flex-1 overflow-auto overscroll-contain p-8 focus-visible:outline-coral " + (isDragging ? "cursor-grabbing select-none" : "cursor-grab")}
                onPointerDown={(event) => {
                    if (event.pointerType !== "mouse" || event.button !== 0) return;
                    if ((event.target as HTMLElement).closest("a, input, select, [role='alert']")) return;
                    suppressClickRef.current = false;
                    dragRef.current = {
                        id: event.pointerId,
                        x: event.clientX,
                        y: event.clientY,
                        left: event.currentTarget.scrollLeft,
                        top: event.currentTarget.scrollTop,
                        moved: false,
                    };
                }}
                onPointerMove={(event) => {
                    const drag = dragRef.current;
                    if (!drag || drag.id !== event.pointerId) return;
                    if (event.buttons !== 1) {
                        dragRef.current = null;
                        setIsDragging(false);
                        return;
                    }
                    const dx = event.clientX - drag.x;
                    const dy = event.clientY - drag.y;
                    if (!drag.moved && Math.hypot(dx, dy) < 5) return;
                    drag.moved = true;
                    suppressClickRef.current = true;
                    event.currentTarget.setPointerCapture(event.pointerId);
                    setIsDragging(true);
                    event.preventDefault();
                    event.currentTarget.scrollLeft = drag.left - dx;
                    event.currentTarget.scrollTop = drag.top - dy;
                }}
                onPointerUp={(event) => {
                    dragRef.current = null;
                    setIsDragging(false);
                    if (event.currentTarget.hasPointerCapture(event.pointerId)) {
                        event.currentTarget.releasePointerCapture(event.pointerId);
                    }
                }}
                onPointerCancel={() => {
                    dragRef.current = null;
                    setIsDragging(false);
                }}
                onLostPointerCapture={() => {
                    dragRef.current = null;
                    setIsDragging(false);
                }}
                onClickCapture={(event) => {
                    if (suppressClickRef.current && event.detail !== 0) {
                        event.preventDefault();
                        event.stopPropagation();
                        suppressClickRef.current = false;
                    }
                }}
            >
                {error ? (
                    <div role="alert" className="mx-auto max-w-lg rounded-lg border border-gray-200 bg-white p-6 shadow-card">
                        <p className="text-sm text-red-600">
                            {error instanceof ApiError && error.status === 401
                                ? "Sign in to view the organization chart."
                                : error instanceof ApiError && error.status === 403
                                    ? "You don't have permission to view this chart."
                                    : "Couldn't load the organization chart. Please try again."}
                        </p>
                        <div className="mt-4 flex items-center gap-4">
                            <Button variant="secondary" className="!w-auto" onClick={retry}>Try again</Button>
                            {error instanceof ApiError && error.status === 401 && <Link to="/sign-in" className="text-sm font-semibold text-coral hover:underline">Sign in</Link>}
                        </div>
                    </div>
                ) : isLoading ? (
                    <p role="status" className="text-sm text-gray-500">Loading organization chart...</p>
                ) : view === "reporting" ? (
                    hierarchy.data?.length ? (
                        <div ref={contentRef} style={{ zoom: zoom / 100 }} className="mx-auto w-max">
                            <ul className="org-chart-tree">
                                {hierarchy.data.map((employee) => <OrgChartBranch key={employee.id} employee={employee} root />)}
                            </ul>
                        </div>
                    ) : <p className="text-sm text-gray-500">No reporting structure available.</p>
                ) : groups.length ? (
                    <div ref={contentRef} style={{ zoom: zoom / 100 }} className="mx-auto flex w-max items-start justify-center gap-6">
                        {groups.map((group) => (
                            <section key={group.id ?? "unassigned"} className="w-72 shrink-0">
                                <h2 className="mb-6 rounded-lg bg-navy px-4 py-5 font-semibold text-white">
                                    {group.name}
                                    <span className="mt-1 block text-xs font-normal text-white/70">
                                        {group.employees.length} {group.employees.length === 1 ? "employee" : "employees"}
                                    </span>
                                </h2>
                                <ul className="flex flex-col gap-2">
                                    {group.employees.map((employee) => <li key={employee.id}><EmployeeCard employee={employee} /></li>)}
                                </ul>
                                {group.employees.length === 0 && <p className="rounded-md bg-white p-4 text-sm text-gray-500">No employees assigned.</p>}
                            </section>
                        ))}
                    </div>
                ) : <p className="text-sm text-gray-500">No groups available.</p>}
            </section>
        </div>
    );
}
