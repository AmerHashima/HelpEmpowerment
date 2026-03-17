using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for Course Examination System
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursesMasterExam> CoursesMasterExams { get; set; }
        public DbSet<CourseQuestion> CourseQuestions { get; set; }
        public DbSet<CourseAnswer> CourseAnswers { get; set; }
        public DbSet<AppLookupHeader> AppLookupHeaders { get; set; }
        public DbSet<AppLookupDetail> AppLookupDetails { get; set; }

        // NEW DbSets - AUTH & USERS
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }

        // NEW DbSets - COURSE FEATURES & CONTENT
        public DbSet<CourseFeature> CourseFeatures { get; set; }
        public DbSet<CourseOutline> CourseOutlines { get; set; }
        public DbSet<CourseContent> CourseContents { get; set; }
        public DbSet<CourseVideo> CourseVideos { get; set; }
        public DbSet<CourseVideoAttachment> CourseVideoAttachments { get; set; }

        // NEW DbSets - STUDENT EXAMS
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentExamQuestion> StudentExamQuestions { get; set; }
        public DbSet<StudentExamQuestionAnswer> StudentExamQuestionAnswers { get; set; }

        // NEW DbSets - LIVE SESSIONS
        public DbSet<CourseLiveSession> CourseLiveSessions { get; set; }
        public DbSet<CourseLiveSessionStudent> CourseLiveSessionStudents { get; set; }

        // NEW DbSets - INSTRUCTORS & TARGET AUDIENCE
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<CourseTargetAudience> CourseTargetAudiences { get; set; }

        // Add DbSets
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentBasket> StudentBaskets { get; set; }
        public DbSet<ServiceContactUs> ServiceContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.CourseCode).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });

                // Validate Course Level Lookup
                entity.HasOne(c => c.CourseLevelLookup)
                    .WithMany()
                    .HasForeignKey(c => c.CourseLevelLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Validate Course Category Lookup
                entity.HasOne(c => c.CourseCategoryLookup)
                    .WithMany()
                    .HasForeignKey(c => c.CourseCategoryLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            // Configure AppLookupHeader
            modelBuilder.Entity<AppLookupHeader>(entity =>
            {
                entity.HasIndex(e => e.LookupCode).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // Configure AppLookupDetail
            modelBuilder.Entity<AppLookupDetail>(entity =>
            {
                entity.HasOne(d => d.LookupHeader)
                    .WithMany(h => h.LookupDetails)
                    .HasForeignKey(d => d.LookupHeaderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => new { e.LookupHeaderId, e.IsActive });
                entity.HasIndex(e => new { e.LookupHeaderId, e.LookupValue });
            });

            // Configure CoursesMasterExam
            modelBuilder.Entity<CoursesMasterExam>(entity =>
            {
                entity.HasOne(e => e.Course)
                    .WithMany(c => c.MasterExams)
                    .HasForeignKey(e => e.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                // Validate Course Level Lookup
                entity.HasOne(e => e.CourseLevelLookup)
                    .WithMany()
                    .HasForeignKey(e => e.CourseLevelLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Validate Course Category Lookup
                entity.HasOne(e => e.CourseCategoryLookup)
                    .WithMany()
                    .HasForeignKey(e => e.CourseCategoryLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });
            });

            // Configure CourseQuestion
            modelBuilder.Entity<CourseQuestion>(entity =>
            {
                entity.HasOne(q => q.MasterExam)
                    .WithMany(e => e.Questions)
                    .HasForeignKey(q => q.CoursesMasterExamOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                // Validate Question Type Lookup
                entity.HasOne(q => q.QuestionTypeLookup)
                    .WithMany()
                    .HasForeignKey(q => q.QuestionTypeLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Self-referencing relationship for correct choice
                entity.HasOne(q => q.CorrectChoice)
                    .WithMany()
                    .HasForeignKey(q => q.CorrectChoiceOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.CoursesMasterExamOid);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });
                entity.HasIndex(e => e.OrderNo);
            });

            // Configure CourseAnswer
            modelBuilder.Entity<CourseAnswer>(entity =>
            {
                entity.HasOne(a => a.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);

                entity.HasIndex(e => e.QuestionId);
                entity.HasIndex(e => new { e.IsDeleted, e.IsCorrect });
                entity.HasIndex(e => e.OrderNo);
            });

            // Seed Initial Data
            SeedLookupData(modelBuilder);

            // Configure base entity indexes
            ConfigureBaseEntityIndexes<Course>(modelBuilder);
            ConfigureBaseEntityIndexes<CoursesMasterExam>(modelBuilder);
            ConfigureBaseEntityIndexes<CourseQuestion>(modelBuilder);
            ConfigureBaseEntityIndexes<CourseAnswer>(modelBuilder);
            ConfigureBaseEntityIndexes<AppLookupHeader>(modelBuilder);
            ConfigureBaseEntityIndexes<AppLookupDetail>(modelBuilder);

            // ###################################
            // NEW CONFIGURATIONS
            // ###################################

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });

                entity.HasOne(u => u.RoleLookup)
                    .WithMany()
                    .HasForeignKey(u => u.RoleLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(u => u.StatusLookup)
                    .WithMany()
                    .HasForeignKey(u => u.StatusLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            // Configure Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });
            });

            // Configure CourseFeature
            modelBuilder.Entity<CourseFeature>(entity =>
            {
                entity.HasOne(cf => cf.Course)
                    .WithMany(c => c.Features)
                    .HasForeignKey(cf => cf.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseOutline
            modelBuilder.Entity<CourseOutline>(entity =>
            {
                entity.HasOne(co => co.Course)
                    .WithMany(c => c.Outlines)
                    .HasForeignKey(co => co.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseContent
            modelBuilder.Entity<CourseContent>(entity =>
            {
                entity.HasOne(cc => cc.CourseOutline)
                    .WithMany(co => co.Contents)
                    .HasForeignKey(cc => cc.CourseOutlineOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(cc => cc.ContentTypeLookup)
                    .WithMany()
                    .HasForeignKey(cc => cc.ContentTypeLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.CourseOutlineOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseVideo
            modelBuilder.Entity<CourseVideo>(entity =>
            {
                entity.HasOne(cv => cv.Course)
                    .WithMany(c => c.Videos)
                    .HasForeignKey(cv => cv.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(cv => cv.VideoTypeLookup)
                    .WithMany()
                    .HasForeignKey(cv => cv.VideoTypeLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => new { e.IsDeleted, e.IsActive });
            });

            // Configure CourseVideoAttachment
            modelBuilder.Entity<CourseVideoAttachment>(entity =>
            {
                entity.HasOne(cva => cva.CourseVideo)
                    .WithMany(cv => cv.Attachments)
                    .HasForeignKey(cva => cva.CourseVideoOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(cva => cva.FileTypeLookup)
                    .WithMany()
                    .HasForeignKey(cva => cva.FileTypeLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.CourseVideoOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure StudentExam
            modelBuilder.Entity<StudentExam>(entity =>
            {
                entity.HasOne(se => se.Student)
                    .WithMany(s => s.StudentExams)
                    .HasForeignKey(se => se.StudentOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(se => se.MasterExam)
                    .WithMany()
                    .HasForeignKey(se => se.CoursesMasterExamOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(se => se.ExamStatusLookup)
                    .WithMany()
                    .HasForeignKey(se => se.ExamStatusLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.StudentOid);
                entity.HasIndex(e => e.CoursesMasterExamOid);
                entity.HasIndex(e => new { e.StudentOid, e.CoursesMasterExamOid });
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure StudentExamQuestion
            modelBuilder.Entity<StudentExamQuestion>(entity =>
            {
                entity.HasOne(seq => seq.StudentExam)
                    .WithMany(se => se.ExamQuestions)
                    .HasForeignKey(seq => seq.StudentExamOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(seq => seq.Question)
                    .WithMany()
                    .HasForeignKey(seq => seq.QuestionOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(seq => seq.QuestionStatus)
                    .WithMany()
                    .HasForeignKey(seq => seq.QuestionStatusLookupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.StudentExamOid);
                entity.HasIndex(e => e.QuestionOid);
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.StudentExamOid, e.QuestionOid });
            });

            // Configure StudentExamQuestionAnswer
            modelBuilder.Entity<StudentExamQuestionAnswer>(entity =>
            {
                entity.HasOne(a => a.StudentExamQuestion)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.StudentExamQuestionOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(a => a.SelectedAnswer)
                    .WithMany()
                    .HasForeignKey(a => a.SelectedAnswerOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.StudentExamQuestionOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseLiveSession
            modelBuilder.Entity<CourseLiveSession>(entity =>
            {
                entity.HasOne(cls => cls.Course)
                    .WithMany(c => c.LiveSessions)
                    .HasForeignKey(cls => cls.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.IsDeleted, e.Active });
            });

            // Configure CourseLiveSessionStudent
            modelBuilder.Entity<CourseLiveSessionStudent>(entity =>
            {
                entity.HasOne(clss => clss.LiveSession)
                    .WithMany(cls => cls.SessionStudents)
                    .HasForeignKey(clss => clss.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasOne(clss => clss.Student)
                    .WithMany(s => s.LiveSessionEnrollments)
                    .HasForeignKey(clss => clss.StudentOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.StudentOid);
                entity.HasIndex(e => new { e.CourseOid, e.StudentOid }).IsUnique();
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseInstructor
            modelBuilder.Entity<CourseInstructor>(entity =>
            {
                entity.HasOne(ci => ci.Course)
                    .WithMany(c => c.Instructors)
                    .HasForeignKey(ci => ci.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // Configure CourseTargetAudience
            modelBuilder.Entity<CourseTargetAudience>(entity =>
            {
                entity.HasOne(cta => cta.Course)
                    .WithMany(c => c.TargetAudiences)
                    .HasForeignKey(cta => cta.CourseOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(true);

                entity.HasIndex(e => e.CourseOid);
                entity.HasIndex(e => e.IsDeleted);
            });

            // ✅ StudentCourse configuration
            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasOne(sc => sc.Student)
                    .WithMany(s => s.EnrolledCourses)
                    .HasForeignKey(sc => sc.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentEnrollments)
                    .HasForeignKey(sc => sc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
                entity.HasIndex(e => e.PaymentStatusLookupId);
                entity.HasIndex(e => e.EnrollmentStatusLookupId);
                entity.HasIndex(e => e.TransactionId);
            });

            // Configure StudentBasket
            modelBuilder.Entity<StudentBasket>(entity =>
            {
                entity.HasOne(sb => sb.Student)
                    .WithMany(s => s.BasketItems)
                    .HasForeignKey(sb => sb.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sb => sb.Course)
                    .WithMany(c => c.BasketItems)
                    .HasForeignKey(sb => sb.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.StudentId, e.CourseId });
                entity.HasIndex(e => e.BasketStatusLookupId);
            });

            // Configure ServiceContactUs
            modelBuilder.Entity<ServiceContactUs>(entity =>
            {
                entity.HasOne(c => c.Student)
                    .WithMany(s => s.ContactRequests)
                    .HasForeignKey(c => c.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.TicketNumber).IsUnique();
                entity.HasIndex(e => e.StatusLookupId);
                entity.HasIndex(e => e.ContactTypeLookupId);
            });
        }

        private void SeedLookupData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            // Define Lookup Header IDs
            var courseLevelHeaderId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var courseCategoryHeaderId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var questionTypeHeaderId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var questionStatusHeaderId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var userRoleHeaderId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var userStatusHeaderId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var examStatusHeaderId = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var paymentStatusHeaderId = Guid.Parse("88888888-8888-8888-8888-888888888888");
            var enrollmentStatusHeaderId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var contentTypeHeaderId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
            var videoTypeHeaderId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
            var fileTypeHeaderId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
            var basketStatusHeaderId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");
            var contactTypeHeaderId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
            var contactStatusHeaderId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
            var examModeHeaderId = Guid.Parse("12121212-1212-1212-1212-121212121212");

            // ============================================
            // SEED LOOKUP HEADERS
            // ============================================
            modelBuilder.Entity<AppLookupHeader>().HasData(
                new AppLookupHeader
                {
                    Oid = courseLevelHeaderId,
                    LookupCode = "COURSE_LEVEL",
                    LookupNameAr = "مستوى الدورة",
                    LookupNameEn = "Course Level",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = courseCategoryHeaderId,
                    LookupCode = "COURSE_CATEGORY",
                    LookupNameAr = "فئة الدورة",
                    LookupNameEn = "Course Category",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = questionTypeHeaderId,
                    LookupCode = "QUESTION_TYPE",
                    LookupNameAr = "نوع السؤال",
                    LookupNameEn = "Question Type",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = questionStatusHeaderId,
                    LookupCode = "QUESTION_STATUS",
                    LookupNameAr = "حالة السؤال",
                    LookupNameEn = "Question Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = userRoleHeaderId,
                    LookupCode = "USER_ROLE",
                    LookupNameAr = "دور المستخدم",
                    LookupNameEn = "User Role",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = userStatusHeaderId,
                    LookupCode = "USER_STATUS",
                    LookupNameAr = "حالة المستخدم",
                    LookupNameEn = "User Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = examStatusHeaderId,
                    LookupCode = "EXAM_STATUS",
                    LookupNameAr = "حالة الامتحان",
                    LookupNameEn = "Exam Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = paymentStatusHeaderId,
                    LookupCode = "PAYMENT_STATUS",
                    LookupNameAr = "حالة الدفع",
                    LookupNameEn = "Payment Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = enrollmentStatusHeaderId,
                    LookupCode = "ENROLLMENT_STATUS",
                    LookupNameAr = "حالة التسجيل",
                    LookupNameEn = "Enrollment Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = contentTypeHeaderId,
                    LookupCode = "CONTENT_TYPE",
                    LookupNameAr = "نوع المحتوى",
                    LookupNameEn = "Content Type",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = videoTypeHeaderId,
                    LookupCode = "VIDEO_TYPE",
                    LookupNameAr = "نوع الفيديو",
                    LookupNameEn = "Video Type",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = fileTypeHeaderId,
                    LookupCode = "FILE_TYPE",
                    LookupNameAr = "نوع الملف",
                    LookupNameEn = "File Type",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = basketStatusHeaderId,
                    LookupCode = "BASKET_STATUS",
                    LookupNameAr = "حالة السلة",
                    LookupNameEn = "Basket Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = contactTypeHeaderId,
                    LookupCode = "CONTACT_TYPE",
                    LookupNameAr = "نوع الاتصال",
                    LookupNameEn = "Contact Type",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupHeader
                {
                    Oid = contactStatusHeaderId,
                    LookupCode = "CONTACT_STATUS",
                    LookupNameAr = "حالة الاتصال",
                    LookupNameEn = "Contact Status",
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                 new AppLookupHeader
                 {
                     Oid = examModeHeaderId,
                     LookupCode = "EXAM_MODE",
                     LookupNameAr = "وضع الامتحان",
                     LookupNameEn = "Exam Mode",
                     IsActive = true,
                     CreatedAt = seedDate,
                     CreatedBy = null
                 }
            );

            // ============================================
            // SEED LOOKUP DETAILS - COURSE LEVELS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                    LookupHeaderId = courseLevelHeaderId,
                    LookupValue = "BEGINNER",
                    LookupNameAr = "مبتدئ",
                    LookupNameEn = "Beginner",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                    LookupHeaderId = courseLevelHeaderId,
                    LookupValue = "INTERMEDIATE",
                    LookupNameAr = "متوسط",
                    LookupNameEn = "Intermediate",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                    LookupHeaderId = courseLevelHeaderId,
                    LookupValue = "ADVANCED",
                    LookupNameAr = "متقدم",
                    LookupNameEn = "Advanced",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                    LookupHeaderId = courseLevelHeaderId,
                    LookupValue = "EXPERT",
                    LookupNameAr = "خبير",
                    LookupNameEn = "Expert",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - COURSE CATEGORIES
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "PROGRAMMING",
                    LookupNameAr = "البرمجة",
                    LookupNameEn = "Programming",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "MEDICAL",
                    LookupNameAr = "الطب",
                    LookupNameEn = "Medical",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "FINANCE",
                    LookupNameAr = "المالية",
                    LookupNameEn = "Finance",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222204"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "ENGINEERING",
                    LookupNameAr = "الهندسة",
                    LookupNameEn = "Engineering",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222205"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "BUSINESS",
                    LookupNameAr = "الأعمال",
                    LookupNameEn = "Business",
                    OrderNo = 5,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222206"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "DESIGN",
                    LookupNameAr = "التصميم",
                    LookupNameEn = "Design",
                    OrderNo = 6,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222207"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "MARKETING",
                    LookupNameAr = "التسويق",
                    LookupNameEn = "Marketing",
                    OrderNo = 7,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222208"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "LANGUAGE",
                    LookupNameAr = "اللغة",
                    LookupNameEn = "Language",
                    OrderNo = 8,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222209"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "DATA_SCIENCE",
                    LookupNameAr = "علم البيانات",
                    LookupNameEn = "Data Science",
                    OrderNo = 9,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("22222222-2222-2222-2222-222222222210"),
                    LookupHeaderId = courseCategoryHeaderId,
                    LookupValue = "CYBERSECURITY",
                    LookupNameAr = "الأمن السيبراني",
                    LookupNameEn = "Cybersecurity",
                    OrderNo = 10,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - QUESTION TYPES
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                    LookupHeaderId = questionTypeHeaderId,
                    LookupValue = "MCQ",
                    LookupNameAr = "اختيار من متعدد",
                    LookupNameEn = "Multiple Choice Question",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("33333333-3333-3333-3333-333333333302"),
                    LookupHeaderId = questionTypeHeaderId,
                    LookupValue = "TRUE_FALSE",
                    LookupNameAr = "صح أو خطأ",
                    LookupNameEn = "True/False",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("33333333-3333-3333-3333-333333333306"),
                    LookupHeaderId = questionTypeHeaderId,
                    LookupValue = "MATCHING",
                    LookupNameAr = "مطابقة",
                    LookupNameEn = "Matching",
                    OrderNo = 6,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - QUESTION STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                    LookupHeaderId = questionStatusHeaderId,
                    LookupValue = "CORRECT",
                    LookupNameAr = "صحيح",
                    LookupNameEn = "Correct",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                    LookupHeaderId = questionStatusHeaderId,
                    LookupValue = "INCORRECT",
                    LookupNameAr = "غير صحيح",
                    LookupNameEn = "Incorrect",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                    LookupHeaderId = questionStatusHeaderId,
                    LookupValue = "NOT_ANSWERED",
                    LookupNameAr = "لم يتم الإجابة",
                    LookupNameEn = "Not Answered",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - USER ROLES
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("55555555-5555-5555-5555-555555555501"),
                    LookupHeaderId = userRoleHeaderId,
                    LookupValue = "ADMIN",
                    LookupNameAr = "مدير النظام",
                    LookupNameEn = "Admin",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("55555555-5555-5555-5555-555555555502"),
                    LookupHeaderId = userRoleHeaderId,
                    LookupValue = "INSTRUCTOR",
                    LookupNameAr = "مدرب",
                    LookupNameEn = "Instructor",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("55555555-5555-5555-5555-555555555503"),
                    LookupHeaderId = userRoleHeaderId,
                    LookupValue = "STUDENT",
                    LookupNameAr = "طالب",
                    LookupNameEn = "Student",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("55555555-5555-5555-5555-555555555504"),
                    LookupHeaderId = userRoleHeaderId,
                    LookupValue = "SUPPORT",
                    LookupNameAr = "الدعم الفني",
                    LookupNameEn = "Support",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - USER STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("66666666-6666-6666-6666-666666666601"),
                    LookupHeaderId = userStatusHeaderId,
                    LookupValue = "ACTIVE",
                    LookupNameAr = "نشط",
                    LookupNameEn = "Active",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("66666666-6666-6666-6666-666666666602"),
                    LookupHeaderId = userStatusHeaderId,
                    LookupValue = "INACTIVE",
                    LookupNameAr = "غير نشط",
                    LookupNameEn = "Inactive",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("66666666-6666-6666-6666-666666666603"),
                    LookupHeaderId = userStatusHeaderId,
                    LookupValue = "SUSPENDED",
                    LookupNameAr = "معلق",
                    LookupNameEn = "Suspended",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("66666666-6666-6666-6666-666666666604"),
                    LookupHeaderId = userStatusHeaderId,
                    LookupValue = "PENDING",
                    LookupNameAr = "قيد الانتظار",
                    LookupNameEn = "Pending",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - EXAM STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("77777777-7777-7777-7777-777777777701"),
                    LookupHeaderId = examStatusHeaderId,
                    LookupValue = "NOT_STARTED",
                    LookupNameAr = "لم يبدأ",
                    LookupNameEn = "Not Started",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("77777777-7777-7777-7777-777777777702"),
                    LookupHeaderId = examStatusHeaderId,
                    LookupValue = "IN_PROGRESS",
                    LookupNameAr = "قيد التنفيذ",
                    LookupNameEn = "In Progress",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("77777777-7777-7777-7777-777777777703"),
                    LookupHeaderId = examStatusHeaderId,
                    LookupValue = "SUBMITTED",
                    LookupNameAr = "تم التقديم",
                    LookupNameEn = "Submitted",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("77777777-7777-7777-7777-777777777704"),
                    LookupHeaderId = examStatusHeaderId,
                    LookupValue = "REVIEWED",
                    LookupNameAr = "تمت المراجعة",
                    LookupNameEn = "Reviewed",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("77777777-7777-7777-7777-777777777705"),
                    LookupHeaderId = examStatusHeaderId,
                    LookupValue = "DELETED",
                    LookupNameAr = "تم الحذف",
                    LookupNameEn = "Deleted",
                    OrderNo = 5,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - PAYMENT STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("88888888-8888-8888-8888-888888888801"),
                    LookupHeaderId = paymentStatusHeaderId,
                    LookupValue = "PENDING",
                    LookupNameAr = "قيد الانتظار",
                    LookupNameEn = "Pending",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("88888888-8888-8888-8888-888888888802"),
                    LookupHeaderId = paymentStatusHeaderId,
                    LookupValue = "PAID",
                    LookupNameAr = "مدفوع",
                    LookupNameEn = "Paid",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("88888888-8888-8888-8888-888888888803"),
                    LookupHeaderId = paymentStatusHeaderId,
                    LookupValue = "FAILED",
                    LookupNameAr = "فشل",
                    LookupNameEn = "Failed",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("88888888-8888-8888-8888-888888888804"),
                    LookupHeaderId = paymentStatusHeaderId,
                    LookupValue = "REFUNDED",
                    LookupNameAr = "مسترد",
                    LookupNameEn = "Refunded",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - ENROLLMENT STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("99999999-9999-9999-9999-999999999901"),
                    LookupHeaderId = enrollmentStatusHeaderId,
                    LookupValue = "ACTIVE",
                    LookupNameAr = "نشط",
                    LookupNameEn = "Active",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("99999999-9999-9999-9999-999999999902"),
                    LookupHeaderId = enrollmentStatusHeaderId,
                    LookupValue = "EXPIRED",
                    LookupNameAr = "منتهي",
                    LookupNameEn = "Expired",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("99999999-9999-9999-9999-999999999903"),
                    LookupHeaderId = enrollmentStatusHeaderId,
                    LookupValue = "SUSPENDED",
                    LookupNameAr = "معلق",
                    LookupNameEn = "Suspended",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("99999999-9999-9999-9999-999999999904"),
                    LookupHeaderId = enrollmentStatusHeaderId,
                    LookupValue = "COMPLETED",
                    LookupNameAr = "مكتمل",
                    LookupNameEn = "Completed",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - CONTENT TYPE
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAA01"),
                    LookupHeaderId = contentTypeHeaderId,
                    LookupValue = "VIDEO",
                    LookupNameAr = "فيديو",
                    LookupNameEn = "Video",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAA02"),
                    LookupHeaderId = contentTypeHeaderId,
                    LookupValue = "PDF",
                    LookupNameAr = "بي دي إف",
                    LookupNameEn = "PDF",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAA03"),
                    LookupHeaderId = contentTypeHeaderId,
                    LookupValue = "QUIZ",
                    LookupNameAr = "اختبار",
                    LookupNameEn = "Quiz",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAA04"),
                    LookupHeaderId = contentTypeHeaderId,
                    LookupValue = "ASSIGNMENT",
                    LookupNameAr = "واجب",
                    LookupNameEn = "Assignment",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAA05"),
                    LookupHeaderId = contentTypeHeaderId,
                    LookupValue = "ARTICLE",
                    LookupNameAr = "مقال",
                    LookupNameEn = "Article",
                    OrderNo = 5,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - VIDEO TYPE
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBB01"),
                    LookupHeaderId = videoTypeHeaderId,
                    LookupValue = "INTRODUCTION",
                    LookupNameAr = "مقدمة",
                    LookupNameEn = "Introduction",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBB02"),
                    LookupHeaderId = videoTypeHeaderId,
                    LookupValue = "LECTURE",
                    LookupNameAr = "محاضرة",
                    LookupNameEn = "Lecture",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBB03"),
                    LookupHeaderId = videoTypeHeaderId,
                    LookupValue = "TUTORIAL",
                    LookupNameAr = "درس تعليمي",
                    LookupNameEn = "Tutorial",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBB04"),
                    LookupHeaderId = videoTypeHeaderId,
                    LookupValue = "DEMO",
                    LookupNameAr = "عرض توضيحي",
                    LookupNameEn = "Demo",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - FILE TYPE
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCC01"),
                    LookupHeaderId = fileTypeHeaderId,
                    LookupValue = "PDF",
                    LookupNameAr = "بي دي إف",
                    LookupNameEn = "PDF",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCC02"),
                    LookupHeaderId = fileTypeHeaderId,
                    LookupValue = "DOCX",
                    LookupNameAr = "وورد",
                    LookupNameEn = "Word Document",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCC03"),
                    LookupHeaderId = fileTypeHeaderId,
                    LookupValue = "PPTX",
                    LookupNameAr = "باوربوينت",
                    LookupNameEn = "PowerPoint",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCC04"),
                    LookupHeaderId = fileTypeHeaderId,
                    LookupValue = "ZIP",
                    LookupNameAr = "ملف مضغوط",
                    LookupNameEn = "ZIP Archive",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - BASKET STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDD01"),
                    LookupHeaderId = basketStatusHeaderId,
                    LookupValue = "ACTIVE",
                    LookupNameAr = "نشط",
                    LookupNameEn = "Active",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDD02"),
                    LookupHeaderId = basketStatusHeaderId,
                    LookupValue = "CHECKED_OUT",
                    LookupNameAr = "تم الدفع",
                    LookupNameEn = "Checked Out",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDD03"),
                    LookupHeaderId = basketStatusHeaderId,
                    LookupValue = "ABANDONED",
                    LookupNameAr = "متروك",
                    LookupNameEn = "Abandoned",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - CONTACT TYPE
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEE01"),
                    LookupHeaderId = contactTypeHeaderId,
                    LookupValue = "TECHNICAL_SUPPORT",
                    LookupNameAr = "دعم فني",
                    LookupNameEn = "Technical Support",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEE02"),
                    LookupHeaderId = contactTypeHeaderId,
                    LookupValue = "BILLING",
                    LookupNameAr = "الفوترة",
                    LookupNameEn = "Billing",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEE03"),
                    LookupHeaderId = contactTypeHeaderId,
                    LookupValue = "GENERAL_INQUIRY",
                    LookupNameAr = "استفسار عام",
                    LookupNameEn = "General Inquiry",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEE04"),
                    LookupHeaderId = contactTypeHeaderId,
                    LookupValue = "COMPLAINT",
                    LookupNameAr = "شكوى",
                    LookupNameEn = "Complaint",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );

            // ============================================
            // SEED LOOKUP DETAILS - CONTACT STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFF01"),
                    LookupHeaderId = contactStatusHeaderId,
                    LookupValue = "NEW",
                    LookupNameAr = "جديد",
                    LookupNameEn = "New",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFF02"),
                    LookupHeaderId = contactStatusHeaderId,
                    LookupValue = "IN_PROGRESS",
                    LookupNameAr = "قيد المعالجة",
                    LookupNameEn = "In Progress",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFF03"),
                    LookupHeaderId = contactStatusHeaderId,
                    LookupValue = "RESOLVED",
                    LookupNameAr = "تم الحل",
                    LookupNameEn = "Resolved",
                    OrderNo = 3,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFF04"),
                    LookupHeaderId = contactStatusHeaderId,
                    LookupValue = "CLOSED",
                    LookupNameAr = "مغلق",
                    LookupNameEn = "Closed",
                    OrderNo = 4,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
            );
            // ============================================
            // SEED LOOKUP DETAILS - BASKET STATUS
            // ============================================
            modelBuilder.Entity<AppLookupDetail>().HasData(
                new AppLookupDetail
                {
                    Oid = Guid.Parse("DDDDDDDD-DDDD-DDDD-1212-DDDDDDDDDD01"),
                    LookupHeaderId = examModeHeaderId,
                    LookupValue = "PRACTICE_MODE",
                    LookupNameAr = "وضع الممارسة",
                    LookupNameEn = "Practice Mode",
                    OrderNo = 1,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                },
                new AppLookupDetail
                {
                    Oid = Guid.Parse("DDDDDDDD-DDDD-DDDD-1212-DDDDDDDDDD02"),
                    LookupHeaderId = examModeHeaderId,
                    LookupValue = "EXAM_MODE",
                    LookupNameAr = "وضع الامتحان",
                    LookupNameEn = "Exam Mode",
                    OrderNo = 2,
                    IsActive = true,
                    CreatedAt = seedDate,
                    CreatedBy = null
                }
              
            );


            //// ============================================
            //// SEED SAMPLE USERS (Password: "Admin@123" hashed with SHA256)
            //// ============================================
            //var adminUserId = Guid.Parse("A0000000-0000-0000-0000-000000000001");
            //var instructorUserId = Guid.Parse("A0000000-0000-0000-0000-000000000002");

            //modelBuilder.Entity<User>().HasData(
            //    new User
            //    {
            //        Oid = adminUserId,
            //        Username = "admin",
            //        Email = "admin@helpempowerment.com",
            //        PasswordHash = "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", // Admin@123
            //        RoleLookupId = Guid.Parse("55555555-5555-5555-5555-555555555501"), // ADMIN
            //        StatusLookupId = Guid.Parse("66666666-6666-6666-6666-666666666601"), // ACTIVE
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = null
            //    },
            //    new User
            //    {
            //        Oid = instructorUserId,
            //        Username = "instructor1",
            //        Email = "instructor@helpempowerment.com",
            //        PasswordHash = "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", // Admin@123
            //        RoleLookupId = Guid.Parse("55555555-5555-5555-5555-555555555502"), // INSTRUCTOR
            //        StatusLookupId = Guid.Parse("66666666-6666-6666-6666-666666666601"), // ACTIVE
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = adminUserId
            //    }
            //);

            //// ============================================
            //// SEED SAMPLE STUDENTS (Password: "Student@123" hashed with SHA256)
            //// ============================================
            //var student1Id = Guid.Parse("B0000000-0000-0000-0000-000000000001");
            //var student2Id = Guid.Parse("B0000000-0000-0000-0000-000000000002");

            //modelBuilder.Entity<Student>().HasData(
            //    new Student
            //    {
            //        Oid = student1Id,
            //        Username = "student1",
            //        Email = "student1@example.com",
            //        PasswordHash = "D41D8CD98F00B204E9800998ECF8427E", // Student@123
            //        NameEn = "John Doe",
            //        NameAr = "جون دو",
            //        Mobile = "+1234567890",
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = adminUserId
            //    },
            //    new Student
            //    {
            //        Oid = student2Id,
            //        Username = "student2",
            //        Email = "student2@example.com",
            //        PasswordHash = "D41D8CD98F00B204E9800998ECF8427E", // Student@123
            //        NameEn = "Jane Smith",
            //        NameAr = "جين سميث",
            //        Mobile = "+1234567891",
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = adminUserId
            //    }
            //);

            //// ============================================
            //// SEED SAMPLE COURSES
            //// ============================================
            //var course1Id = Guid.Parse("C0000000-0000-0000-0000-000000000001");
            //var course2Id = Guid.Parse("C0000000-0000-0000-0000-000000000002");

            //modelBuilder.Entity<Course>().HasData(
            //    new Course
            //    {
            //        Oid = course1Id,
            //        CourseCode = "CS101",
            //        CourseName = "Introduction to C# Programming",
            //        CourseDescription = "Learn the fundamentals of C# programming language",
            //        CourseLevelLookupId = Guid.Parse("11111111-1111-1111-1111-111111111101"), // BEGINNER
            //        CourseCategoryLookupId = Guid.Parse("22222222-2222-2222-2222-222222222201"), // PROGRAMMING
            //        InstructorOid = instructorUserId,
            //        Price = "199.99",
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = adminUserId
            //    },
            //    new Course
            //    {
            //        Oid = course2Id,
            //        CourseCode = "CS201",
            //        CourseName = "Advanced .NET Development",
            //        CourseDescription = "Master advanced .NET concepts and best practices",
            //        CourseLevelLookupId = Guid.Parse("11111111-1111-1111-1111-111111111103"), // ADVANCED
            //        CourseCategoryLookupId = Guid.Parse("22222222-2222-2222-2222-222222222201"), // PROGRAMMING
            //        InstructorOid = instructorUserId,
            //        Price = "299.99",
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = adminUserId
            //    }
            //);

            //// ============================================
            //// SEED SAMPLE MASTER EXAMS
            //// ============================================
            //var exam1Id = Guid.Parse("D0000000-0000-0000-0000-000000000001");
            //var exam2Id = Guid.Parse("D0000000-0000-0000-0000-000000000002");

            //modelBuilder.Entity<CoursesMasterExam>().HasData(
            //    new CoursesMasterExam
            //    {
            //        Oid = exam1Id,
            //        CourseOid = course1Id,
            //        CourseName = "C# Fundamentals Final Exam",
            //        ExamName = "Final Exam",
            //        CourseLevelLookupId = Guid.Parse("11111111-1111-1111-1111-111111111101"), // BEGINNER
            //        CourseCategoryLookupId = Guid.Parse("22222222-2222-2222-2222-222222222201"), // PROGRAMMING
            //        DurationMinutes = 60,
            //        PassPercent = 70,
            //        QuestionCount = 3,
            //        MaxAttempts = 3,
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CoursesMasterExam
            //    {
            //        Oid = exam2Id,
            //        CourseOid = course2Id,
            //        CourseName = ".NET Advanced Certification Exam",
            //        ExamName = "Certification Exam",
            //        CourseLevelLookupId = Guid.Parse("11111111-1111-1111-1111-111111111103"), // ADVANCED
            //        CourseCategoryLookupId = Guid.Parse("22222222-2222-2222-2222-222222222201"), // PROGRAMMING
            //        DurationMinutes = 90,
            //        PassPercent = 75,
            //        QuestionCount = 5,
            //        MaxAttempts = 2,
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    }
            //);

            //// ============================================
            //// SEED SAMPLE QUESTIONS FOR EXAM 1
            //// ============================================
            //var question1Id = Guid.Parse("E0000000-0000-0000-0000-000000000001");
            //var question2Id = Guid.Parse("E0000000-0000-0000-0000-000000000002");
            //var question3Id = Guid.Parse("E0000000-0000-0000-0000-000000000003");

            //modelBuilder.Entity<CourseQuestion>().HasData(
            //    new CourseQuestion
            //    {
            //        Oid = question1Id,
            //        CoursesMasterExamOid = exam1Id,
            //        QuestionText = "What is the correct syntax to declare a variable in C#?",
            //        QuestionTypeLookupId = Guid.Parse("33333333-3333-3333-3333-333333333301"), // MCQ
            //        QuestionScore = 10,
            //        OrderNo = 1,
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CourseQuestion
            //    {
            //        Oid = question2Id,
            //        CoursesMasterExamOid = exam1Id,
            //        QuestionText = "C# is a case-sensitive language",
            //        QuestionTypeLookupId = Guid.Parse("33333333-3333-3333-3333-333333333302"), // TRUE_FALSE
            //        QuestionScore = 5,
            //        OrderNo = 2,
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CourseQuestion
            //    {
            //        Oid = question3Id,
            //        CoursesMasterExamOid = exam1Id,
            //        QuestionText = "Which of the following is a reference type in C#?",
            //        QuestionTypeLookupId = Guid.Parse("33333333-3333-3333-3333-333333333301"), // MCQ
            //        QuestionScore = 10,
            //        OrderNo = 3,
            //        IsActive = true,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    }
            //);

            //// ============================================
            //// SEED SAMPLE ANSWERS FOR QUESTIONS
            //// ============================================
            //var answer1Id = Guid.Parse("F0000000-0000-0000-0000-000000000001");
            //var answer2Id = Guid.Parse("F0000000-0000-0000-0000-000000000002");
            //var answer3Id = Guid.Parse("F0000000-0000-0000-0000-000000000003");
            //var answer4Id = Guid.Parse("F0000000-0000-0000-0000-000000000004");
            //var answer5Id = Guid.Parse("F0000000-0000-0000-0000-000000000005");
            //var answer6Id = Guid.Parse("F0000000-0000-0000-0000-000000000006");

            //modelBuilder.Entity<CourseAnswer>().HasData(
            //    // Answers for Question 1 (MCQ - Variable Declaration)
            //    new CourseAnswer
            //    {
            //        Oid = answer1Id,
            //        QuestionId = question1Id,
            //        AnswerText = "int x = 10;",
            //        IsCorrect = true,
            //        OrderNo = 1,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CourseAnswer
            //    {
            //        Oid = answer2Id,
            //        QuestionId = question1Id,
            //        AnswerText = "var x 10;",
            //        IsCorrect = false,
            //        OrderNo = 2,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    // Answers for Question 2 (TRUE/FALSE - Case Sensitivity)
            //    new CourseAnswer
            //    {
            //        Oid = answer3Id,
            //        QuestionId = question2Id,
            //        AnswerText = "True",
            //        IsCorrect = true,
            //        OrderNo = 1,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CourseAnswer
            //    {
            //        Oid = answer4Id,
            //        QuestionId = question2Id,
            //        AnswerText = "False",
            //        IsCorrect = false,
            //        OrderNo = 2,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    // Answers for Question 3 (MCQ - Reference Types)
            //    new CourseAnswer
            //    {
            //        Oid = answer5Id,
            //        QuestionId = question3Id,
            //        AnswerText = "string",
            //        IsCorrect = true,
            //        OrderNo = 1,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    },
            //    new CourseAnswer
            //    {
            //        Oid = answer6Id,
            //        QuestionId = question3Id,
            //        AnswerText = "int",
            //        IsCorrect = false,
            //        OrderNo = 2,
            //        CreatedAt = seedDate,
            //        CreatedBy = instructorUserId
            //    }
            // );
        }

        private void ConfigureBaseEntityIndexes<TEntity>(ModelBuilder modelBuilder) 
            where TEntity : Common.BaseEntity
        {
            // Index on Oid (Primary Key - automatically indexed, but explicit for clarity)
            modelBuilder.Entity<TEntity>()
                .HasIndex(e => e.Oid);

            // Index on IsDeleted for soft delete queries
            modelBuilder.Entity<TEntity>()
                .HasIndex(e => e.IsDeleted);

            // Index on CreatedAt for date range queries
            modelBuilder.Entity<TEntity>()
                .HasIndex(e => e.CreatedAt);

            // Composite index for common soft delete queries
            modelBuilder.Entity<TEntity>()
                .HasIndex(e => new { e.IsDeleted, e.CreatedAt });
        }
    }
}