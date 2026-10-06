-- Run explicitly with psql against the intended database after V4 has been applied:
-- psql <connection-options> -X -v admin_email="<existing employee email>" -f bootstrap-admin.sql
-- This is an operator script, not an automatic migration.
-- Sign in again afterwards to receive a token with the new permissions.
\set ON_ERROR_STOP on

\if :{?admin_email}
\else
    \echo 'Required psql variable: admin_email. No changes made.'
    \quit 1
\endif

BEGIN ISOLATION LEVEL READ COMMITTED;

-- Serialize assignments, including concurrent runs targeting different employees.
-- This lock blocks writes to EmployeeRoles until the transaction ends.
LOCK TABLE "EmployeeRoles" IN SHARE ROW EXCLUSIVE MODE;

-- psql quotes the input as a SQL literal. The setting exists only in this transaction.
SELECT set_config('infigohr.bootstrap_admin_email', :'admin_email', true);

DO $$
DECLARE
    target_email TEXT := btrim(current_setting('infigohr.bootstrap_admin_email'));
    target_employee_id INTEGER;
    admin_role_id INTEGER;
BEGIN
    IF target_email = '' THEN
        RAISE EXCEPTION 'admin_email must not be empty';
    END IF;

    -- Exact email matching follows the current sign-in lookup.
    SELECT "Id" INTO target_employee_id
    FROM "Employees"
    WHERE "Email" = target_email AND "Status" = 1
    FOR SHARE;

    IF target_employee_id IS NULL THEN
        RAISE EXCEPTION 'No active employee found for the supplied email';
    END IF;

    PERFORM 1 FROM "Auths"
    WHERE "EmployeeId" = target_employee_id
      AND btrim("PasswordHash") <> ''
    FOR SHARE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'The employee must already have login credentials';
    END IF;

    SELECT "Id" INTO admin_role_id
    FROM "Roles"
    WHERE "Name" = 'Admin'
    FOR SHARE;

    IF admin_role_id IS NULL THEN
        RAISE EXCEPTION 'Admin role is missing; apply V4 before bootstrapping';
    END IF;

    IF (
        SELECT COUNT(*)
        FROM "RolePermissions" rp
        JOIN "Permissions" p ON p."Id" = rp."PermissionId"
        WHERE rp."RoleId" = admin_role_id
          AND p."Name" IN ('ReadDepartment', 'ManageDepartment')
    ) <> 2 THEN
        RAISE EXCEPTION 'Admin permission mappings are incomplete; apply V4 first';
    END IF;

    IF EXISTS (
        SELECT 1 FROM "EmployeeRoles"
        WHERE "RoleId" = admin_role_id
          AND "EmployeeId" <> target_employee_id
    ) THEN
        RAISE EXCEPTION 'Another employee already has Admin; bootstrap refused';
    END IF;

    INSERT INTO "EmployeeRoles" ("EmployeeId", "RoleId")
    VALUES (target_employee_id, admin_role_id)
    ON CONFLICT ("EmployeeId", "RoleId") DO NOTHING;
END $$;

COMMIT;
