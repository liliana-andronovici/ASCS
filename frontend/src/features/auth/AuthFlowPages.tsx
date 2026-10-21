import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";

function AuthPageHeader({ title, description }: { title: string; description: string }) {
    return (
        <>
            <h2 className="text-xl font-semibold text-navy">{title}</h2>
            <p className="mb-6 text-sm text-gray-500">{description}</p>
        </>
    );
}

function BackendNotice() {
    return (
        <p className="rounded-md bg-amber-50 px-3 py-2 text-sm text-amber-800" role="status">
            This authentication flow is not connected to the backend yet.
        </p>
    );
}

function BackToSignIn() {
    return (
        <Link to="/sign-in" className="mt-4 block text-center text-sm text-coral hover:underline">
            Back to sign in
        </Link>
    );
}

export function MagicLinkPage() {
    const [email, setEmail] = useState("");
    const [submitted, setSubmitted] = useState(false);

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setSubmitted(true);
    }

    return (
        <>
            <AuthPageHeader title="Sign in with a magic link" description="We’ll email you a link to sign in." />
            <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
                <Input
                    id="magic-link-email"
                    label="Work email"
                    type="email"
                    autoComplete="email"
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                    required
                />
                <BackendNotice />
                {submitted && <p className="text-sm text-amber-800">No magic-link endpoint is configured.</p>}
                <Button type="submit">Email me a sign-in link</Button>
            </form>
            <BackToSignIn />
        </>
    );
}
export { default as ResetPasswordPage } from "./ResetPasswordPage";
export { default as TwoFactorPage } from "./TwoFactorPage";

export { default as AcceptInvitePage } from "./AcceptInvitePage";
