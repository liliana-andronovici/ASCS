CREATE TABLE "Department" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE "Location" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(150) NOT NULL UNIQUE,
    "Title" VARCHAR(150) NOT NULL,
    "StartedOnUtc" TIMESTAMPTZ NOT NULL,
    "DepartmentId" INT REFERENCES "Department"("Id") ON DELETE SET NULL,
    "LocationId" INT NOT NULL REFERENCES "Location"("Id") ON DELETE RESTRICT,
    "ManagerId" INT REFERENCES "Employees"("Id") ON DELETE RESTRICT,
    "InviteToken" VARCHAR(250),
    "InviteTokenExpiresAtUtc" TIMESTAMPTZ,
    "Status" INT NOT NULL
);

CREATE TABLE "Auth" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL UNIQUE REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "PasswordHash" TEXT NOT NULL,
    "ResetToken" VARCHAR(250),
    "ResetTokenExpiresAtUtc" TIMESTAMPTZ
);

CREATE TABLE "LeaveRequest" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "StartDateUtc" TIMESTAMPTZ NOT NULL,
    "EndDateUtc" TIMESTAMPTZ NOT NULL,
    "Note" VARCHAR(500) NOT NULL,
    "Type" INT NOT NULL,
    "StatusT" INT NOT NULL
);

CREATE TABLE "TrustedDevice" (
    "Id" SERIAL PRIMARY KEY,
    "DeviceToken" VARCHAR(250) NOT NULL UNIQUE,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "TrustedUntilUtc" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "TwoFactorAuth" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL UNIQUE REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "SecretKey" VARCHAR(100) NOT NULL
);