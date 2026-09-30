type AvatarProps = {
    firstName: string;
    lastName: string;
    size?: "sm" | "md" | "lg";
    photoUrl?: string | null;
};

export function Avatar({ firstName, lastName, size = "md", photoUrl }: AvatarProps) {
    const initials = `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
    const dimensions =
        size === "lg" ? "h-16 w-16 text-xl" : size === "sm" ? "h-8 w-8 text-xs" : "h-10 w-10 text-sm";

    if (photoUrl) {
        return (
            <img
                src={photoUrl}
                alt={`${firstName} ${lastName}`}
                className={`${dimensions} shrink-0 rounded-full object-cover`}
            />
        );
    }

    const colors = size === "lg" ? "bg-navy text-white" : "bg-[#dfe5ec] text-navy";

    return (
        <div className={`flex ${dimensions} shrink-0 items-center justify-center rounded-full font-semibold ${colors}`}>
            {initials}
        </div>
    );
}