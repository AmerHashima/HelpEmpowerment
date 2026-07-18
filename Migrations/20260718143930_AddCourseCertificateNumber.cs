using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCertificateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CertificateNumber",
                table: "courses",
                type: "int",
                nullable: true,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateNumber",
                table: "courses");
        }
    }
}
