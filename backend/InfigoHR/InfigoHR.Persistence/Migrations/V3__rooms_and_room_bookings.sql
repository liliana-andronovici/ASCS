CREATE TABLE "Rooms"
(
    "Id"           SERIAL PRIMARY KEY,
    "Name"         VARCHAR(150) NOT NULL,
    "Floor"        INT          NOT NULL,
    "Seats"        INT          NOT NULL,
    "Amenities"    VARCHAR(500),
    "LocationId"   INT          NOT NULL REFERENCES "Locations" ("Id") ON DELETE CASCADE,
    "CreatedOnUtc" TIMESTAMPTZ  NOT NULL,
    "UpdatedOnUtc" TIMESTAMPTZ  NOT NULL
);

CREATE TABLE "RoomBookings"
(
    "Id"                  SERIAL PRIMARY KEY,
    "RoomId"              INT          NOT NULL REFERENCES "Rooms" ("Id") ON DELETE CASCADE,
    "OrganizerEmployeeId" INT          NOT NULL REFERENCES "Employees" ("Id") ON DELETE CASCADE,
    "Title"               VARCHAR(200) NOT NULL,
    "StartDateTimeUtc"    TIMESTAMPTZ  NOT NULL,
    "DurationMinutes"     INT          NOT NULL,
    "CreatedOnUtc"        TIMESTAMPTZ  NOT NULL,
    "UpdatedOnUtc"        TIMESTAMPTZ  NOT NULL
);

CREATE TABLE "EmployeeRoomBookings"
(
    "EmployeeId"    INT NOT NULL REFERENCES "Employees" ("Id") ON DELETE CASCADE,
    "RoomBookingId" INT NOT NULL REFERENCES "RoomBookings" ("Id") ON DELETE CASCADE,
    PRIMARY KEY ("EmployeeId", "RoomBookingId")
);