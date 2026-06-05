using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeLookupSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppLookupH",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupCode", "LookupNameAr", "LookupNameEn", "UpdatedAt" },
                values: new object[] { new Guid("13131313-1313-1313-1313-131313131313"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "SERVICE_TYPE", "نوع الخدمة", "Service Type", null });

            migrationBuilder.InsertData(
                table: "AppLookupD",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupHeaderId", "LookupNameAr", "LookupNameEn", "LookupValue", "OrderNo", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("13131313-1313-1313-1313-131313131301"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("13131313-1313-1313-1313-131313131313"), "محاكاة الاختبار", "Exam Simulation", "EXAM_SIMULATION", 1, null },
                    { new Guid("13131313-1313-1313-1313-131313131302"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("13131313-1313-1313-1313-131313131313"), "دورة مسجلة", "Recorded Course", "RECORDED_COURSE", 2, null },
                    { new Guid("13131313-1313-1313-1313-131313131303"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("13131313-1313-1313-1313-131313131313"), "دورة مباشرة", "Live Course", "LIVE_COURSE", 3, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("13131313-1313-1313-1313-131313131301"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("13131313-1313-1313-1313-131313131302"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("13131313-1313-1313-1313-131313131303"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("13131313-1313-1313-1313-131313131313"));
        }
    }
}
