CREATE TABLE "Roles" (
    "Id" INT PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE "Permissions" (
    "Id" INT PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE "RolePermissions" (
    "RoleId" INT NOT NULL
        REFERENCES "Roles"("Id") ON DELETE CASCADE,
    "PermissionId" INT NOT NULL
        REFERENCES "Permissions"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("RoleId", "PermissionId")
);

CREATE INDEX "IX_RolePermissions_PermissionId"
    ON "RolePermissions" ("PermissionId");

CREATE TABLE "EmployeeRoles" (
    "EmployeeId" INT NOT NULL
     REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "RoleId" INT NOT NULL
     REFERENCES "Roles"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("EmployeeId", "RoleId")
);

CREATE INDEX "IX_EmployeeRoles_RoleId"
    ON "EmployeeRoles" ("RoleId");

INSERT INTO "Roles" ("Id", "Name")
VALUES
    (1, 'Employee'),
    (2, 'Admin');

INSERT INTO "Permissions" ("Id", "Name")
VALUES
    (1, 'ReadDepartment'),
    (2, 'ManageDepartment');

INSERT INTO "RolePermissions" ("RoleId", "PermissionId")
VALUES
    (1, 1),
    (2, 1),
    (2, 2);

INSERT INTO "EmployeeRoles" ("EmployeeId", "RoleId")
SELECT "Id", 1
FROM "Employees";