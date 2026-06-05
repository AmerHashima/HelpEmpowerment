using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_course_reservations_AppLookupD_ServiceId",
                table: "student_course_reservations");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "student_course_reservations");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "student_course_reservations",
                newName: "CourseServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_student_course_reservations_StudentCourseId_ServiceId",
                table: "student_course_reservations",
                newName: "IX_student_course_reservations_StudentCourseId_CourseServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_student_course_reservations_ServiceId",
                table: "student_course_reservations",
                newName: "IX_student_course_reservations_CourseServiceId");

            migrationBuilder.CreateTable(
                name: "course_services",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActiveTime = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_services", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_services_AppLookupD_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_services_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_services_CourseId",
                table: "course_services",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_course_services_CourseId_ServiceId",
                table: "course_services",
                columns: new[] { "CourseId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_services_IsActive",
                table: "course_services",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_course_services_ServiceId",
                table: "course_services",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_course_reservations_course_services_CourseServiceId",
                table: "student_course_reservations",
                column: "CourseServiceId",
                principalTable: "course_services",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_course_reservations_course_services_CourseServiceId",
                table: "student_course_reservations");

            migrationBuilder.DropTable(
                name: "course_services");

            migrationBuilder.RenameColumn(
                name: "CourseServiceId",
                table: "student_course_reservations",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_student_course_reservations_StudentCourseId_CourseServiceId",
                table: "student_course_reservations",
                newName: "IX_student_course_reservations_StudentCourseId_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_student_course_reservations_CourseServiceId",
                table: "student_course_reservations",
                newName: "IX_student_course_reservations_ServiceId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "student_course_reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_student_course_reservations_AppLookupD_ServiceId",
                table: "student_course_reservations",
                column: "ServiceId",
                principalTable: "AppLookupD",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
