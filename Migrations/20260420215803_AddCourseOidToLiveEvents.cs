using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseOidToLiveEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseOid",
                table: "live_webinars",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseOid",
                table: "live_courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_live_webinars_CourseOid",
                table: "live_webinars",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_live_courses_CourseOid",
                table: "live_courses",
                column: "CourseOid");

            migrationBuilder.AddForeignKey(
                name: "FK_live_courses_courses_CourseOid",
                table: "live_courses",
                column: "CourseOid",
                principalTable: "courses",
                principalColumn: "Oid");

            migrationBuilder.AddForeignKey(
                name: "FK_live_webinars_courses_CourseOid",
                table: "live_webinars",
                column: "CourseOid",
                principalTable: "courses",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_live_courses_courses_CourseOid",
                table: "live_courses");

            migrationBuilder.DropForeignKey(
                name: "FK_live_webinars_courses_CourseOid",
                table: "live_webinars");

            migrationBuilder.DropIndex(
                name: "IX_live_webinars_CourseOid",
                table: "live_webinars");

            migrationBuilder.DropIndex(
                name: "IX_live_courses_CourseOid",
                table: "live_courses");

            migrationBuilder.DropColumn(
                name: "CourseOid",
                table: "live_webinars");

            migrationBuilder.DropColumn(
                name: "CourseOid",
                table: "live_courses");
        }
    }
}
