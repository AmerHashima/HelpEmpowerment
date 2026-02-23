using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AnswerSelectedAnswerOid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnswerSelectedAnswerOid",
                table: "student_exam_questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_AnswerSelectedAnswerOid",
                table: "student_exam_questions",
                column: "AnswerSelectedAnswerOid");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_course_answers_AnswerSelectedAnswerOid",
                table: "student_exam_questions",
                column: "AnswerSelectedAnswerOid",
                principalTable: "course_answers",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_course_answers_AnswerSelectedAnswerOid",
                table: "student_exam_questions");

            migrationBuilder.DropIndex(
                name: "IX_student_exam_questions_AnswerSelectedAnswerOid",
                table: "student_exam_questions");

            migrationBuilder.DropColumn(
                name: "AnswerSelectedAnswerOid",
                table: "student_exam_questions");
        }
    }
}
