CREATE TABLE "Roles" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "Permissions" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "RolePermissions" (
    "RoleId" INT NOT NULL
        REFERENCES "Roles"("Id") ON DELETE CASCADE,
    "PermissionId" INT NOT NULL
        REFERENCES "Permissions"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("RoleId", "PermissionId")
);

CREATE TABLE "EmployeeRoles" (
    "EmployeeId" INT NOT NULL
        REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "RoleId" INT NOT NULL
        REFERENCES "Roles"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("EmployeeId", "RoleId")
);