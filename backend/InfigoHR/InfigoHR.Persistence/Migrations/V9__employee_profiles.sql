ALTER TABLE "Employees"
    ADD COLUMN "Phone" VARCHAR(40),
    ADD COLUMN "Desk" VARCHAR(150),
    ADD COLUMN "BirthdayMonth" INT,
    ADD COLUMN "BirthdayDay" INT,
    ADD COLUMN "Languages" TEXT[] NOT NULL DEFAULT '{}',
    ADD COLUMN "EmployeeCode" VARCHAR(50),
    ADD COLUMN "EmploymentType" VARCHAR(50),
    ADD COLUMN "ContractType" VARCHAR(100),
    ADD COLUMN "WeeklyHours" NUMERIC(5,2),
    ADD COLUMN "ProfileVersion" BIGINT NOT NULL DEFAULT 0;

ALTER TABLE "Employees"
    ADD CONSTRAINT "CK_Employees_ProfileVersion" CHECK ("ProfileVersion" >= 0),
    ADD CONSTRAINT "CK_Employees_WeeklyHours" CHECK ("WeeklyHours" BETWEEN 0 AND 168),
    ADD CONSTRAINT "CK_Employees_Birthday" CHECK (
        ("BirthdayMonth" IS NULL AND "BirthdayDay" IS NULL) OR
        ("BirthdayMonth" IS NOT NULL AND "BirthdayDay" IS NOT NULL AND
         "BirthdayMonth" BETWEEN 1 AND 12 AND "BirthdayDay" BETWEEN 1 AND
             CASE WHEN "BirthdayMonth" = 2 THEN 29
                  WHEN "BirthdayMonth" IN (4, 6, 9, 11) THEN 30
                  ELSE 31 END)),
    ADD CONSTRAINT "CK_Employees_Manager_NotSelf" CHECK (
        "ManagerId" IS NULL OR "ManagerId" <> "Id");

CREATE INDEX "IX_Employees_ManagerId" ON "Employees" ("ManagerId");

CREATE TABLE "EmployeeDocuments" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "Name" VARCHAR(200) NOT NULL,
    "FileName" VARCHAR(255) NOT NULL,
    "ContentType" VARCHAR(150) NOT NULL,
    "SizeBytes" BIGINT NOT NULL CHECK ("SizeBytes" >= 0),
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);
CREATE INDEX "IX_EmployeeDocuments_EmployeeId_Id" ON "EmployeeDocuments" ("EmployeeId", "Id");

CREATE TABLE "EmployeeHistory" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "ActorEmployeeId" INT REFERENCES "Employees"("Id") ON DELETE SET NULL,
    "ChangedFields" TEXT[] NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);
CREATE INDEX "IX_EmployeeHistory_EmployeeId_Id" ON "EmployeeHistory" ("EmployeeId", "Id");

INSERT INTO "Permissions" ("Name", "CreatedOnUtc", "UpdatedOnUtc")
VALUES ('ManageEmployee', NOW(), NOW())
ON CONFLICT ("Name") DO NOTHING;
