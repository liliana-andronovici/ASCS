using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InfigoHR.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Department",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(
                    type: "character varying(150)",
                    maxLength: 150,
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Department", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Location",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(
                    type: "character varying(150)",
                    maxLength: 150,
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Location", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>(
                    type: "character varying(100)",
                    maxLength: 100,
                    nullable: false),
                LastName = table.Column<string>(
                    type: "character varying(100)",
                    maxLength: 100,
                    nullable: false),
                Email = table.Column<string>(
                    type: "character varying(150)",
                    maxLength: 150,
                    nullable: false),
                Title = table.Column<string>(
                    type: "character varying(150)",
                    maxLength: 150,
                    nullable: false),
                StartedOnUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
                DepartmentId = table.Column<int>(type: "integer", nullable: true),
                LocationId = table.Column<int>(type: "integer", nullable: false),
                ManagerId = table.Column<int>(type: "integer", nullable: true),
                InviteToken = table.Column<string>(
                    type: "character varying(250)",
                    maxLength: 250,
                    nullable: true),
                InviteTokenExpiresAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
                table.ForeignKey(
                    name: "FK_Employees_Department_DepartmentId",
                    column: x => x.DepartmentId,
                    principalTable: "Department",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Employees_Employees_ManagerId",
                    column: x => x.ManagerId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Employees_Location_LocationId",
                    column: x => x.LocationId,
                    principalTable: "Location",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Auth",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EmployeeId = table.Column<int>(type: "integer", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: false),
                ResetToken = table.Column<string>(
                    type: "character varying(250)",
                    maxLength: 250,
                    nullable: true),
                ResetTokenExpiresAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Auth", x => x.Id);
                table.ForeignKey(
                    name: "FK_Auth_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "LeaveRequest",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EmployeeId = table.Column<int>(type: "integer", nullable: false),
                StartDateUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
                EndDateUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
                Note = table.Column<string>(
                    type: "character varying(500)",
                    maxLength: 500,
                    nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                StatusT = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LeaveRequest", x => x.Id);
                table.ForeignKey(
                    name: "FK_LeaveRequest_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TrustedDevice",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                DeviceToken = table.Column<string>(
                    type: "character varying(250)",
                    maxLength: 250,
                    nullable: false),
                EmployeeId = table.Column<int>(type: "integer", nullable: false),
                TrustedUntilUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrustedDevice", x => x.Id);
                table.ForeignKey(
                    name: "FK_TrustedDevice_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TwoFactorAuth",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation(
                        "Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EmployeeId = table.Column<int>(type: "integer", nullable: false),
                SecretKey = table.Column<string>(
                    type: "character varying(100)",
                    maxLength: 100,
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TwoFactorAuth", x => x.Id);
                table.ForeignKey(
                    name: "FK_TwoFactorAuth_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Auth_EmployeeId",
            table: "Auth",
            column: "EmployeeId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Employees_DepartmentId",
            table: "Employees",
            column: "DepartmentId");

        migrationBuilder.CreateIndex(
            name: "IX_Employees_Email",
            table: "Employees",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Employees_LocationId",
            table: "Employees",
            column: "LocationId");

        migrationBuilder.CreateIndex(
            name: "IX_Employees_ManagerId",
            table: "Employees",
            column: "ManagerId");

        migrationBuilder.CreateIndex(
            name: "IX_LeaveRequest_EmployeeId",
            table: "LeaveRequest",
            column: "EmployeeId");

        migrationBuilder.CreateIndex(
            name: "IX_TrustedDevice_DeviceToken",
            table: "TrustedDevice",
            column: "DeviceToken",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TrustedDevice_EmployeeId",
            table: "TrustedDevice",
            column: "EmployeeId");

        migrationBuilder.CreateIndex(
            name: "IX_TwoFactorAuth_EmployeeId",
            table: "TwoFactorAuth",
            column: "EmployeeId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Auth");
        migrationBuilder.DropTable(name: "LeaveRequest");
        migrationBuilder.DropTable(name: "TrustedDevice");
        migrationBuilder.DropTable(name: "TwoFactorAuth");
        migrationBuilder.DropTable(name: "Employees");
        migrationBuilder.DropTable(name: "Department");
        migrationBuilder.DropTable(name: "Location");
    }
}
