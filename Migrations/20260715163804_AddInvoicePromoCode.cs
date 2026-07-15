using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicePromoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "Invoices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PromoCode",
                table: "Invoices",
                column: "PromoCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_PromoCode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "Invoices");
        }
    }
}
