import { useEffect, useState, type FormEvent } from "react";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { setToken } from "../../lib/sessions";
import {
    disableTwoFactor, enableTwoFactor, getTwoFactorStatus,
    regenerateTwoFactorCodes, setupTwoFactor,
    type TwoFactorSetup, type TwoFactorStatus,
} from "../auth/api";
import { authErrorMessage } from "../auth/authErrors";

export function TwoFactorSettings() {
    const [status, setStatus] = useState<TwoFactorStatus | null>(null);
    const [setup, setSetup] = useState<TwoFactorSetup | null>(null);
    const [password, setPassword] = useState("");
    const [code, setCode] = useState("");
    const [useRecovery, setUseRecovery] = useState(false);
    const [recoveryCodes, setRecoveryCodes] = useState<string[]>([]);
    const [pending, setPending] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        let active = true;
        getTwoFactorStatus().then(
            (response) => { if (active) setStatus(response); },
            (error) => { if (active) setError(authErrorMessage(error, "Unable to load two-factor settings. Reopen this tab to try again.")); },
        );
        return () => { active = false; };
    }, []);

    async function perform(action: "setup" | "enable" | "disable" | "regenerate") {
        if (pending) return;
        setPending(true);
        setError("");
        try {
            if (action === "setup") {
                setSetup(await setupTwoFactor(password));
            } else if (action === "enable") {
                if (!setup) return;
                const response = await enableTwoFactor(setup.setupId, code);
                setToken(response.accessToken);
                setRecoveryCodes(response.recoveryCodes);
                setStatus({ isEnabled: true, recoveryCodesRemaining: response.recoveryCodes.length });
                setSetup(null);
            } else {
                const proof = { password, ...(useRecovery ? { recoveryCode: code.trim() } : { code }) };
                if (action === "regenerate") {
                    const response = await regenerateTwoFactorCodes(proof);
                    setToken(response.accessToken);
                    setRecoveryCodes(response.recoveryCodes);
                    setStatus({ isEnabled: true, recoveryCodesRemaining: response.recoveryCodes.length });
                } else {
                    const response = await disableTwoFactor(proof);
                    if (!response.accessToken) throw new Error("Missing access token.");
                    setToken(response.accessToken);
                    setStatus({ isEnabled: false, recoveryCodesRemaining: 0 });
                    setRecoveryCodes([]);
                }
            }
            setPassword("");
            setCode("");
            setUseRecovery(false);
        } catch (error) {
            setError(authErrorMessage(error, "Unable to update two-factor authentication. Please try again."));
        } finally {
            setPending(false);
        }
    }

    function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        void perform(setup ? "enable" : status?.isEnabled ? "regenerate" : "setup");
    }

    const validProof = useRecovery ? code.trim().length > 0 : /^\d{6}$/.test(code);

    return (
        <section className="mt-4 flex flex-col gap-4 border-t border-gray-200 pt-4">
            <h3 className="font-semibold text-navy">Two-factor authentication</h3>
            {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
            {!status ? <p className="text-sm text-gray-500">{error ? "Settings unavailable." : "Loading..."}</p> : <>
                <p className="text-sm text-gray-500">
                    {status.isEnabled ? `Enabled. Backup codes remaining: ${status.recoveryCodesRemaining}.` : "Disabled. Add verification with an authenticator app to protect your account."}
                </p>
                {recoveryCodes.length > 0 && <div className="rounded-md bg-amber-50 p-4 text-sm text-amber-900">
                    <p>Save these backup codes in a safe place. Each code can be used once. They will not be shown again after you leave this tab.</p>
                    <ul className="mt-2 break-all font-mono">{recoveryCodes.map((value) => <li key={value}>{value}</li>)}</ul>
                    <Button type="button" variant="secondary" className="mt-3" onClick={() => setRecoveryCodes([])}>I have saved my codes</Button>
                </div>}
                <form className="flex flex-col gap-4" onSubmit={submit}>
                    {setup ? <>
                        <p className="text-sm text-gray-500">Enter this setup key in your authenticator app, then enter its 6-digit code below. Setup expires at {new Date(setup.expiresAtUtc).toLocaleTimeString()}.</p>
                        <code className="break-all rounded-md bg-gray-50 p-3 text-sm select-all">{setup.sharedKey}</code>
                    </> : <Input id="two-factor-password" label="Current password" type="password" autoComplete="current-password" value={password} onChange={(event) => setPassword(event.target.value)} required disabled={pending} />}
                    {(setup || status.isEnabled) && <Input id="two-factor-proof" label={useRecovery ? "Backup code" : "Authenticator code"}
                        value={code} onChange={(event) => setCode(event.target.value)} required disabled={pending}
                        autoComplete="off" inputMode={useRecovery ? "text" : "numeric"} pattern={useRecovery ? undefined : "[0-9]{6}"} />}
                    {status.isEnabled && <button type="button" disabled={pending} className="text-left text-sm text-coral underline"
                        onClick={() => { setUseRecovery(!useRecovery); setCode(""); }}>
                        {useRecovery ? "Use authenticator code" : "Use a backup code"}
                    </button>}
                    <Button type="submit" disabled={pending || (setup ? !validProof : !password || (status.isEnabled && !validProof))}>
                        {pending ? "Saving..." : setup ? "Enable two-factor authentication" : status.isEnabled ? "Regenerate backup codes" : "Set up two-factor authentication"}
                    </Button>
                    {status.isEnabled && <Button type="button" variant="secondary" disabled={pending || !password || !validProof} onClick={() => void perform("disable")}>Disable two-factor authentication</Button>}
                    {setup && <Button type="button" variant="secondary" disabled={pending} onClick={() => { setSetup(null); setCode(""); setError(""); }}>Cancel setup</Button>}
                </form>
            </>}
        </section>
    );
}
