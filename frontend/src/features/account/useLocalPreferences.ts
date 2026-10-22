import { useCallback, useState } from "react";
import { getCurrentEmployeeId } from "../../lib/sessions";

export interface Preferences {
    emailNotifications: boolean;
    timeOffAlerts: boolean;
    newHireAlerts: boolean;
    pageSize: 10 | 25 | 50;
    compactTables: boolean;
}

const KEY = "hr_portal_preferences";

function storageKey(): string {
    return `${KEY}_${getCurrentEmployeeId() ?? "guest"}`;
}

const DEFAULTS: Preferences = {
    emailNotifications: true,
    timeOffAlerts: true,
    newHireAlerts: false,
    pageSize: 25,
    compactTables: false,
};

function load(): Preferences {
    try {
        const raw = localStorage.getItem(storageKey());
        return raw ? { ...DEFAULTS, ...JSON.parse(raw) } : DEFAULTS;
    } catch {
        return DEFAULTS;
    }
}

export function useLocalPreferences() {
    const [prefs, setPrefs] = useState<Preferences>(load);

    const update = useCallback(<K extends keyof Preferences>(key: K, value: Preferences[K]) => {
        setPrefs((prev) => {
            const next = { ...prev, [key]: value };
            localStorage.setItem(storageKey(), JSON.stringify(next));
            return next;
        });
    }, []);

    return { prefs, update };
}
