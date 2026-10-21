import assert from "node:assert/strict";
import test from "node:test";
import { apiUrl } from "../src/lib/apiUrl.ts";

test("Render URLs have one separator even with whitespace or a trailing slash", () => {
    assert.equal(apiUrl(" https://ascs-bk.onrender.com/api/ ", "/Rooms", true),
        "https://ascs-bk.onrender.com/api/Rooms");
});

test("blank preview configuration uses Render while local development uses the proxy", () => {
    assert.equal(apiUrl(" ", "/auth/sign-in", true), "https://ascs-bk.onrender.com/api/auth/sign-in");
    assert.equal(apiUrl(undefined, "/Rooms", false), "/api/Rooms");
});

test("custom endpoints and booking query parameters are preserved", () => {
    assert.equal(apiUrl("/api/", "RoomBookings?fromUtc=2026-10-21T00%3A00%3A00Z", false),
        "/api/RoomBookings?fromUtc=2026-10-21T00%3A00%3A00Z");
});
