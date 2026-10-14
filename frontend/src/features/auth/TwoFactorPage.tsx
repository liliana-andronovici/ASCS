import { useState, type FormEvent } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { verifyTwoFactor } from "./api";
import { completeAuthentication } from "./authCompletion";
import { authErrorMessage } from "./authErrors";
import { CodeInput } from "./CodeInput";

export default function TwoFactorPage() {
    const location = useLocation();
    const state = location.state as { challengeToken?: string; challengeExpiresAtUtc?: string } | null;
    const navigate = useNavigate();
    const [code, setCode] = useState("");
    const [recoveryCode, setRecoveryCode] = useState("");
    const [useRecovery, setUseRecovery] = useState(false);
    const [pending, setPending] = useState(false);
    const [error, setError] = useState("");

    async function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (pending || !state?.challengeToken) return;
        setPending(true);
        setError("");
        try {
            const response = await verifyTwoFactor({ challengeToken: state.challengeToken,
                ...(useRecovery ? { recoveryCode: recoveryCode.trim() } : { code }) });
            completeAuthentication(response, navigate);
        } catch (error) {
            setError(authErrorMessage(error, "Unable to verify your code. Please try again or sign in again."));
        } finally { setPending(false); }
    }

    if (!state?.challengeToken) return (
        <>
            <h2 className="text-xl font-semibold text-navy">Sign in first</h2>
            <p className="mt-2 text-sm text-gray-500">Sign in with your email and password to start verification.</p>
            <Link to="/sign-in" replace state={null} className="mt-4 block text-sm text-coral underline">Back to sign in</Link>
        </>
    );

    return (
        <>
            <h2 className="text-2xl font-semibold text-navy">{useRecovery ? "Use a backup code" : "Two-factor authentication"}</h2>
            <p className="mb-6 mt-1 text-sm text-gray-500">{useRecovery ? "Enter one of your unused recovery codes." : "Enter the 6-digit code from your authenticator app."}</p>
            <form className="flex flex-col gap-4" onSubmit={submit}>
                {useRecovery ? <Input id="recovery-code" label="Backup code" autoComplete="off" value={recoveryCode}
                    onChange={(event) => setRecoveryCode(event.target.value)} required disabled={pending} />
                    : <CodeInput value={code} onChange={setCode} disabled={pending} />}
                {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
                <Button type="submit" disabled={pending || (!useRecovery && !/^\d{6}$/.test(code))}>{pending ? "Verifying..." : "Verify"}</Button>
            </form>
            <button type="button" disabled={pending} onClick={() => { setUseRecovery(!useRecovery); setError(""); setCode(""); setRecoveryCode(""); }}
                className="mt-4 text-sm text-coral underline disabled:opacity-50">{useRecovery ? "Use authenticator code" : "Use a backup code"}</button>
            <Link to="/sign-in" replace state={null} className="mt-4 block text-center text-sm text-coral hover:underline">Back to sign in</Link>
        </>
    );
}
