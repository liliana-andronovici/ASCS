import { useRef, type ClipboardEvent, type KeyboardEvent } from "react";

interface CodeInputProps {
    value: string;
    onChange: (value: string) => void;
    disabled?: boolean;
}

export function CodeInput({ value, onChange, disabled }: CodeInputProps) {
    const inputs = useRef<Array<HTMLInputElement | null>>([]);
    // Keep empty slots while editing so deleting a digit never shifts the others.
    const digits = Array.from({ length: 6 }, (_, index) => value[index] === " " ? "" : value[index] ?? "");

    function insert(index: number, raw: string) {
        const incoming = raw.replace(/\D/g, "").slice(0, 6 - index);
        const next = [...digits];
        if (!incoming) next[index] = "";
        else incoming.split("").forEach((digit, offset) => { next[index + offset] = digit; });
        onChange(next.map((digit) => digit || " ").join(""));
        if (incoming) inputs.current[Math.min(index + incoming.length, 5)]?.focus();
    }

    function paste(event: ClipboardEvent<HTMLInputElement>, index: number) {
        event.preventDefault();
        const text = event.clipboardData.getData("text");
        insert(text.replace(/\D/g, "").length >= 6 ? 0 : index, text);
    }

    function keyDown(event: KeyboardEvent<HTMLInputElement>, index: number) {
        if (event.key === "ArrowLeft" || event.key === "ArrowRight") {
            event.preventDefault();
            inputs.current[Math.max(0, Math.min(5, index + (event.key === "ArrowLeft" ? -1 : 1)))]?.focus();
        } else if (event.key === "Backspace" && !digits[index] && index > 0) {
            event.preventDefault();
            insert(index - 1, "");
            inputs.current[index - 1]?.focus();
        }
    }

    return (
        <fieldset disabled={disabled}>
            <legend className="mb-2 text-sm text-gray-700">Verification code</legend>
            <div className="grid grid-cols-6 gap-2">
                {digits.map((digit, index) => (
                    <input
                        key={index}
                        ref={(element) => { inputs.current[index] = element; }}
                        aria-label={`Digit ${index + 1} of 6`}
                        type="text"
                        inputMode="numeric"
                        autoComplete={index === 0 ? "one-time-code" : "off"}
                        pattern="[0-9]"
                        required
                        value={digit}
                        onChange={(event) => insert(index, event.target.value)}
                        onPaste={(event) => paste(event, index)}
                        onKeyDown={(event) => keyDown(event, index)}
                        onFocus={(event) => event.target.select()}
                        className="h-13 min-w-0 rounded-lg border border-gray-300 text-center text-2xl font-semibold text-navy focus:border-coral focus:outline-none focus:ring-1 focus:ring-coral disabled:opacity-50"
                    />
                ))}
            </div>
        </fieldset>
    );
}
