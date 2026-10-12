export interface Role {
    id: number;
    name: string;
    employeeCount: number;
    permissions: string[];
}

export interface Permission {
    id: number;
    name: string;
}
