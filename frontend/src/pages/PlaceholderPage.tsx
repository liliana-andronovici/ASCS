import { PageHeader } from "../layout/PageHeader";

type PlaceholderPageProps = {
    title: string;
};

export default function PlaceholderPage({ title }: PlaceholderPageProps) {
    return (
        <>
            <PageHeader title={title} />
            <div className="p-6 text-sm text-gray-500">{title}, Coming soon ...</div>
        </>
    );
}