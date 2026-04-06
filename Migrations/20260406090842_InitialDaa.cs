using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialDaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLookupH",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LookupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LookupNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LookupNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLookupH", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "AppLookupD",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LookupHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LookupValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LookupNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LookupNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLookupD", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_AppLookupD_AppLookupH_LookupHeaderId",
                        column: x => x.LookupHeaderId,
                        principalTable: "AppLookupH",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RoleLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_users_AppLookupD_RoleLookupId",
                        column: x => x.RoleLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_AppLookupD_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CourseDescription = table.Column<string>(type: "text", nullable: true),
                    HeaderOne = table.Column<string>(type: "text", nullable: true),
                    HeaderTwo = table.Column<string>(type: "text", nullable: true),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<string>(type: "text", nullable: true),
                    CourseLevelLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseCategoryLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstructorOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_courses", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_courses_AppLookupD_CourseCategoryLookupId",
                        column: x => x.CourseCategoryLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_courses_AppLookupD_CourseLevelLookupId",
                        column: x => x.CourseLevelLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_courses_users_InstructorOid",
                        column: x => x.InstructorOid,
                        principalTable: "users",
                        principalColumn: "Oid");
                });

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
                name: "course_features",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureHeader = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FeatureDescription = table.Column<string>(type: "text", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_features", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_features_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_instructors",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeaderAr = table.Column<string>(type: "text", nullable: true),
                    HeaderEn = table.Column<string>(type: "text", nullable: true),
                    BioEn = table.Column<string>(type: "text", nullable: true),
                    BioAr = table.Column<string>(type: "text", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_instructors", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_instructors_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_Live_Session",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxNumberReservation = table.Column<int>(type: "int", nullable: true),
                    NumberOfReservations = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_Live_Session", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_Live_Session_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_outline",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_outline", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_outline_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_target_audience",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionAr = table.Column<string>(type: "text", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_target_audience", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_target_audience_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_videos",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    DescriptionAr = table.Column<string>(type: "text", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    VideoTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPreview = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_videos", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_videos_AppLookupD_VideoTypeLookupId",
                        column: x => x.VideoTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_videos_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "courses_Master_Exam",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CourseLevelLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseCategoryLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PassPercent = table.Column<int>(type: "int", nullable: true),
                    FreeExam = table.Column<bool>(type: "bit", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    QuestionCount = table.Column<int>(type: "int", nullable: true),
                    MaxAttempts = table.Column<int>(type: "int", nullable: true),
                    ShuffleQuestions = table.Column<bool>(type: "bit", nullable: false),
                    ShuffleAnswers = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_courses_Master_Exam", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_courses_Master_Exam_AppLookupD_CourseCategoryLookupId",
                        column: x => x.CourseCategoryLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_courses_Master_Exam_AppLookupD_CourseLevelLookupId",
                        column: x => x.CourseLevelLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_courses_Master_Exam_courses_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "courses",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
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
                    ExamSimulationReserv = table.Column<bool>(type: "bit", nullable: false),
                    RecordedCourseReserv = table.Column<bool>(type: "bit", nullable: false),
                    LiveCourseReserv = table.Column<bool>(type: "bit", nullable: false),
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
                    ExamSimulationReserv = table.Column<bool>(type: "bit", nullable: false),
                    RecordedCourseReserv = table.Column<bool>(type: "bit", nullable: false),
                    LiveCourseReserv = table.Column<bool>(type: "bit", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "course_Live_Session_Studient",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_Live_Session_Studient", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_Live_Session_Studient_course_Live_Session_CourseOid",
                        column: x => x.CourseOid,
                        principalTable: "course_Live_Session",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_Live_Session_Studient_students_StudentOid",
                        column: x => x.StudentOid,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_contents",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOutlineOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContentOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_contents", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_contents_AppLookupD_ContentTypeLookupId",
                        column: x => x.ContentTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_contents_course_outline_CourseOutlineOid",
                        column: x => x.CourseOutlineOid,
                        principalTable: "course_outline",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_video_attachments",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseVideoOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_video_attachments", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_video_attachments_AppLookupD_FileTypeLookupId",
                        column: x => x.FileTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_video_attachments_course_videos_CourseVideoOid",
                        column: x => x.CourseVideoOid,
                        principalTable: "course_videos",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_questions",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoursesMasterExamOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    QuestionText_Ar = table.Column<string>(type: "text", nullable: false),
                    QuestionExplination = table.Column<string>(type: "text", nullable: false),
                    QuestionTypeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionScore = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CorrectAnswer = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<bool>(type: "bit", nullable: false),
                    CorrectChoiceOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_questions", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_questions_AppLookupD_QuestionTypeLookupId",
                        column: x => x.QuestionTypeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_questions_course_questions_CorrectChoiceOid",
                        column: x => x.CorrectChoiceOid,
                        principalTable: "course_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_questions_courses_Master_Exam_CoursesMasterExamOid",
                        column: x => x.CoursesMasterExamOid,
                        principalTable: "courses_Master_Exam",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_exams",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoursesMasterExamOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNo = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: true),
                    ObtainedScore = table.Column<int>(type: "int", nullable: true),
                    PassPercent = table.Column<int>(type: "int", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: true),
                    ExamStatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExamModeLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exams", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_exams_AppLookupD_ExamModeLookupId",
                        column: x => x.ExamModeLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exams_AppLookupD_ExamStatusLookupId",
                        column: x => x.ExamStatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exams_courses_Master_Exam_CoursesMasterExamOid",
                        column: x => x.CoursesMasterExamOid,
                        principalTable: "courses_Master_Exam",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exams_students_StudentOid",
                        column: x => x.StudentOid,
                        principalTable: "students",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_answers",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    AnswerText_Ar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question_Ask = table.Column<bool>(type: "bit", nullable: false),
                    CorrectAnswerOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_answers", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_course_answers_course_answers_CorrectAnswerOid",
                        column: x => x.CorrectAnswerOid,
                        principalTable: "course_answers",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_course_answers_course_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "course_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_exam_questions",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentExamOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionStatusLookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    QuestionScore = table.Column<int>(type: "int", nullable: true),
                    ObtainedScore = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exam_questions", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_exam_questions_AppLookupD_QuestionStatusLookupId",
                        column: x => x.QuestionStatusLookupId,
                        principalTable: "AppLookupD",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_questions_course_questions_QuestionOid",
                        column: x => x.QuestionOid,
                        principalTable: "course_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_questions_student_exams_StudentExamOid",
                        column: x => x.StudentExamOid,
                        principalTable: "student_exams",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_exam_question_answers",
                columns: table => new
                {
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentExamQuestionOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedAnswerOid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerSelectedAnswerOid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_exam_question_answers", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_course_answers_AnswerSelectedAnswerOid",
                        column: x => x.AnswerSelectedAnswerOid,
                        principalTable: "course_answers",
                        principalColumn: "Oid");
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_course_answers_SelectedAnswerOid",
                        column: x => x.SelectedAnswerOid,
                        principalTable: "course_answers",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_exam_question_answers_student_exam_questions_StudentExamQuestionOid",
                        column: x => x.StudentExamQuestionOid,
                        principalTable: "student_exam_questions",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppLookupH",
                columns: new[] { "Oid", "CreatedAt", "CreatedBy", "DeletedAt", "IsActive", "IsDeleted", "LookupCode", "LookupNameAr", "LookupNameEn", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "COURSE_LEVEL", "مستوى الدورة", "Course Level", null },
                    { new Guid("12121212-1212-1212-1212-121212121212"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "EXAM_MODE", "وضع الامتحان", "Exam Mode", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "COURSE_CATEGORY", "فئة الدورة", "Course Category", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "QUESTION_TYPE", "نوع السؤال", "Question Type", null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "QUESTION_STATUS", "حالة السؤال", "Question Status", null },
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
                    { new Guid("11111111-1111-1111-1111-111111111101"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("11111111-1111-1111-1111-111111111111"), "مبتدئ", "Beginner", "BEGINNER", 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111102"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("11111111-1111-1111-1111-111111111111"), "متوسط", "Intermediate", "INTERMEDIATE", 2, null },
                    { new Guid("11111111-1111-1111-1111-111111111103"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("11111111-1111-1111-1111-111111111111"), "متقدم", "Advanced", "ADVANCED", 3, null },
                    { new Guid("11111111-1111-1111-1111-111111111104"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("11111111-1111-1111-1111-111111111111"), "خبير", "Expert", "EXPERT", 4, null },
                    { new Guid("22222222-2222-2222-2222-222222222201"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "البرمجة", "Programming", "PROGRAMMING", 1, null },
                    { new Guid("22222222-2222-2222-2222-222222222202"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "الطب", "Medical", "MEDICAL", 2, null },
                    { new Guid("22222222-2222-2222-2222-222222222203"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "المالية", "Finance", "FINANCE", 3, null },
                    { new Guid("22222222-2222-2222-2222-222222222204"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "الهندسة", "Engineering", "ENGINEERING", 4, null },
                    { new Guid("22222222-2222-2222-2222-222222222205"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "الأعمال", "Business", "BUSINESS", 5, null },
                    { new Guid("22222222-2222-2222-2222-222222222206"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "التصميم", "Design", "DESIGN", 6, null },
                    { new Guid("22222222-2222-2222-2222-222222222207"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "التسويق", "Marketing", "MARKETING", 7, null },
                    { new Guid("22222222-2222-2222-2222-222222222208"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "اللغة", "Language", "LANGUAGE", 8, null },
                    { new Guid("22222222-2222-2222-2222-222222222209"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "علم البيانات", "Data Science", "DATA_SCIENCE", 9, null },
                    { new Guid("22222222-2222-2222-2222-222222222210"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("22222222-2222-2222-2222-222222222222"), "الأمن السيبراني", "Cybersecurity", "CYBERSECURITY", 10, null },
                    { new Guid("33333333-3333-3333-3333-333333333301"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("33333333-3333-3333-3333-333333333333"), "اختيار من متعدد", "Multiple Choice Question", "MCQ", 1, null },
                    { new Guid("33333333-3333-3333-3333-333333333302"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("33333333-3333-3333-3333-333333333333"), "صح أو خطأ", "True/False", "TRUE_FALSE", 2, null },
                    { new Guid("33333333-3333-3333-3333-333333333306"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("33333333-3333-3333-3333-333333333333"), "مطابقة", "Matching", "MATCHING", 6, null },
                    { new Guid("44444444-4444-4444-4444-444444444401"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "صحيح", "Correct", "CORRECT", 1, null },
                    { new Guid("44444444-4444-4444-4444-444444444402"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "غير صحيح", "Incorrect", "INCORRECT", 2, null },
                    { new Guid("44444444-4444-4444-4444-444444444403"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, new Guid("44444444-4444-4444-4444-444444444444"), "لم يتم الإجابة", "Not Answered", "NOT_ANSWERED", 3, null },
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
                name: "IX_AppLookupD_CreatedAt",
                table: "AppLookupD",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupD_IsDeleted",
                table: "AppLookupD",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupD_IsDeleted_CreatedAt",
                table: "AppLookupD",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupD_LookupHeaderId_IsActive",
                table: "AppLookupD",
                columns: new[] { "LookupHeaderId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupD_LookupHeaderId_LookupValue",
                table: "AppLookupD",
                columns: new[] { "LookupHeaderId", "LookupValue" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupD_Oid",
                table: "AppLookupD",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_CreatedAt",
                table: "AppLookupH",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_IsActive",
                table: "AppLookupH",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_IsDeleted",
                table: "AppLookupH",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_IsDeleted_CreatedAt",
                table: "AppLookupH",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_LookupCode",
                table: "AppLookupH",
                column: "LookupCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppLookupH_Oid",
                table: "AppLookupH",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_CorrectAnswerOid",
                table: "course_answers",
                column: "CorrectAnswerOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_CreatedAt",
                table: "course_answers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_IsDeleted",
                table: "course_answers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_IsDeleted_CreatedAt",
                table: "course_answers",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_IsDeleted_IsCorrect",
                table: "course_answers",
                columns: new[] { "IsDeleted", "IsCorrect" });

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_Oid",
                table: "course_answers",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_OrderNo",
                table: "course_answers",
                column: "OrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_course_answers_QuestionId",
                table: "course_answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_course_contents_ContentTypeLookupId",
                table: "course_contents",
                column: "ContentTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_course_contents_CourseOutlineOid",
                table: "course_contents",
                column: "CourseOutlineOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_contents_IsDeleted",
                table: "course_contents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_features_CourseOid",
                table: "course_features",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_features_IsDeleted",
                table: "course_features",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_instructors_CourseOid",
                table: "course_instructors",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_instructors_IsDeleted",
                table: "course_instructors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_CourseOid",
                table: "course_Live_Session",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_Date",
                table: "course_Live_Session",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_IsDeleted_Active",
                table: "course_Live_Session",
                columns: new[] { "IsDeleted", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_Studient_CourseOid",
                table: "course_Live_Session_Studient",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_Studient_CourseOid_StudentOid",
                table: "course_Live_Session_Studient",
                columns: new[] { "CourseOid", "StudentOid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_Studient_IsDeleted",
                table: "course_Live_Session_Studient",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_Live_Session_Studient_StudentOid",
                table: "course_Live_Session_Studient",
                column: "StudentOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_outline_CourseOid",
                table: "course_outline",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_outline_IsDeleted",
                table: "course_outline",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_CorrectChoiceOid",
                table: "course_questions",
                column: "CorrectChoiceOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_CoursesMasterExamOid",
                table: "course_questions",
                column: "CoursesMasterExamOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_CreatedAt",
                table: "course_questions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_IsDeleted",
                table: "course_questions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_IsDeleted_CreatedAt",
                table: "course_questions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_IsDeleted_IsActive",
                table: "course_questions",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_Oid",
                table: "course_questions",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_OrderNo",
                table: "course_questions",
                column: "OrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_course_questions_QuestionTypeLookupId",
                table: "course_questions",
                column: "QuestionTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_course_target_audience_CourseOid",
                table: "course_target_audience",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_target_audience_IsDeleted",
                table: "course_target_audience",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_video_attachments_CourseVideoOid",
                table: "course_video_attachments",
                column: "CourseVideoOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_video_attachments_FileTypeLookupId",
                table: "course_video_attachments",
                column: "FileTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_course_video_attachments_IsDeleted",
                table: "course_video_attachments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_course_videos_CourseOid",
                table: "course_videos",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_course_videos_IsDeleted_IsActive",
                table: "course_videos",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_course_videos_VideoTypeLookupId",
                table: "course_videos",
                column: "VideoTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_CourseCategoryLookupId",
                table: "courses",
                column: "CourseCategoryLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_CourseCode",
                table: "courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_CourseLevelLookupId",
                table: "courses",
                column: "CourseLevelLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_CreatedAt",
                table: "courses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_courses_InstructorOid",
                table: "courses",
                column: "InstructorOid");

            migrationBuilder.CreateIndex(
                name: "IX_courses_IsDeleted",
                table: "courses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_courses_IsDeleted_CreatedAt",
                table: "courses",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_courses_IsDeleted_IsActive",
                table: "courses",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_courses_Oid",
                table: "courses",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_CourseCategoryLookupId",
                table: "courses_Master_Exam",
                column: "CourseCategoryLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_CourseLevelLookupId",
                table: "courses_Master_Exam",
                column: "CourseLevelLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_CourseOid",
                table: "courses_Master_Exam",
                column: "CourseOid");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_CreatedAt",
                table: "courses_Master_Exam",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_IsDeleted",
                table: "courses_Master_Exam",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_IsDeleted_CreatedAt",
                table: "courses_Master_Exam",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_IsDeleted_IsActive",
                table: "courses_Master_Exam",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_courses_Master_Exam_Oid",
                table: "courses_Master_Exam",
                column: "Oid");

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

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_AnswerSelectedAnswerOid",
                table: "student_exam_question_answers",
                column: "AnswerSelectedAnswerOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_IsDeleted",
                table: "student_exam_question_answers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_SelectedAnswerOid",
                table: "student_exam_question_answers",
                column: "SelectedAnswerOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_question_answers_StudentExamQuestionOid",
                table: "student_exam_question_answers",
                column: "StudentExamQuestionOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_IsDeleted",
                table: "student_exam_questions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_QuestionOid",
                table: "student_exam_questions",
                column: "QuestionOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_QuestionStatusLookupId",
                table: "student_exam_questions",
                column: "QuestionStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_StudentExamOid",
                table: "student_exam_questions",
                column: "StudentExamOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exam_questions_StudentExamOid_QuestionOid",
                table: "student_exam_questions",
                columns: new[] { "StudentExamOid", "QuestionOid" });

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_CoursesMasterExamOid",
                table: "student_exams",
                column: "CoursesMasterExamOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_ExamModeLookupId",
                table: "student_exams",
                column: "ExamModeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_ExamStatusLookupId",
                table: "student_exams",
                column: "ExamStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_IsDeleted",
                table: "student_exams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_StudentOid",
                table: "student_exams",
                column: "StudentOid");

            migrationBuilder.CreateIndex(
                name: "IX_student_exams_StudentOid_CoursesMasterExamOid",
                table: "student_exams",
                columns: new[] { "StudentOid", "CoursesMasterExamOid" });

            migrationBuilder.CreateIndex(
                name: "IX_students_Email",
                table: "students",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_students_IsDeleted",
                table: "students",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_students_IsDeleted_IsActive",
                table: "students",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_students_Username",
                table: "students",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_IsDeleted",
                table: "users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_users_IsDeleted_IsActive",
                table: "users",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleLookupId",
                table: "users",
                column: "RoleLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_users_StatusLookupId",
                table: "users",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_contents");

            migrationBuilder.DropTable(
                name: "course_features");

            migrationBuilder.DropTable(
                name: "course_instructors");

            migrationBuilder.DropTable(
                name: "course_Live_Session_Studient");

            migrationBuilder.DropTable(
                name: "course_target_audience");

            migrationBuilder.DropTable(
                name: "course_video_attachments");

            migrationBuilder.DropTable(
                name: "service_contact_us");

            migrationBuilder.DropTable(
                name: "student_baskets");

            migrationBuilder.DropTable(
                name: "student_courses");

            migrationBuilder.DropTable(
                name: "student_exam_question_answers");

            migrationBuilder.DropTable(
                name: "course_outline");

            migrationBuilder.DropTable(
                name: "course_Live_Session");

            migrationBuilder.DropTable(
                name: "course_videos");

            migrationBuilder.DropTable(
                name: "course_answers");

            migrationBuilder.DropTable(
                name: "student_exam_questions");

            migrationBuilder.DropTable(
                name: "course_questions");

            migrationBuilder.DropTable(
                name: "student_exams");

            migrationBuilder.DropTable(
                name: "courses_Master_Exam");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "AppLookupD");

            migrationBuilder.DropTable(
                name: "AppLookupH");
        }
    }
}
