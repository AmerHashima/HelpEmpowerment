using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedatefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_course_answers_AnswerSelectedAnswerOid",
                table: "student_exam_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_course_answers_SelectedAnswerOid",
                table: "student_exam_questions");

            //migrationBuilder.DropIndex(
            //    name: "IX_student_exam_questions_AnswerSelectedAnswerOid",
            //    table: "student_exam_questions");

            //migrationBuilder.DropIndex(
            //    name: "IX_student_exam_questions_SelectedAnswerOid",
            //    table: "student_exam_questions");

            migrationBuilder.DropColumn(
                name: "AnswerSelectedAnswerOid",
                table: "student_exam_questions");

            migrationBuilder.DropColumn(
                name: "SelectedAnswerOid",
                table: "student_exam_questions");

            migrationBuilder.CreateTable(
                name: "student_exam_question_answers",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentExamQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedAnswerOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerSelectedAnswerOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exam_question_answers", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_course_answers_AnswerSelectedAnswerOid",
                        column: x => x.AnswerSelectedAnswerOid,
                        principalTable: "course_answers",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_course_answers_SelectedAnswerOid",
                        column: x => x.SelectedAnswerOid,
                        principalTable: "course_answers",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_student_exam_questions_StudentExamQuestionOid",
                        column: x => x.StudentExamQuestionOid,
                        principalTable: "student_exam_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_StudentExamOid_QuestionOid",
                table: "student_exam_questions",
                columns: new[] { "StudentExamOid", "QuestionOid" });

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_AnswerSelectedAnswerOid",
                table: "student_exam_question_answers",
                column: "AnswerSelectedAnswerOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_IsDeleted",
                table: "student_exam_question_answers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_SelectedAnswerOid",
                table: "student_exam_question_answers",
                column: "SelectedAnswerOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_StudentExamQuestionOid",
                table: "student_exam_question_answers",
                column: "StudentExamQuestionOid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_exam_question_answers");

            migrationBuilder.DropIndex(
                name: "IX_student_exam_questions_StudentExamOid_QuestionOid",
                table: "student_exam_questions");

            migrationBuilder.AddColumn<Guid>(
                name: "AnswerSelectedAnswerOid",
                table: "student_exam_questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedAnswerOid",
                table: "student_exam_questions",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_student_exam_questions_AnswerSelectedAnswerOid",
            //    table: "student_exam_questions",
            //    column: "AnswerSelectedAnswerOid");

            //migrationBuilder.CreateIndex(
            //    name: "IX_student_exam_questions_SelectedAnswerOid",
            //    table: "student_exam_questions",
            //    column: "SelectedAnswerOid");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_course_answers_AnswerSelectedAnswerOid",
                table: "student_exam_questions",
                column: "AnswerSelectedAnswerOid",
                principalTable: "course_answers",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_course_answers_SelectedAnswerOid",
                table: "student_exam_questions",
                column: "SelectedAnswerOid",
                principalTable: "course_answers",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
