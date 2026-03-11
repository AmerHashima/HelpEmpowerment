using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class initialdbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuestionStatusLookupId",
                table: "student_exam_questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AppLookupH",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupCode", "LookupNameAr", "LookupNameEn", "UpdatedAt" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "QUESTION_STATUS", "حالة السؤال", "Question Status", null });

            migrationBuilder.InsertData(
                table: "AppLookupD",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupHeaderId", "LookupNameAr", "LookupNameEn", "LookupValue", "OrderNo", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444401"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "صحيح", "Correct", "CORRECT", 1, null },
                    { new Guid("44444444-4444-4444-4444-444444444402"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "غير صحيح", "Incorrect", "INCORRECT", 2, null },
                    { new Guid("44444444-4444-4444-4444-444444444403"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "لم يتم الإجابة", "Not Answered", "NOT_ANSWERED", 3, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_QuestionStatusLookupId",
                table: "student_exam_questions",
                column: "QuestionStatusLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions",
                column: "QuestionStatusLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions");

            migrationBuilder.DropIndex(
                name: "IX_student_exam_questions_QuestionStatusLookupId",
                table: "student_exam_questions");

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("44444444-4444-4444-4444-444444444402"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("44444444-4444-4444-4444-444444444403"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DropColumn(
                name: "QuestionStatusLookupId",
                table: "student_exam_questions");
        }
    }
}
