const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const ts = require('typescript');
const zod = require('zod');
const root = path.resolve(__dirname, '../src');

function load(file, imports = {}, globals = {}) {
    const source = fs.readFileSync(path.join(root, file), 'utf8')
        .replace(/import.meta.env.VITE_API_URL/g, '" https://ascs-bk.onrender.com/api/ "')
        .replace(/import.meta.env.PROD/g, 'true')
        .replace(/import.meta.env.VITE_\w+/g, 'undefined');
    const exports = {};
    vm.runInNewContext(ts.transpileModule(source, { compilerOptions: {
        module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2022,
    } }).outputText, { exports, require(id) {
        if (id === 'zod') return zod;
        assert.ok(imports[id], `Unexpected import ${id}`);
        return imports[id];
    }, Headers, URLSearchParams, ...globals });
    return exports;
}

async function run() {
    const storage = new Map([['accessToken', 'admin-token']]);
    const localStorage = {
        getItem: key => storage.get(key) ?? null,
        setItem: (key, value) => storage.set(key, value),
        removeItem: key => storage.delete(key),
    };
    const sessions = load('lib/sessions.ts', {}, { localStorage });
    assert.equal(sessions.getToken(), 'admin-token');
    sessions.setToken('admin-token');
    assert.equal(storage.get('accessToken'), 'admin-token');

    const calls = [];
    let payload;
    let status = 200;
    let beforeResponse = () => {};
    let redirects = 0;
    const api = load('lib/api.ts', { './sessions': sessions }, {
        fetch: async (url, options) => {
            calls.push({ url, options });
            beforeResponse();
            return { ok: status < 400, status, statusText: 'Error', text: async () => JSON.stringify(payload) };
        }, window: { location: { assign: () => redirects++ } },
    });
    const contract = load('features/rooms/contract.ts');
    const rooms = load('features/rooms/api.ts', { '../../lib/api': api, './contract': contract });
    payload = [{ id: 1, name: 'Room', floor: 1, seats: 6, amenities: null, locationId: 1, locationName: 'Office' }];
    await rooms.getRooms();
    assert.equal(calls.at(-1).url, 'https://ascs-bk.onrender.com/api/Rooms');
    assert.equal(calls.at(-1).options.headers.get('Authorization'), 'Bearer admin-token');
    const start = '2026-10-21T10:00:00Z';
    const end = '2026-10-22T10:00:00Z';
    payload = [{ id: 1, roomId: 1, roomName: 'Room', organizerEmployeeId: 1,
        organizerName: 'Organizer', title: 'Meeting', startDateTimeUtc: start, durationMinutes: 30 }];
    await rooms.getRoomBookings(start, end);
    const bookingUrl = new URL(calls.at(-1).url);
    assert.equal(bookingUrl.pathname, '/api/RoomBookings');
    assert.equal(bookingUrl.searchParams.get('fromUtc'), start);
    assert.equal(bookingUrl.searchParams.get('toUtc'), end);
    const booking = { roomId: 1, title: 'Meeting', startDateTimeUtc: start, durationMinutes: 30 };
    payload = { id: 2 };
    await rooms.createRoomBooking(booking);
    assert.equal(calls.at(-1).options.method, 'POST');
    assert.deepEqual(JSON.parse(calls.at(-1).options.body), booking);

    status = 401;
    await assert.rejects(api.apiFetch('/auth/sign-in', { auth: 'public', method: 'POST' }));
    assert.equal(calls.at(-1).options.headers.has('Authorization'), false);
    assert.equal(sessions.getToken(), 'admin-token');
    assert.equal(redirects, 0);
    beforeResponse = () => sessions.setToken('new-session');
    await assert.rejects(rooms.getRooms());
    assert.equal(sessions.getToken(), 'new-session');
    assert.equal(redirects, 0);
    beforeResponse = () => {};
    status = 500;
    await assert.rejects(rooms.getRooms(), error => error instanceof api.ApiError && error.status === 500);
    assert.equal(sessions.getToken(), 'new-session');
    console.log('PASS: Rooms API paths, DTOs, query parameters, bearer token, public login, stale-session protection and server errors.');
}
run().catch(error => { console.error(error); process.exitCode = 1; });
