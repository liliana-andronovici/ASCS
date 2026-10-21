INSERT INTO "Permissions" ("Name", "CreatedOnUtc", "UpdatedOnUtc")
VALUES
    ('ReadDepartment', NOW(), NOW()),
    ('ManageDepartment', NOW(), NOW()),
    ('ManageRoles', NOW(), NOW())
ON CONFLICT ("Name") DO NOTHING;
