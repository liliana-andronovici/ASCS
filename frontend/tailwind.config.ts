import type {Config} from "tailwindcss";

export default {
    content: ["./index.html","./src/**/*.{ts,tsx}"],
    theme: {
        extend: {
            colors: {
                navy: "#0e2a3b",
                coral: "#e35d56",
            },
            borderRadius: {
                sm: "8px",
                md: "12px",
                lg: "16px",
            },
            fontFamily: {
                sans: ["Plus Jakarta Sans", "sans-serif"],
            },
            boxShadow: {
                card: "0 1px 3px rgba(14, 42, 59, 0.08)",  
            },
        },
    },
} satisfies Config;