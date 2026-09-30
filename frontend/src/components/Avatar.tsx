type AvatarProps = {
    firstName: string;
    lastName: string;
    size?: "sm" | "md";
};

export function Avatar({ firstName, lastName, size = "md" }: AvatarProps) {
    const initials = `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
    const dimensions = size === "sm" ? "h-8 w-8 text-xs" : "h-10 w-10 text-sm";

    return (
        <div
            className={`flex ${dimensions} shrink-0 items-center justify-center rounded-full bg-[#dfe5ec] font-semibold text-navy`}
        >
            {initials}
        </div>
    );
}