CREATE TABLE "EquipmentType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(80) NOT NULL,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE UNIQUE INDEX "IX_EquipmentType_Name"
    ON "EquipmentType" (LOWER("Name"));

INSERT INTO "EquipmentType" ("Name", "CreatedOnUtc", "UpdatedOnUtc") VALUES
    ('Keyboard', NOW(), NOW()),
    ('Mouse', NOW(), NOW()),
    ('Headset', NOW(), NOW());

CREATE TABLE "Equipment" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL,
    "Code" VARCHAR(80) NOT NULL,
    "EquipmentTypeId" INT NOT NULL REFERENCES "EquipmentType"("Id") ON DELETE RESTRICT,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL
);

CREATE UNIQUE INDEX "IX_Equipment_Code"
    ON "Equipment" (LOWER("Code"));

CREATE TABLE "EmployeeEquipment" (
    "Id" SERIAL PRIMARY KEY,
    "EmployeeId" INT NOT NULL REFERENCES "Employees"("Id") ON DELETE CASCADE,
    "EquipmentId" INT NOT NULL REFERENCES "Equipment"("Id") ON DELETE RESTRICT,
    "AssignedOnUtc" TIMESTAMPTZ NOT NULL,
    "ReturnedOnUtc" TIMESTAMPTZ,
    "CreatedOnUtc" TIMESTAMPTZ NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ NOT NULL,
    CONSTRAINT "CK_EmployeeEquipment_ReturnedAfterAssigned"
        CHECK ("ReturnedOnUtc" IS NULL OR "ReturnedOnUtc" >= "AssignedOnUtc")
);

CREATE INDEX "IX_EmployeeEquipment_EmployeeId"
    ON "EmployeeEquipment" ("EmployeeId");

CREATE UNIQUE INDEX "IX_EmployeeEquipment_ActiveAssignment"
    ON "EmployeeEquipment" ("EquipmentId")
    WHERE "ReturnedOnUtc" IS NULL;
