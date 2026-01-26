using Microsoft.EntityFrameworkCore;
using StandardArticture.Models;

namespace StandardArticture.Data
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
                //new AppLookupDetail
                //{
                //    Oid = Guid.Parse("33333333-3333-3333-3333-333333333303"),
                //    LookupHeaderId = questionTypeHeaderId,
                //    LookupValue = "TEXT",
                //    LookupNameAr = "نص",
                //    LookupNameEn = "Text Answer",
                //    OrderNo = 3,
                //    IsActive = true,
                //    CreatedAt = DateTime.UtcNow,
                //    CreatedBy = null
                //},
                //new AppLookupDetail
                //{
                //    Oid = Guid.Parse("33333333-3333-3333-3333-333333333304"),
                //    LookupHeaderId = questionTypeHeaderId,
                //    LookupValue = "FILL_BLANK",
                //    LookupNameAr = "املأ الفراغ",
                //    LookupNameEn = "Fill in the Blank",
                //    OrderNo = 4,
                //    IsActive = true,
                //    CreatedAt = DateTime.UtcNow,
                //    CreatedBy = null
                //},
                //new AppLookupDetail
                //{
                //    Oid = Guid.Parse("33333333-3333-3333-3333-333333333305"),
                //    LookupHeaderId = questionTypeHeaderId,
                //    LookupValue = "ESSAY",
                //    LookupNameAr = "مقال",
                //    LookupNameEn = "Essay",
                //    OrderNo = 5,
                //    IsActive = true,
                //    CreatedAt = DateTime.UtcNow,
                //    CreatedBy = null
                //},
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