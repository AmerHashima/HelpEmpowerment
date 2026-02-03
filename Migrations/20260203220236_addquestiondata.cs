using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class addquestiondata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrectAnswerOid",
                table: "course_answers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Question_Ask",
                table: "course_answers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_CorrectAnswerOid",
                table: "course_answers",
                column: "CorrectAnswerOid");

            migrationBuilder.AddForeignKey(
                name: "FK_course_answers_course_answers_CorrectAnswerOid",
                table: "course_answers",
                column: "CorrectAnswerOid",
                principalTable: "course_answers",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_answers_course_answers_CorrectAnswerOid",
                table: "course_answers");

            migrationBuilder.DropIndex(
                name: "IX_course_answers_CorrectAnswerOid",
                table: "course_answers");

            migrationBuilder.DropColumn(
                name: "CorrectAnswerOid",
                table: "course_answers");

            migrationBuilder.DropColumn(
                name: "Question_Ask",
                table: "course_answers");
        }
    }
}
