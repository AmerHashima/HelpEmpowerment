using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class testexam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamModeLookupId",
                table: "student_exams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AppLookupH",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupCode", "LookupNameAr", "LookupNameEn", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("12121212-1212-1212-1212-121212121212"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "EXAM_MODE", "وضع الامتحان", "Exam Mode", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "USER_ROLE", "دور المستخدم", "User Role", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "USER_STATUS", "حالة المستخدم", "User Status", null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "EXAM_STATUS", "حالة الامتحان", "Exam Status", null },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "PAYMENT_STATUS", "حالة الدفع", "Payment Status", null },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "ENROLLMENT_STATUS", "حالة التسجيل", "Enrollment Status", null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "CONTENT_TYPE", "نوع المحتوى", "Content Type", null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "VIDEO_TYPE", "نوع الفيديو", "Video Type", null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "FILE_TYPE", "نوع الملف", "File Type", null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "BASKET_STATUS", "حالة السلة", "Basket Status", null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "CONTACT_TYPE", "نوع الاتصال", "Contact Type", null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "CONTACT_STATUS", "حالة الاتصال", "Contact Status", null }
                });

            migrationBuilder.InsertData(
                table: "AppLookupD",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupHeaderId", "LookupNameAr", "LookupNameEn", "LookupValue", "OrderNo", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555501"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("55555555-5555-5555-5555-555555555555"), "مدير النظام", "Admin", "ADMIN", 1, null },
                    { new Guid("55555555-5555-5555-5555-555555555502"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("55555555-5555-5555-5555-555555555555"), "مدرب", "Instructor", "INSTRUCTOR", 2, null },
                    { new Guid("55555555-5555-5555-5555-555555555503"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("55555555-5555-5555-5555-555555555555"), "طالب", "Student", "STUDENT", 3, null },
                    { new Guid("55555555-5555-5555-5555-555555555504"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("55555555-5555-5555-5555-555555555555"), "الدعم الفني", "Support", "SUPPORT", 4, null },
                    { new Guid("66666666-6666-6666-6666-666666666601"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("66666666-6666-6666-6666-666666666666"), "نشط", "Active", "ACTIVE", 1, null },
                    { new Guid("66666666-6666-6666-6666-666666666602"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("66666666-6666-6666-6666-666666666666"), "غير نشط", "Inactive", "INACTIVE", 2, null },
                    { new Guid("66666666-6666-6666-6666-666666666603"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("66666666-6666-6666-6666-666666666666"), "معلق", "Suspended", "SUSPENDED", 3, null },
                    { new Guid("66666666-6666-6666-6666-666666666604"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("66666666-6666-6666-6666-666666666666"), "قيد الانتظار", "Pending", "PENDING", 4, null },
                    { new Guid("77777777-7777-7777-7777-777777777701"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("77777777-7777-7777-7777-777777777777"), "لم يبدأ", "Not Started", "NOT_STARTED", 1, null },
                    { new Guid("77777777-7777-7777-7777-777777777702"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("77777777-7777-7777-7777-777777777777"), "قيد التنفيذ", "In Progress", "IN_PROGRESS", 2, null },
                    { new Guid("77777777-7777-7777-7777-777777777703"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("77777777-7777-7777-7777-777777777777"), "تم التقديم", "Submitted", "SUBMITTED", 3, null },
                    { new Guid("77777777-7777-7777-7777-777777777704"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("77777777-7777-7777-7777-777777777777"), "تمت المراجعة", "Reviewed", "REVIEWED", 4, null },
                    { new Guid("77777777-7777-7777-7777-777777777705"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("77777777-7777-7777-7777-777777777777"), "تم الحذف", "Deleted", "DELETED", 5, null },
                    { new Guid("88888888-8888-8888-8888-888888888801"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("88888888-8888-8888-8888-888888888888"), "قيد الانتظار", "Pending", "PENDING", 1, null },
                    { new Guid("88888888-8888-8888-8888-888888888802"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("88888888-8888-8888-8888-888888888888"), "مدفوع", "Paid", "PAID", 2, null },
                    { new Guid("88888888-8888-8888-8888-888888888803"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("88888888-8888-8888-8888-888888888888"), "فشل", "Failed", "FAILED", 3, null },
                    { new Guid("88888888-8888-8888-8888-888888888804"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("88888888-8888-8888-8888-888888888888"), "مسترد", "Refunded", "REFUNDED", 4, null },
                    { new Guid("99999999-9999-9999-9999-999999999901"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("99999999-9999-9999-9999-999999999999"), "نشط", "Active", "ACTIVE", 1, null },
                    { new Guid("99999999-9999-9999-9999-999999999902"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("99999999-9999-9999-9999-999999999999"), "منتهي", "Expired", "EXPIRED", 2, null },
                    { new Guid("99999999-9999-9999-9999-999999999903"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("99999999-9999-9999-9999-999999999999"), "معلق", "Suspended", "SUSPENDED", 3, null },
                    { new Guid("99999999-9999-9999-9999-999999999904"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("99999999-9999-9999-9999-999999999999"), "مكتمل", "Completed", "COMPLETED", 4, null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "فيديو", "Video", "VIDEO", 1, null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "بي دي إف", "PDF", "PDF", 2, null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "اختبار", "Quiz", "QUIZ", 3, null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "واجب", "Assignment", "ASSIGNMENT", 4, null },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "مقال", "Article", "ARTICLE", 5, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "مقدمة", "Introduction", "INTRODUCTION", 1, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "محاضرة", "Lecture", "LECTURE", 2, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "درس تعليمي", "Tutorial", "TUTORIAL", 3, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "عرض توضيحي", "Demo", "DEMO", 4, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccc01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "بي دي إف", "PDF", "PDF", 1, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccc02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "وورد", "Word Document", "DOCX", 2, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccc03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "باوربوينت", "PowerPoint", "PPTX", 3, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccc04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "ملف مضغوط", "ZIP Archive", "ZIP", 4, null },
                    { new Guid("dddddddd-dddd-dddd-1212-dddddddddd01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("12121212-1212-1212-1212-121212121212"), "وضع الممارسة", "Practice Mode", "PRACTICE_MODE", 1, null },
                    { new Guid("dddddddd-dddd-dddd-1212-dddddddddd02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("12121212-1212-1212-1212-121212121212"), "وضع الامتحان", "Exam Mode", "EXAM_MODE", 2, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddd01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "نشط", "Active", "ACTIVE", 1, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddd02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "تم الدفع", "Checked Out", "CHECKED_OUT", 2, null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddd03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "متروك", "Abandoned", "ABANDONED", 3, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "دعم فني", "Technical Support", "TECHNICAL_SUPPORT", 1, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "الفوترة", "Billing", "BILLING", 2, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "استفسار عام", "General Inquiry", "GENERAL_INQUIRY", 3, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "شكوى", "Complaint", "COMPLAINT", 4, null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffff01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "جديد", "New", "NEW", 1, null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffff02"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "قيد المعالجة", "In Progress", "IN_PROGRESS", 2, null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffff03"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "تم الحل", "Resolved", "RESOLVED", 3, null },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffff04"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "مغلق", "Closed", "CLOSED", 4, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_ExamModeLookupId",
                table: "student_exams",
                column: "ExamModeLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions",
                column: "QuestionStatusLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams",
                column: "ExamModeLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                table: "student_exams");

            migrationBuilder.DropIndex(
                name: "IX_student_exams_ExamModeLookupId",
                table: "student_exams");

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("55555555-5555-5555-5555-555555555502"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("55555555-5555-5555-5555-555555555503"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("55555555-5555-5555-5555-555555555504"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("66666666-6666-6666-6666-666666666601"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("66666666-6666-6666-6666-666666666602"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("66666666-6666-6666-6666-666666666603"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("66666666-6666-6666-6666-666666666604"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("88888888-8888-8888-8888-888888888801"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("88888888-8888-8888-8888-888888888802"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("88888888-8888-8888-8888-888888888803"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("88888888-8888-8888-8888-888888888804"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("99999999-9999-9999-9999-999999999901"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("99999999-9999-9999-9999-999999999902"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("99999999-9999-9999-9999-999999999903"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("99999999-9999-9999-9999-999999999904"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccc01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccc02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccc03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccc04"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-1212-dddddddddd01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-1212-dddddddddd02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddd01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddd02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddd03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffff01"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffff02"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffff03"));

            migrationBuilder.DeleteData(
                table: "AppLookupD",
                keyColumn: "Oid",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffff04"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("12121212-1212-1212-1212-121212121212"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "AppLookupH",
                keyColumn: "Oid",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));

            migrationBuilder.DropColumn(
                name: "ExamModeLookupId",
                table: "student_exams");

            migrationBuilder.AddForeignKey(
                name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                table: "student_exam_questions",
                column: "QuestionStatusLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid");
        }
    }
}
