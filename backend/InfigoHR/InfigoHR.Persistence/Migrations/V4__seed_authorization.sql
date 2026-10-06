INSERT INTO "Permissions" ("Name", "CreatedOnUtc", "UpdatedOnUtc")
VALUES ('ReadDepartment', NOW(), NOW()),
       ('ManageDepartment', NOW(), NOW())
ON CONFLICT ("Name") DO NOTHING;

INSERT INTO "Roles" ("Name", "CreatedOnUtc", "UpdatedOnUtc")
VALUES ('Admin', NOW(), NOW())
ON CONFLICT ("Name") DO NOTHING;

INSERT INTO "RolePermissions" ("RoleId", "PermissionId")
SELECT r."Id", p."Id"
FROM "Roles" r
         CROSS JOIN "Permissions" p
WHERE r."Name" = 'Admin'
  AND p."Name" IN ('ReadDepartment', 'ManageDepartment')
ON CONFLICT ("RoleId", "PermissionId") DO NOTHING;
