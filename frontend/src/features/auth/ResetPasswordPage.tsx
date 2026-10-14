import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "../../components/Button";
import { Input } from "../../components/Input";
import { completePasswordReset, requestPasswordReset, verifyPasswordReset } from "./api";
import { completeAuthentication } from "./authCompletion";
import { authErrorMessage } from "./authErrors";
import { CodeInput } from "./CodeInput";

export default function ResetPasswordPage() {
    const [step, setStep] = useState<"email" | "code" | "password">("email");
    const [email, setEmail] = useState("");
    const [code, setCode] = useState("");
    const [resetToken, setResetToken] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [pending, setPending] = useState(false);
    const [error, setError] = useState("");
    const [notice, setNotice] = useState("");
    const navigate = useNavigate();

    function startAgain() {
        setStep("email");
        setCode("");
        setResetToken("");
        setNewPassword("");
        setConfirmPassword("");
        setError("");
        setNotice("");
    }

    async function sendCode() {
        if (pending) return;
        setPending(true);
        setError("");
        setNotice("");
        try {
            await requestPasswordReset(email.trim());
            setEmail(email.trim());
            setCode("");
            setStep("code");
            setNotice("If an active account exists for this email, a reset code will be sent. Delivery may take a moment.");
        } catch (error) {
            setError(authErrorMessage(error, "Unable to request a code. Please try again."));
        } finally {
            setPending(false);
        }
    }

    async function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (pending) return;
        if (step === "email") { await sendCode(); return; }
        setError("");
        setNotice("");
        if (step === "password" && newPassword !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }
        setPending(true);
        try {
            if (step === "code") {
                const response = await verifyPasswordReset(email, code);
                if (!response.resetToken) throw new Error("Missing reset token.");
                setResetToken(response.resetToken);
                setCode("");
                setStep("password");
            } else {
                const response = await completePasswordReset({ email, resetToken, newPassword, confirmPassword });
                setResetToken("");
                setNewPassword("");
                setConfirmPassword("");
                completeAuthentication(response, navigate);
            }
        } catch (error) {
            setError(authErrorMessage(error, step === "code"
                ? "Unable to verify the code. Please try again or request another code."
                : "Unable to reset your password. Please try again or start again."));
        } finally {
            setPending(false);
        }
    }

    const title = step === "email" ? "Reset password" : step === "code" ? "Check your email" : "Create a new password";
    const description = step === "email" ? "Enter your work email to reset your password."
        : step === "code" ? `Enter the 6-digit code sent to ${email}.`
        : "Choose a password between 12 and 128 characters.";

    return (
        <>
            <h2 className="text-2xl font-semibold text-navy">{title}</h2>
            <p className="mb-6 mt-1 break-words text-sm text-gray-500">{description}</p>
            <form className="flex flex-col gap-4" onSubmit={submit}>
                {step === "email" && <Input id="reset-email" label="Work email" type="email" autoComplete="email"
                    maxLength={150} value={email} onChange={(event) => setEmail(event.target.value)} required disabled={pending} />}
                {step === "code" && <CodeInput value={code} onChange={setCode} disabled={pending} />}
                {step === "password" && <>
                    <Input id="new-password" label="New password" type="password" autoComplete="new-password"
                        minLength={12} maxLength={128} value={newPassword} onChange={(event) => setNewPassword(event.target.value)} required disabled={pending} />
                    <Input id="confirm-password" label="Confirm new password" type="password" autoComplete="new-password"
                        minLength={12} maxLength={128} value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} required disabled={pending} />
                </>}
                {notice && <p role="status" className="text-sm text-gray-600">{notice}</p>}
                {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
                <Button type="submit" disabled={pending || (step === "code" && !/^\d{6}$/.test(code))}>
                    {pending ? "Please wait..." : step === "email" ? "Send reset code" : step === "code" ? "Verify code" : "Reset password"}
                </Button>
            </form>
            {step === "code" && <div className="mt-4 flex flex-wrap justify-between gap-3 text-sm">
                <button type="button" disabled={pending} onClick={sendCode} className="text-coral underline disabled:opacity-50">Resend code</button>
                <button type="button" disabled={pending} onClick={startAgain} className="text-coral underline disabled:opacity-50">Change email</button>
            </div>}
            {step === "password" && <button type="button" disabled={pending} onClick={startAgain}
                className="mt-4 text-sm text-coral underline disabled:opacity-50">Start again</button>}
            <Link to="/sign-in" className="mt-4 block text-center text-sm text-coral hover:underline">Back to sign in</Link>
        </>
    );
}
