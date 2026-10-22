import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Eye, EyeOff } from "lucide-react";
import { Input } from "../../components/Input";
import { Button } from "../../components/Button";
import { signUp } from "./api";
import { signUpSchema } from "./signUpSchema";
import { authErrorMessage } from "./authErrors";

export default function SignUpPage() {
    const navigate = useNavigate();
    const [values, setValues] = useState({ firstName: "", lastName: "", email: "", title: "", password: "", confirmPassword: "" });
    const [showPassword, setShowPassword] = useState(false);
    const [pending, setPending] = useState(false);
    const [error, setError] = useState("");
    const [fields, setFields] = useState<Record<string, string>>({});

    async function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (pending) return;
        setError("");
        const parsed = signUpSchema.safeParse({ ...values, email: values.email.trim() });
        if (!parsed.success) {
            const messages: Record<string, string> = {};
            for (const issue of parsed.error.issues) messages[String(issue.path[0])] ??= issue.message;
            setFields(messages);
            return;
        }
        setFields({});
        setPending(true);
        try {
            const { firstName, lastName, email, title, password } = parsed.data;
            await signUp({ firstName, lastName, email, title, password });
            navigate("/sign-in", { replace: true, state: { signupComplete: true, signupEmail: email } });
        } catch (error) {
            setError(authErrorMessage(error, "Unable to create your account. Please try again."));
        } finally {
            setPending(false);
        }
    }

    function input(name: keyof typeof values, label: string, type = "text", maxLength?: number) {
        return <div key={name}>
            <Input id={name} label={label} type={type} value={values[name]} required disabled={pending}
                maxLength={maxLength} minLength={name.includes("assword") ? 12 : undefined}
                autoComplete={name === "email" ? "email" : name.includes("assword") ? "new-password" : name === "firstName" ? "given-name" : name === "lastName" ? "family-name" : "organization-title"}
                aria-invalid={!!fields[name]} aria-describedby={fields[name] ? `${name}-error` : undefined}
                onChange={event => setValues(previous => ({ ...previous, [name]: event.target.value }))} />
            {fields[name] && <p id={`${name}-error`} className="mt-1 text-sm text-red-600">{fields[name]}</p>}
        </div>;
    }

    return <>
        <h2 className="text-center text-xl font-semibold text-navy">Create account</h2>
        <p className="mb-5 mt-1 text-center text-sm text-gray-500">Register with your work email.</p>
        <form onSubmit={submit} className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-3">{input("firstName", "First name", "text", 100)}{input("lastName", "Last name", "text", 100)}</div>
            {input("email", "Work email", "email", 150)}
            {input("title", "Job title", "text", 150)}
            {input("password", "Password", showPassword ? "text" : "password", 128)}
            <p className="-mt-2 text-xs text-gray-500">Use 12–128 characters.</p>
            {input("confirmPassword", "Confirm password", showPassword ? "text" : "password", 128)}
            <button type="button" disabled={pending} aria-pressed={showPassword} onClick={() => setShowPassword(!showPassword)}
                className="flex items-center gap-2 self-start text-sm text-gray-500">
                {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}{showPassword ? "Hide passwords" : "Show passwords"}
            </button>
            {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
            <Button type="submit" disabled={pending}>{pending ? "Creating account..." : "Create account"}</Button>
        </form>
        <p className="mt-4 text-center text-sm text-gray-500">Already have an account? <Link to="/sign-in" className="text-coral underline">Sign in</Link></p>
    </>;
}
