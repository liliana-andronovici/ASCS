export interface OrgChartEmployee {
    id: number;
    firstName: string;
    lastName: string;
    title: string;
    directReports: OrgChartEmployee[];
}
