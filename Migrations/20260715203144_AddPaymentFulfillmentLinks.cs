using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFulfillmentLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceItemId",
                table: "student_courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BasketItemId",
                table: "InvoiceItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_InvoiceItemId",
                table: "student_courses",
                column: "InvoiceItemId",
                unique: true,
                filter: "[InvoiceItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_BasketItemId",
                table: "InvoiceItems",
                column: "BasketItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_student_courses_InvoiceItemId",
                table: "student_courses");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_BasketItemId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "InvoiceItemId",
                table: "student_courses");

            migrationBuilder.DropColumn(
                name: "BasketItemId",
                table: "InvoiceItems");
        }
    }
}
