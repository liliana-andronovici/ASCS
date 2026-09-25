type PlaceholderPageProps = {
    title: string;
};

export default function PlaceholderPage({ title }: PlaceholderPageProps) {
    return (
        <div className="p-6 text-sm text-gray-500">
            {title}, Coming soon ...
        </div>
    );
}