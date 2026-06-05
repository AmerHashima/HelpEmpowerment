using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToStudentCourseReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "student_course_reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "student_course_reservations");
        }
    }
}
