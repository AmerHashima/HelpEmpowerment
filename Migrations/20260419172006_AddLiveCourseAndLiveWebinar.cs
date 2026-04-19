using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveCourseAndLiveWebinar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "live_courses",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CourseFormat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumberOfSessions = table.Column<int>(type: "int", nullable: true),
                    TotalHours = table.Column<int>(type: "int", nullable: true),
                    ScheduleNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhatsAppLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_live_courses", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "live_webinars",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebinarName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WebinarFormat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WebinarDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WebinarStartTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WebinarEndTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    WhatsAppLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_live_webinars", x => x.Oid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "live_courses");

            migrationBuilder.DropTable(
                name: "live_webinars");
        }
    }
}
