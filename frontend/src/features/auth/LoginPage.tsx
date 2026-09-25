import { useState } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";

export default function LoginPage() {
    const [showPassword, setShowPassword] = useState(false);

    return (
        <>
            <h2 className="text-xl font-semibold text-navy">Sign in</h2>
            <p className="mb-6 text-sm text-gray-500">Use your work account.</p>

            <Button variant="secondary" className="mb-4">
                Continue with Google
            </Button>

            <div className="mb-4 flex items-center gap-3 text-xs text-gray-400">
                <div className="h-px flex-1 bg-gray-200" />
                OR
                <div className="h-px flex-1 bg-gray-200" />
            </div>

            <form className="flex flex-col gap-4">
                <Input id="email" label="Work email" type="email" placeholder="ana.popescu@infigo.ro" />

                <div>
                    <div className="flex items-center justify-between">
                        <label htmlFor="password" className="text-sm text-gray-700">
                            Password
                        </label>
                        <a href="/forgot-password" className="text-xs text-coral hover:underline">
                            Forgot?
                        </a>
                    </div>
                    <div className="relative">
                        <input
                            id="password"
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

                <Button type="submit">Sign in</Button>
            </form>

            <a href="/magic-link" className="mt-4 block text-center text-sm text-coral hover:underline">
                Email me a magic link instead
            </a>
        </>
    );
}
