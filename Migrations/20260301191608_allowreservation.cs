using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class allowreservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExamSimulationReserv",
                table: "student_courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LiveCourseReserv",
                table: "student_courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecordedCourseReserv",
                table: "student_courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExamSimulationReserv",
                table: "student_baskets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LiveCourseReserv",
                table: "student_baskets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecordedCourseReserv",
                table: "student_baskets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamSimulationReserv",
                table: "student_courses");

            migrationBuilder.DropColumn(
                name: "LiveCourseReserv",
                table: "student_courses");

            migrationBuilder.DropColumn(
                name: "RecordedCourseReserv",
                table: "student_courses");

            migrationBuilder.DropColumn(
                name: "ExamSimulationReserv",
                table: "student_baskets");

            migrationBuilder.DropColumn(
                name: "LiveCourseReserv",
                table: "student_baskets");

            migrationBuilder.DropColumn(
                name: "RecordedCourseReserv",
                table: "student_baskets");
        }
    }
}
