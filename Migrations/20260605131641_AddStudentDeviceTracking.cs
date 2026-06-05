using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDeviceTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Browser",
                table: "user_devices");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "user_devices");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "user_devices");

            migrationBuilder.CreateTable(
                name: "student_devices",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FirstLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_devices", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_devices_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_devices_StudentId",
                table: "student_devices",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_devices_StudentId_DeviceId",
                table: "student_devices",
                columns: new[] { "StudentId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_devices_StudentId_IsActive_IsDeleted",
                table: "student_devices",
                columns: new[] { "StudentId", "IsActive", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_devices");

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "user_devices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "user_devices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "user_devices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
