const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const ts = require('typescript');
const root = path.resolve(__dirname, '../src');
function load(file, imports = {}, globals = {}) {
    const source = fs.readFileSync(path.join(root, file), 'utf8')
        .replace(/import.meta.env.VITE_API_URL/g, 'undefined')
        .replace(/import.meta.env.PROD/g, 'true');
    const exports = {};
    vm.runInNewContext(ts.transpileModule(source, { compilerOptions: {
        module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2022,
    } }).outputText, { exports, require: id => {
        if (id === 'zod') return require('zod');
        assert.ok(imports[id], id); return imports[id];
    }, Headers, ...globals });
    return exports;
}
async function run() {
    const { signUpSchema } = load('features/auth/signUpSchema.ts');
    const values = { firstName: ' Ada ', lastName: ' Example ', email: 'ada@example.com',
        title: ' Engineer ', password: '  Valid password  ', confirmPassword: '  Valid password  ' };
    const parsed = signUpSchema.parse(values);
    assert.equal(parsed.firstName, 'Ada');
    assert.equal(parsed.title, 'Engineer');
    assert.equal(parsed.password, values.password);
    for (const invalid of [{ firstName: ' ' }, { title: '' }, { email: 'invalid' },
        { password: 'short', confirmPassword: 'short' }, { confirmPassword: 'different' }]) {
        assert.equal(signUpSchema.safeParse({ ...values, ...invalid }).success, false);
    }
    let request;
    let status = 201;
    let body = '';
    const api = load('lib/api.ts', { './sessions': {
        getToken: () => 'old-session', clearToken: () => { throw Error('Must preserve existing session'); },
    } }, {
        window: { location: { assign: () => { throw Error('Must not redirect on public request'); } } },
        fetch: async (url, options) => {
            request = { url, options };
            return { ok: status < 400, status, statusText: 'Error', text: async () => body };
        },
    });
    const auth = load('features/auth/api.ts', { '../../lib/api': api });
    const { firstName, lastName, email, title, password } = parsed;
    const input = { firstName, lastName, email, title, password };
    assert.equal(await auth.signUp(input), undefined);
    assert.equal(request.url, 'https://ascs-bk.onrender.com/api/auth/sign-up');
    assert.equal(request.options.method, 'POST');
    assert.equal(request.options.headers.has('Authorization'), false);
    assert.deepEqual(JSON.parse(request.options.body), input);
    assert.equal('confirmPassword' in JSON.parse(request.options.body), false);
    status = 400;
    body = JSON.stringify({ errors: ['An account with this email already exists.'] });
    const errors = load('features/auth/authErrors.ts', { '../../lib/api': api });
    await assert.rejects(auth.signUp(input), error => {
        assert.equal(errors.authErrorMessage(error, 'Fallback'), 'An account with this email already exists.');
        return error.status === 400;
    });
    console.log('PASS signup validation, existing API contract, empty 201 response, duplicate email and public session isolation.');
}
run().catch(error => { console.error(error); process.exitCode = 1; });
