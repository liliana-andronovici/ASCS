CREATE TABLE "Onboardings" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL UNIQUE REFERENCES "Employees" ("Id") ON DELETE CASCADE,
    "BuddyId" INT NULL REFERENCES "Employees" ("Id"),
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ
);

CREATE TABLE "OnboardingTasks" (
    "Id" SERIAL PRIMARY KEY,
    "OnboardingId" INT NOT NULL REFERENCES "Onboardings" ("Id") ON DELETE CASCADE,
    "Section" INT NOT NULL,
    "Title" TEXT NOT NULL,
    "Note" TEXT NULL,
    "DueDateUtc" TIMESTAMPTZ NULL,
    "CompletedOnUtc" TIMESTAMPTZ NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ
);