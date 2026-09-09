using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAssignmentsAndRevenueSharing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_revenue_shares",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShareTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_revenue_shares", x => x.Oid);
                    table.CheckConstraint("CK_CourseRevenueShare_Dates", "[EffectiveTo] IS NULL OR [EffectiveFrom] IS NULL OR [EffectiveFrom] <= [EffectiveTo]");
                    table.CheckConstraint("CK_CourseRevenueShare_Percentage", "[CalculationType] <> 'Percentage' OR [Value] <= 100");
                    table.CheckConstraint("CK_CourseRevenueShare_Value", "[Value] >= 0");
                    table.ForeignKey(
                        name: "FK_course_revenue_shares_AppLookupD_ShareTypeLookupId",
                        column: x => x.ShareTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_shares_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_shares_users_BeneficiaryUserId",
                        column: x => x.BeneficiaryUserId,
                        principalTable: "users",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "revenue_settlements",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettlementNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revenue_settlements", x => x.Oid);
                    table.CheckConstraint("CK_RevenueSettlement_Period", "[PeriodFrom] <= [PeriodTo]");
                    table.ForeignKey(
                        name: "FK_revenue_settlements_users_BeneficiaryUserId",
                        column: x => x.BeneficiaryUserId,
                        principalTable: "users",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_course_assignments",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_user_course_assignments", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_user_course_assignments_AppLookupD_AssignmentTypeLookupId",
                        column: x => x.AssignmentTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_course_assignments_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_course_assignments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_revenue_distributions",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenueShareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShareTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppliedPercentage = table.Column<decimal>(type: "decimal(9,4)", nullable: true),
                    AppliedFixedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShareAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_revenue_distributions", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_AppLookupD_ShareTypeLookupId",
                        column: x => x.ShareTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_InvoiceItems_InvoiceItemId",
                        column: x => x.InvoiceItemId,
                        principalTable: "InvoiceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_course_revenue_shares_RevenueShareId",
                        column: x => x.RevenueShareId,
                        principalTable: "course_revenue_shares",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_revenue_settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "revenue_settlements",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_revenue_distributions_users_BeneficiaryUserId",
                        column: x => x.BeneficiaryUserId,
                        principalTable: "users",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppLookupH",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupCode", "LookupNameAr", "LookupNameEn", "UpdatedAt" },
                values: new object[] { new Guid("14141414-1414-1414-1414-141414141414"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "COURSE_ASSIGNMENT_TYPE", "نوع التكليف بالدورة", "Course Assignment Type", null });

            migrationBuilder.InsertData(
                table: "links",
                columns: new[] { "Oid", "Active", "CreatedAt", "CreatedBy", "DeletedAt", "Icon", "IsActive", "IsDeleted", "NameAr", "NameEn", "Path", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("15151515-1515-1515-1515-151515151501"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "Courses.AssignUsers", "Courses.AssignUsers", "Courses.AssignUsers", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151502"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "Courses.ViewAssignedUsers", "Courses.ViewAssignedUsers", "Courses.ViewAssignedUsers", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151503"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueShares.View", "RevenueShares.View", "RevenueShares.View", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151504"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueShares.Manage", "RevenueShares.Manage", "RevenueShares.Manage", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151505"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueDistributions.View", "RevenueDistributions.View", "RevenueDistributions.View", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151506"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueSettlements.View", "RevenueSettlements.View", "RevenueSettlements.View", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151507"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueSettlements.Manage", "RevenueSettlements.Manage", "RevenueSettlements.Manage", null, null, null },
                    { new Guid("15151515-1515-1515-1515-151515151508"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, false, "RevenueDashboard.View", "RevenueDashboard.View", "RevenueDashboard.View", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "AppLookupD",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupHeaderId", "LookupNameAr", "LookupNameEn", "LookupValue", "OrderNo", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("14141414-1414-1414-1414-141414141401"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "المالك", "Owner", "OWNER", 1, null },
                    { new Guid("14141414-1414-1414-1414-141414141402"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "المدرب", "Trainer", "TRAINER", 2, null },
                    { new Guid("14141414-1414-1414-1414-141414141403"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "المدرب المساعد", "Assistant Trainer", "ASSISTANT_TRAINER", 3, null },
                    { new Guid("14141414-1414-1414-1414-141414141404"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "التسويق", "Marketing", "MARKETING", 4, null },
                    { new Guid("14141414-1414-1414-1414-141414141405"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "المبيعات", "Sales", "SALES", 5, null },
                    { new Guid("14141414-1414-1414-1414-141414141406"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "العمليات", "Operations", "OPERATIONS", 6, null },
                    { new Guid("14141414-1414-1414-1414-141414141407"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "أخرى", "Other", "OTHER", 7, null },
                    { new Guid("14141414-1414-1414-1414-141414141408"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("14141414-1414-1414-1414-141414141414"), "المنصة", "Platform", "PLATFORM", 8, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_BeneficiaryUserId_Status_CreatedAt",
                table: "course_revenue_distributions",
                columns: new[] { "BeneficiaryUserId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_CourseId",
                table: "course_revenue_distributions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_InvoiceId",
                table: "course_revenue_distributions",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_InvoiceItemId",
                table: "course_revenue_distributions",
                column: "InvoiceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_PaymentTransactionId_InvoiceItemId_RevenueShareId",
                table: "course_revenue_distributions",
                columns: new[] { "PaymentTransactionId", "InvoiceItemId", "RevenueShareId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_RevenueShareId",
                table: "course_revenue_distributions",
                column: "RevenueShareId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_SettlementId",
                table: "course_revenue_distributions",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_distributions_ShareTypeLookupId",
                table: "course_revenue_distributions",
                column: "ShareTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_shares_BeneficiaryUserId",
                table: "course_revenue_shares",
                column: "BeneficiaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_shares_CourseId",
                table: "course_revenue_shares",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_shares_CourseId_BeneficiaryUserId_ShareTypeLookupId",
                table: "course_revenue_shares",
                columns: new[] { "CourseId", "BeneficiaryUserId", "ShareTypeLookupId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_course_revenue_shares_ShareTypeLookupId",
                table: "course_revenue_shares",
                column: "ShareTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_revenue_settlements_BeneficiaryUserId_Status",
                table: "revenue_settlements",
                columns: new[] { "BeneficiaryUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_revenue_settlements_SettlementNumber",
                table: "revenue_settlements",
                column: "SettlementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_course_assignments_AssignmentTypeLookupId",
                table: "user_course_assignments",
                column: "AssignmentTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_user_course_assignments_CourseId",
                table: "user_course_assignments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_user_course_assignments_UserId",
                table: "user_course_assignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_course_assignments_UserId_CourseId_AssignmentTypeLookupId",
                table: "user_course_assignments",
                columns: new[] { "UserId", "CourseId", "AssignmentTypeLookupId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_revenue_distributions");

            migrationBuilder.DropTable(
                name: "user_course_assignments");

            migrationBuilder.DropTable(
                name: "course_revenue_shares");

            migrationBuilder.DropTable(
                name: "revenue_settlements");

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141401"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141402"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141403"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141404"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141405"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141406"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141407"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141408"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151501"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151502"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151503"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151504"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151505"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151506"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151507"));

            migrationBuilder.DeleteData(
                table: "links",
                keyColumn: "Oid",
                keyValue: new Guid("15151515-1515-1515-1515-151515151508"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("14141414-1414-1414-1414-141414141414"));
        }
    }
}
