type AvatarProps = {
    firstName: string;
    lastName: string;
    size?: "sm" | "md" | "lg";
    photoUrl?: string | null;
};

const SIZES = {
    sm: "h-8 w-8 text-xs",
    md: "h-10 w-10 text-sm",
    lg: "h-16 w-16 text-xl",
} as const;

export function Avatar({ firstName, lastName, size = "md", photoUrl }: AvatarProps) {
    const initials = `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
    const dimensions = SIZES[size];

    if (photoUrl) {
        return (
            <img
                src={photoUrl}
                alt=""
                className={`${dimensions} shrink-0 rounded-full object-cover`}
            />
        );
    }

    return (
        <div
            className={`flex ${dimensions} shrink-0 items-center justify-center rounded-full bg-[#dfe5ec] font-semibold text-navy`}
        >
            {initials}
        </div>
    );
}
