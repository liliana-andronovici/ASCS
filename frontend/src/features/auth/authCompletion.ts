import type { NavigateFunction } from "react-router-dom";
import { setToken } from "../../lib/sessions";
import type { AuthResponse } from "./api";

export function completeAuthentication(response: AuthResponse, navigate: NavigateFunction) {
    if (response.requiresTwoFactor) {
        if (!response.challengeToken) throw new Error("Missing verification challenge.");
        navigate("/two-factor", {
            replace: true,
            state: {
                challengeToken: response.challengeToken,
                challengeExpiresAtUtc: response.challengeExpiresAtUtc,
            },
        });
        return;
    }
    if (!response.accessToken) throw new Error("Missing access token.");
    setToken(response.accessToken);
    navigate("/", { replace: true, state: null });
}
