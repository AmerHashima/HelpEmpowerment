using System.Security.Claims;
using HelpEmpowermentApi.Controllers;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Domain;
using HelpEmpowermentApi.Payments.Infrastructure;
using HelpEmpowermentApi.Services;
using HelpEmpowermentApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HelpEmpowermentApi.Tests;

public sealed class RevenueManagementTests
{
    private static readonly Guid TrainerTypeId = Guid.Parse("14141414-1414-1414-1414-141414141402");

    [Fact]
    public async Task Assignment_is_created_and_duplicate_active_assignment_is_rejected()
    {
        await using var test = await TestDatabase.CreateAsync();
        var (user, course) = await test.AddUserAndCourseAsync();
        var dto = new SaveUserCourseAssignmentDto { UserId = user.Oid, CourseId = course.Oid, AssignmentTypeId = TrainerTypeId };

        var first = await test.Service.CreateAssignmentAsync(dto, user.Oid, default);
        var duplicate = await test.Service.CreateAssignmentAsync(dto, user.Oid, default);

        Assert.True(first.IsSuccess);
        Assert.False(duplicate.IsSuccess);
        Assert.Equal(1, await test.Db.UserCourseAssignments.CountAsync());
    }

    [Fact]
    public async Task User_courses_only_include_that_users_active_assignments()
    {
        await using var test = await TestDatabase.CreateAsync();
        var (user, course) = await test.AddUserAndCourseAsync();
        var otherCourse = await test.AddCourseAsync("OTHER");
        await test.Service.CreateAssignmentAsync(new() { UserId = user.Oid, CourseId = course.Oid, AssignmentTypeId = TrainerTypeId }, user.Oid, default);

        var courses = await test.Service.GetUserCoursesAsync(user.Oid, default);

        Assert.Single(courses);
        Assert.Equal(course.Oid, courses[0].CourseId);
        Assert.DoesNotContain(courses, x => x.CourseId == otherCourse.Oid);
    }

