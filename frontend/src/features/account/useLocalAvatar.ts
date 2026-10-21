import { useSyncExternalStore } from "react";
import { getLocalAvatar, subscribeLocalAvatar } from "../../lib/sessions";

export function useLocalAvatar() {
    return useSyncExternalStore(subscribeLocalAvatar, getLocalAvatar, () => null);
}