using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class freexam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams");

            migrationBuilder.AddColumn<bool>(
                name: "FreeExam",
                table: "courses_Master_Exam",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams",
                column: "ExamModeLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams");

            migrationBuilder.DropColumn(
                name: "FreeExam",
                table: "courses_Master_Exam");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams",
                column: "ExamModeLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid");
        }
    }
}
