using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleSubQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_question_sub_questions",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionTextAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_question_sub_questions", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_question_sub_questions_course_questions_CourseQuestionOid",
                        column: x => x.CourseQuestionOid,
                        principalTable: "course_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_question_sub_question_choices",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChoiceText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChoiceTextAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_question_sub_question_choices", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_question_sub_question_choices_course_question_sub_questions_SubQuestionOid",
                        column: x => x.SubQuestionOid,
                        principalTable: "course_question_sub_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_exam_sub_question_answers",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentExamQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedChoiceOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AwardedScore = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exam_sub_question_answers", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_exam_sub_question_answers_course_question_sub_question_choices_SelectedChoiceOid",
                        column: x => x.SelectedChoiceOid,
                        principalTable: "course_question_sub_question_choices",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_sub_question_answers_course_question_sub_questions_SubQuestionOid",
                        column: x => x.SubQuestionOid,
                        principalTable: "course_question_sub_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_sub_question_answers_student_exam_questions_StudentExamQuestionOid",
                        column: x => x.StudentExamQuestionOid,
                        principalTable: "student_exam_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppLookupD",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupHeaderId", "LookupNameAr", "LookupNameEn", "LookupValue", "OrderNo", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333307"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("33333333-3333-3333-3333-333333333333"), "أسئلة متعددة برأس مشترك", "Multiple Sub-Questions", "MULTI_IMAGE", 7, null });

            migrationBuilder.CreateIndex(
                name: "IX_course_question_sub_question_choices_SubQuestionOid_IsDeleted_IsCorrect",
                table: "course_question_sub_question_choices",
                columns: new[] { "SubQuestionOid", "IsDeleted", "IsCorrect" });

            migrationBuilder.CreateIndex(
                name: "IX_course_question_sub_question_choices_SubQuestionOid_OrderNo",
                table: "course_question_sub_question_choices",
                columns: new[] { "SubQuestionOid", "OrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_course_question_sub_questions_CourseQuestionOid_IsDeleted",
                table: "course_question_sub_questions",
                columns: new[] { "CourseQuestionOid", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_course_question_sub_questions_CourseQuestionOid_OrderNo",
                table: "course_question_sub_questions",
                columns: new[] { "CourseQuestionOid", "OrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_sub_question_answers_SelectedChoiceOid",
                table: "student_exam_sub_question_answers",
                column: "SelectedChoiceOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_sub_question_answers_StudentExamQuestionOid_SubQuestionOid",
                table: "student_exam_sub_question_answers",
                columns: new[] { "StudentExamQuestionOid", "SubQuestionOid" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_sub_question_answers_SubQuestionOid",
                table: "student_exam_sub_question_answers",
                column: "SubQuestionOid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_exam_sub_question_answers");

            migrationBuilder.DropTable(
                name: "course_question_sub_question_choices");

            migrationBuilder.DropTable(
                name: "course_question_sub_questions");

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("33333333-3333-3333-3333-333333333307"));
        }
    }
}
