using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpFieldsToUserAndStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiry",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiry",
                table: "students",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "users");

            migrationBuilder.DropColumn(
                name: "OtpExpiry",
                table: "users");

            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "students");

            migrationBuilder.DropColumn(
                name: "OtpExpiry",
                table: "students");
        }
    }
}
