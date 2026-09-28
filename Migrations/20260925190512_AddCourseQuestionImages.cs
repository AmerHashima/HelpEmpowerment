using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseQuestionImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_question_images",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_question_images", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_question_images_course_questions_CourseQuestionOid",
                        column: x => x.CourseQuestionOid,
                        principalTable: "course_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_question_images_CourseQuestionOid",
                table: "course_question_images",
                column: "CourseQuestionOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_question_images_CourseQuestionOid_OrderNo",
                table: "course_question_images",
                columns: new[] { "CourseQuestionOid", "OrderNo" });

            // Preserve files uploaded before questions supported multiple images.
            migrationBuilder.Sql(@"
                INSERT INTO [course_question_images]
                    ([Oid], [CourseQuestionOid], [FileName], [OrderNo], [CreatedAt], [IsDeleted])
                SELECT NEWID(), [Oid], [QuestionImage], 1, SYSUTCDATETIME(), 0
                FROM [course_questions]
                WHERE [IsDeleted] = 0
                  AND NULLIF(LTRIM(RTRIM([QuestionImage])), '') IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_question_images");
        }
    }
}
