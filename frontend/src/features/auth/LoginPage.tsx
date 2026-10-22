import { useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ApiError } from "../../lib/api";
import { signIn } from "./api";
import { Eye, EyeOff } from "lucide-react";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { completeAuthentication } from "./authCompletion";

export default function LoginPage() {
    const [showPassword, setShowPassword] = useState(false);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isPending, setIsPending] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (isPending) return;

        setIsPending(true);
        setError("");

        try {
            const response = await signIn({ email: email.trim(), password });
            completeAuthentication(response, navigate);
        } catch (error) {
            setError(error instanceof ApiError && error.status === 401
                ? "Invalid email or password."
                : "Unable to sign in. Please try again.");
        } finally {
            setIsPending(false);
        }
    }

    return (
        <>
            <h2 className="text-center text-xl font-semibold text-navy">Sign in</h2>

            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <Input
                    id="email"
                    label="Work email"
                    type="email"
                    placeholder="name@company.com"
                    autoComplete="username"
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                    required
                    disabled={isPending}
                />

                <div>
                    <div className="flex items-center justify-between">
                        <label htmlFor="password" className="text-sm text-gray-700">
                            Password
                        </label>
                        <Link to="/reset-password" className="text-xs text-coral hover:underline">
                            Forgot?
                        </Link>
                    </div>
                    <div className="relative">
                        <input
                            id="password"
                            autoComplete="current-password"
                            value={password}
                            onChange={(event) => setPassword(event.target.value)}
                            required
                            disabled={isPending}
                            type={showPassword ? "text" : "password"}
                            className="w-full rounded-md border border-gray-300 px-3 py-2 pr-10 text-sm focus:border-navy focus:outline-none"
                        />
                        <button
                            type="button"
                            onClick={() => setShowPassword((v) => !v)}
                            className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400"
                        >
                            {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                        </button>
                    </div>
                </div>

                {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
                <Button type="submit" disabled={isPending}>
                    {isPending ? "Signing in..." : "Sign in"}
                </Button>
            </form>

            <Link to="/magic-link" className="mt-4 block text-center text-sm text-coral hover:underline">
                Email me a magic link instead
            </Link>
            <Link to="/two-factor" className="mt-3 block text-center text-sm text-gray-500 hover:underline">
                Have a verification code?
            </Link>
            <Link to="/accept-invite" className="mt-2 block text-center text-sm text-coral hover:underline">
                Have an invitation? Accept it
            </Link>
        </>
    );
}
