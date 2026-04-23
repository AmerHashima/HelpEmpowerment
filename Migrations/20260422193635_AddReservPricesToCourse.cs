using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReservPricesToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExamSimulationReservPrice",
                table: "courses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LiveCourseReservPrice",
                table: "courses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RecordedCourseReservPrice",
                table: "courses",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamSimulationReservPrice",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "LiveCourseReservPrice",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "RecordedCourseReservPrice",
                table: "courses");
        }
    }
}
