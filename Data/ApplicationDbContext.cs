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

        // NEW DbSets - LIVE SESSIONS
        public DbSet<CourseLiveSession> CourseLiveSessions { get; set; }
        public DbSet<CourseLiveSessionStudent> CourseLiveSessionStudents { get; set; }

        // NEW DbSets - INSTRUCTORS & TARGET AUDIENCE
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<CourseTargetAudience> CourseTargetAudiences { get; set; }

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

            // ========================================
            // NEW CONFIGURATIONS
            // ========================================

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

                entity.HasOne(seq => seq.SelectedAnswer)
                    .WithMany()
                    .HasForeignKey(seq => seq.SelectedAnswerOid)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasIndex(e => e.StudentExamOid);
                entity.HasIndex(e => e.QuestionOid);
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
        }

        private void SeedLookupData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            // Define Lookup Header IDs
            var courseLevelHeaderId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var courseCategoryHeaderId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var questionTypeHeaderId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            // Seed Lookup Headers
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
                }
            );

            // Seed Lookup Details - Course Levels
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

            // Seed Lookup Details - Course Categories
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

            // Seed Lookup Details - Question Types
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