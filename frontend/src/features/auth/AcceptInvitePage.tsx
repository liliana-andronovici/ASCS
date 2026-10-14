import { useState, type FormEvent } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Input } from "../../components/Input";
import { invitePreview } from "./invitePreview";

export default function AcceptInvitePage() {
    const invitation = invitePreview;
    const [firstName, setFirstName] = useState(invitation.firstName);
    const [lastName, setLastName] = useState(invitation.lastName);
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [submitted, setSubmitted] = useState(false);
    const checks = [password.length >= 12, /[a-z]/.test(password) && /[A-Z]/.test(password), /\d/.test(password), /[^a-zA-Z0-9]/.test(password)];
    const strength = checks.filter(Boolean).length;
    const strengthLabel = strength === 4 ? "Very strong" : strength === 3 ? "Strong" : strength === 2 ? "Fair" : "Weak";
    const initials = invitation.inviterName.split(/\s+/).map((name) => name[0]).slice(0, 2).join("");

    function submit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setSubmitted(true);
    }

    return (
        <>
            <div className="mb-4 flex items-center gap-3 rounded-xl bg-gray-100 p-3 text-sm leading-5 text-navy">
                <span aria-hidden="true" className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-navy text-xs font-semibold text-white">{initials}</span>
                <p><strong>{invitation.inviterName}</strong> invited you to join <strong>{invitation.companyName}</strong> as {invitation.jobTitle}.</p>
            </div>
            <h2 className="text-[28px] font-semibold leading-9 tracking-tight text-navy">Set up your account</h2>
            <p className="mt-1 mb-5 break-words text-sm text-gray-500">{invitation.email}</p>
            <form onSubmit={submit} className="flex flex-col gap-4">
                <div className="grid grid-cols-2 gap-3">
                    <Input id="first-name" label="First name" autoComplete="given-name" value={firstName} onChange={(event) => setFirstName(event.target.value)} maxLength={100} required />
                    <Input id="last-name" label="Last name" autoComplete="family-name" value={lastName} onChange={(event) => setLastName(event.target.value)} maxLength={100} required />
                </div>
                <div>
                    <div className="relative">
                        <Input id="invite-password" label="Password" type={showPassword ? "text" : "password"} autoComplete="new-password" value={password}
                            onChange={(event) => setPassword(event.target.value)} minLength={12} maxLength={128} aria-describedby="password-strength" className="pr-11" required />
                        <button type="button" onClick={() => setShowPassword((visible) => !visible)}
                            aria-label={showPassword ? "Hide password" : "Show password"} aria-controls="invite-password" aria-pressed={showPassword}
                            className="absolute right-2 bottom-1 flex h-8 w-8 items-center justify-center rounded text-gray-400 hover:text-navy focus-visible:outline-2 focus-visible:outline-coral">
                            {showPassword ? <EyeOff size={18} aria-hidden="true" /> : <Eye size={18} aria-hidden="true" />}
                        </button>
                    </div>
                    <div aria-hidden="true" className="mt-1.5 grid grid-cols-4 gap-1">
                        {checks.map((_, index) => <span key={index} className={`h-[3px] rounded-full ${index < strength ? "bg-coral" : "bg-gray-200"}`} />)}
                    </div>
                    <p id="password-strength" className="mt-1.5 text-xs leading-4 text-gray-500">
                        {password ? `${strengthLabel}. ` : ""}Use at least 12 characters, mixed case, and a number.
                    </p>
                </div>
                <button type="submit" className="w-full rounded-lg bg-coral px-4 py-2 text-left text-sm font-semibold text-white transition-opacity hover:opacity-90 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-coral">Create account</button>
                {submitted && <p role="status" className="text-sm text-gray-600">This is a preview. Account activation will be available when invitation support is connected.</p>}
            </form>
            <p className="mt-4 text-xs leading-5 text-gray-500">By continuing you agree to {invitation.companyName}'s terms and privacy policy.</p>
        </>
    );
}
