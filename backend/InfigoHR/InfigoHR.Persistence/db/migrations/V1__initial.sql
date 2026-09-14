CREATE TABLE "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL UNIQUE,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "Locations" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL UNIQUE,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(150) NOT NULL UNIQUE,
    "Title" VARCHAR(150) NOT NULL,
    "StartedOnUtc" TIMESTAMPTZ NOT NULL,
    "DepartmentId" INT REFERENCES "Departments"("Id") ON DELETE SET NULL,
    "LocationId" INT REFERENCES "Locations"("Id") ON DELETE SET NULL,
    "ManagerId" INT REFERENCES "Employees"("Id") ON DELETE SET NULL,
    "InviteToken" VARCHAR(250),
    "InviteTokenExpiresAtUtc" TIMESTAMPTZ,
    "Status" INT NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "Auths" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL UNIQUE REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "PasswordHash" TEXT NOT NULL,
    "ResetToken" VARCHAR(250),
    "ResetTokenExpiresAtUtc" TIMESTAMPTZ,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "LeaveRequests" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "StartDateUtc" TIMESTAMPTZ NOT NULL,
    "EndDateUtc" TIMESTAMPTZ NOT NULL,
    "Note" VARCHAR(500) NOT NULL,
    "Type" INT NOT NULL,
    "Status" INT NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "TrustedDevices" (
    "Id" SERIAL PRIMARY KEY,
    "DeviceToken" VARCHAR(250) NOT NULL UNIQUE,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "TrustedUntilUtc" TIMESTAMPTZ NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "TwoFactorAuths" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL UNIQUE REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "SecretKey" VARCHAR(100) NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);
