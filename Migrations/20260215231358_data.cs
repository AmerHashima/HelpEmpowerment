using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "service_contact_us",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubjectAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PriorityLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TicketNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_contact_us", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_service_contact_us_AppLookupD_ContactTypeLookupId",
                        column: x => x.ContactTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_service_contact_us_AppLookupD_PriorityLookupId",
                        column: x => x.PriorityLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_service_contact_us_AppLookupD_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_service_contact_us_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_contact_us_users_RespondedBy",
                        column: x => x.RespondedBy,
                        principalTable: "users",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_service_contact_us_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Oid");
                });

            migrationBuilder.CreateTable(
                name: "student_baskets",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasketStatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_baskets", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_baskets_AppLookupD_BasketStatusLookupId",
                        column: x => x.BasketStatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_student_baskets_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_baskets_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_courses",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentStatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnrollmentStatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    CompletedLessons = table.Column<int>(type: "int", nullable: false),
                    TotalLessons = table.Column<int>(type: "int", nullable: false),
                    IsCertificateIssued = table.Column<bool>(type: "bit", nullable: false),
                    CertificateIssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_student_courses", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_courses_AppLookupD_EnrollmentStatusLookupId",
                        column: x => x.EnrollmentStatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_student_courses_AppLookupD_PaymentStatusLookupId",
                        column: x => x.PaymentStatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_student_courses_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_courses_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_ContactTypeLookupId",
                table: "service_contact_us",
                column: "ContactTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_Email",
                table: "service_contact_us",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_PriorityLookupId",
                table: "service_contact_us",
                column: "PriorityLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_RespondedBy",
                table: "service_contact_us",
                column: "RespondedBy");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_StatusLookupId",
                table: "service_contact_us",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_StudentId",
                table: "service_contact_us",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_TicketNumber",
                table: "service_contact_us",
                column: "TicketNumber",
                unique: true,
                filter: "[TicketNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_service_contact_us_UserId",
                table: "service_contact_us",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_student_baskets_BasketStatusLookupId",
                table: "student_baskets",
                column: "BasketStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_baskets_CourseId",
                table: "student_baskets",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_student_baskets_StudentId_CourseId",
                table: "student_baskets",
                columns: new[] { "StudentId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_CourseId",
                table: "student_courses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_EnrollmentStatusLookupId",
                table: "student_courses",
                column: "EnrollmentStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_PaymentStatusLookupId",
                table: "student_courses",
                column: "PaymentStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_StudentId_CourseId",
                table: "student_courses",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_courses_TransactionId",
                table: "student_courses",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_contact_us");

            migrationBuilder.DropTable(
                name: "student_baskets");

            migrationBuilder.DropTable(
                name: "student_courses");
        }
    }
}
