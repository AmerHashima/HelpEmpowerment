using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseTabContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_tab_contents",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TabKey = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    ContentJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_tab_contents", x => x.Oid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_CourseCode_Status_IsDeleted",
                table: "course_tab_contents",
                columns: new[] { "CourseCode", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_CourseCode_TabKey",
                table: "course_tab_contents",
                columns: new[] { "CourseCode", "TabKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_CreatedAt",
                table: "course_tab_contents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_IsDeleted",
                table: "course_tab_contents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_IsDeleted_CreatedAt",
                table: "course_tab_contents",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_tab_contents_Oid",
                table: "course_tab_contents",
                column: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_tab_contents");
        }
    }
}
