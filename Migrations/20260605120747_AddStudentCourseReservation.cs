using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCourseReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_course_reservations",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReserved = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_course_reservations", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_course_reservations_AppLookupD_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_course_reservations_student_courses_StudentCourseId",
                        column: x => x.StudentCourseId,
                        principalTable: "student_courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_devices",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_user_devices", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_user_devices_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_course_reservations_IsReserved",
                table: "student_course_reservations",
                column: "IsReserved");

            migrationBuilder.CreateIndex(
                name: "IX_student_course_reservations_ServiceId",
                table: "student_course_reservations",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_student_course_reservations_StudentCourseId",
                table: "student_course_reservations",
                column: "StudentCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_student_course_reservations_StudentCourseId_ServiceId",
                table: "student_course_reservations",
                columns: new[] { "StudentCourseId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_UserId",
                table: "user_devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_UserId_DeviceId",
                table: "user_devices",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_devices_UserId_IsActive_IsDeleted",
                table: "user_devices",
                columns: new[] { "UserId", "IsActive", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_course_reservations");

            migrationBuilder.DropTable(
                name: "user_devices");
        }
    }
}
