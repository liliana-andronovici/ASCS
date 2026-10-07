import { useCallback, useState } from "react";

export interface Preferences {
    emailNotifications: boolean;
    timeOffAlerts: boolean;
    newHireAlerts: boolean;
    pageSize: 10 | 25 | 50;
    compactTables: boolean;
}

const KEY = "infigohr_preferences";

const DEFAULTS: Preferences = {
    emailNotifications: true,
    timeOffAlerts: true,
    newHireAlerts: false,
    pageSize: 25,
    compactTables: false,
};

function load(): Preferences {
    try {
        const raw = localStorage.getItem(KEY);
        return raw ? { ...DEFAULTS, ...JSON.parse(raw) } : DEFAULTS;
    } catch {
        return DEFAULTS;
    }
}

// Preferintele nu au endpoint in backend, deci se tin doar in acest browser.
export function useLocalPreferences() {
    const [prefs, setPrefs] = useState<Preferences>(load);

    const update = useCallback(<K extends keyof Preferences>(key: K, value: Preferences[K]) => {
        setPrefs((prev) => {
            const next = { ...prev, [key]: value };
            localStorage.setItem(KEY, JSON.stringify(next));
            return next;
        });
    }, []);

    return { prefs, update };
}
