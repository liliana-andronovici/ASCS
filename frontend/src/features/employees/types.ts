export type EmployeeStatus = "Active" | "Inactive";

export interface Employee {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    title: string;
    startedOnUtc: string;
    departmentId: number | null;
    locationId: number | null;
    managerId: number | null;
    status: EmployeeStatus;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface EmployeeQuery {
    search?: string;
    departmentId?: number;
    locationId?: number;
    status?: EmployeeStatus;
    sortBy?: string;
    sortDirection?: "Ascending" | "Descending";
    page?: number;
    pageSize?: number;
}