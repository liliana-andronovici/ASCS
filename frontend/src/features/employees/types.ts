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

export interface EmployeeEquipment {
    id: number;
    equipmentId: number;
    name: string;
    code: string;
    typeName: string;
    assignedOnUtc: string;
    returnedOnUtc: string | null;
}

export interface EmployeeSummary {
    id: number;
    firstName: string;
    lastName: string;
    title: string;
}

export interface EmployeeProfile extends Employee {
    departmentName: string | null;
    locationName: string | null;
    manager: EmployeeSummary | null;
    phone: string | null;
    desk: string | null;
    birthdayMonth: number | null;
    birthdayDay: number | null;
    languages: string[];
    employeeCode: string | null;
    employmentType: string | null;
    contractType: string | null;
    weeklyHours: number | null;
    profileVersion: number;
    directReports: EmployeeSummary[];
}

export interface EmployeeDocument {
    id: number;
    name: string;
    fileName: string;
    contentType: string;
    sizeBytes: number;
    createdOnUtc: string;
}

export interface EmployeeHistory {
    id: number;
    actorEmployeeId: number | null;
    changedFields: string[];
    createdOnUtc: string;
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
