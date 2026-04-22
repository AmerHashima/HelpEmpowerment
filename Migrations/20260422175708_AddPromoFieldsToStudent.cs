using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoFieldsToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PromoDiscount",
                table: "students",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TotalMoneyWithPromo",
                table: "students",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "UsersUsedPromo",
                table: "students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "students");

            migrationBuilder.DropColumn(
                name: "PromoDiscount",
                table: "students");

            migrationBuilder.DropColumn(
                name: "TotalMoneyWithPromo",
                table: "students");

            migrationBuilder.DropColumn(
                name: "UsersUsedPromo",
                table: "students");
        }
    }
}