    [Fact]
    public async Task Me_courses_uses_authenticated_user_identity()
    {
        await using var test = await TestDatabase.CreateAsync();
        var (user, course) = await test.AddUserAndCourseAsync();
        var other = await test.AddUserAsync("other");
        await test.Service.CreateAssignmentAsync(new() { UserId = user.Oid, CourseId = course.Oid, AssignmentTypeId = TrainerTypeId }, user.Oid, default);
        var controller = new MeController(test.Service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, other.Oid.ToString()),
                        new Claim("UserType", "User")
                    }, "Test"))
                }
            }
        };

        var action = await controller.Courses(default);
        var result = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<ApiResponse<IReadOnlyList<AssignedCourseDto>>>(result.Value);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task Percentage_value_and_course_total_cannot_exceed_one_hundred()
    {
        await using var test = await TestDatabase.CreateAsync();
        var (user, course) = await test.AddUserAndCourseAsync();
        var ownerType = Guid.Parse("14141414-1414-1414-1414-141414141401");
        var first = await test.Service.CreateShareAsync(course.Oid, new()
        { BeneficiaryUserId = user.Oid, ShareTypeId = TrainerTypeId, CalculationType = RevenueCalculationType.Percentage, Value = 60 }, user.Oid, default);
        var overTotal = await test.Service.CreateShareAsync(course.Oid, new()
        { ShareTypeId = ownerType, CalculationType = RevenueCalculationType.Percentage, Value = 41 }, user.Oid, default);
        var overValue = await test.Service.CreateShareAsync(course.Oid, new()
        { ShareTypeId = ownerType, CalculationType = RevenueCalculationType.Percentage, Value = 101 }, user.Oid, default);

        Assert.True(first.IsSuccess);
        Assert.False(overTotal.IsSuccess);
        Assert.False(overValue.IsSuccess);
    }

    [Fact]
    public async Task Successful_payment_snapshots_distribution_and_duplicate_callback_is_idempotent()
    {
        await using var test = await TestDatabase.CreateAsync();
        var scenario = await test.AddPaidScenarioAsync(20);

        var first = await test.ProcessPaymentAsync(scenario.Payment.Id);
        var second = await test.ProcessPaymentAsync(scenario.Payment.Id);
        var distributions = await test.Db.CourseRevenueDistributions.AsNoTracking().ToListAsync();
        var summary = await test.Service.GetCourseSummaryAsync(scenario.Course.Oid, default);

        Assert.True(first.IsSuccess && first.Value);
        Assert.True(second.IsSuccess && !second.Value);
        var distribution = Assert.Single(distributions);
        Assert.Equal(200m, distribution.ShareAmount);
        Assert.Equal(20m, distribution.AppliedPercentage);
        Assert.Equal(1000m, summary!.TotalRevenue);
        Assert.Equal(200m, summary.DistributedRevenue);
    }

    [Fact]
    public async Task Updating_share_does_not_change_historical_distribution()
    {
        await using var test = await TestDatabase.CreateAsync();
        var scenario = await test.AddPaidScenarioAsync(20);
        await test.ProcessPaymentAsync(scenario.Payment.Id);

        var updated = await test.Service.UpdateShareAsync(scenario.Course.Oid, scenario.Share.Oid, new()
        { BeneficiaryUserId = scenario.User.Oid, ShareTypeId = TrainerTypeId, CalculationType = RevenueCalculationType.Percentage, Value = 30 }, scenario.User.Oid, default);
        var snapshot = await test.Db.CourseRevenueDistributions.AsNoTracking().SingleAsync();

        Assert.True(updated.IsSuccess);
        Assert.Equal(20m, snapshot.AppliedPercentage);
        Assert.Equal(200m, snapshot.ShareAmount);
    }

    [Fact]
    public async Task User_only_sees_own_revenue_and_admin_global_dashboard_sees_all_courses()
    {
        await using var test = await TestDatabase.CreateAsync();
        var scenario = await test.AddPaidScenarioAsync(20);
        var other = await test.AddUserAsync("other");
        await test.ProcessPaymentAsync(scenario.Payment.Id);

        var mine = await test.Service.GetMyRevenueAsync(scenario.User.Oid, null, null, null, null, default);
        var others = await test.Service.GetMyRevenueAsync(other.Oid, null, null, null, null, default);
        var adminDashboard = await test.Service.GetDashboardAsync(other.Oid, true, default);

        Assert.Equal(200m, mine.TotalEarned);
        Assert.Equal(0m, others.TotalEarned);
        Assert.True(adminDashboard.AssignedCourses >= 1);
        Assert.Equal(1000m, adminDashboard.TotalRevenue);
    }

    [Fact]
    public async Task Paid_settlement_moves_distribution_from_pending_to_paid()
    {
        await using var test = await TestDatabase.CreateAsync();
        var scenario = await test.AddPaidScenarioAsync(20);
        await test.ProcessPaymentAsync(scenario.Payment.Id);
        var settlement = await test.Service.CreateSettlementAsync(new()
        { BeneficiaryUserId = scenario.User.Oid, PeriodFrom = DateTime.UtcNow.AddDays(-1), PeriodTo = DateTime.UtcNow.AddDays(1) }, scenario.User.Oid, default);

        Assert.True(settlement.IsSuccess);
        await test.Service.UpdateSettlementStatusAsync(settlement.Value!.Oid,
            new() { Status = RevenueSettlementStatus.Approved }, scenario.User.Oid, default);
        var paid = await test.Service.UpdateSettlementStatusAsync(settlement.Value.Oid,
            new() { Status = RevenueSettlementStatus.Paid, PaymentReference = "BANK-1" }, scenario.User.Oid, default);
        var distribution = await test.Db.CourseRevenueDistributions.AsNoTracking().SingleAsync();

        Assert.True(paid.IsSuccess);
        Assert.Equal(RevenueDistributionStatus.Paid, distribution.Status);
        Assert.NotNull(distribution.PaidAt);
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        public ApplicationDbContext Db { get; }
        public RevenueManagementService Service { get; }

        private TestDatabase(ApplicationDbContext db)
        { Db = db; Service = new(new RevenueManagementRepository(db)); }

        public static async Task<TestDatabase> CreateAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var db = new ApplicationDbContext(options);
            await db.Database.EnsureCreatedAsync();
            return new(db);
        }

        public async Task<(User User, Course Course)> AddUserAndCourseAsync()
        { var user = await AddUserAsync("user"); var course = await AddCourseAsync("COURSE"); return (user, course); }

        public async Task<User> AddUserAsync(string prefix)
        {
            var user = new User { Oid = Guid.NewGuid(), Username = $"{prefix}-{Guid.NewGuid():N}", PasswordHash = "test", IsActive = true };
            Db.Users.Add(user); await Db.SaveChangesAsync(); return user;
        }

        public async Task<Course> AddCourseAsync(string prefix)
        {
            var course = new Course { Oid = Guid.NewGuid(), CourseCode = $"{prefix}-{Guid.NewGuid():N}"[..35], CourseName = prefix, IsActive = true };
            Db.Courses.Add(course); await Db.SaveChangesAsync(); return course;
        }

        public async Task<(User User, Course Course, CourseRevenueShare Share, PaymentTransaction Payment)> AddPaidScenarioAsync(decimal percentage)
        {
            var (user, course) = await AddUserAndCourseAsync();
            var student = new Student { Oid = Guid.NewGuid(), Username = $"student-{Guid.NewGuid():N}", PasswordHash = "test" };
            var share = new CourseRevenueShare { Oid = Guid.NewGuid(), CourseId = course.Oid, BeneficiaryUserId = user.Oid,
                ShareTypeLookupId = TrainerTypeId, CalculationType = RevenueCalculationType.Percentage, Value = percentage, IsActive = true };
            var invoice = new Invoice { Id = Guid.NewGuid(), OwnerId = student.Oid, InvoiceNumber = $"INV-{Guid.NewGuid():N}", Currency = "AED", TotalAmount = 1000, CreatedAt = DateTime.UtcNow };
            invoice.Items.Add(new InvoiceItem { Id = Guid.NewGuid(), CourseId = course.Oid, Description = course.CourseName,
                Quantity = 1, UnitPrice = 1000, LineTotal = 1000 });
            var payment = new PaymentTransaction { Id = Guid.NewGuid(), Invoice = invoice, InvoiceId = invoice.Id,
                CartId = $"C-{Guid.NewGuid():N}", TelrOrderReference = "ORDER-1", Amount = 1000, Currency = "AED",
                Status = PaymentStatus.Pending, CreatedAt = DateTime.UtcNow };
            Db.AddRange(student, share, invoice, payment); await Db.SaveChangesAsync();
            return (user, course, share, payment);
        }

        public Task<ServiceResult<bool>> ProcessPaymentAsync(Guid paymentId)
        {
            var processor = new InvoicePaymentProcessor(Db, new TestClock(), new TestEmailService(), NullLogger<InvoicePaymentProcessor>.Instance);
            return processor.ProcessAsync(paymentId, new(true, false, false, "A", 1000, "AED", string.Empty,
                "ORDER-1", "TX-1", null, null, "request", "response"), default);
        }

        public async ValueTask DisposeAsync()
        { await Db.DisposeAsync(); }
    }

    private sealed class TestClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }
    private sealed class TestEmailService : IEmailService
    {
        public Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true) => Task.FromResult(true);
        public Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName) => Task.FromResult(true);
    }
}
