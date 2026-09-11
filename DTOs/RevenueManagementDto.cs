using System.ComponentModel.DataAnnotations;
using HelpEmpowermentApi.Enums;

namespace HelpEmpowermentApi.DTOs;

public sealed class UserCourseAssignmentSearchDto
{
    public Guid? UserId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? AssignmentTypeId { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class MyRevenueSearchDto
{
    public Guid? CourseId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public RevenueDistributionStatus? Status { get; set; }
}

public sealed class RevenueSettlementSearchDto
{
    public Guid? BeneficiaryUserId { get; set; }
    public RevenueSettlementStatus? Status { get; set; }
}

public sealed record UserCourseAssignmentDto(Guid Oid, Guid UserId, string UserName, Guid CourseId,
    string CourseName, Guid AssignmentTypeId, string AssignmentType, bool IsPrimary, bool IsActive);

public sealed class SaveUserCourseAssignmentDto
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public Guid AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed record AssignedCourseDto(Guid CourseId, string CourseCode, string CourseName,
    Guid AssignmentTypeId, string AssignmentType, bool IsPrimary, bool IsActive);

public sealed record CourseRevenueShareDto(Guid Oid, Guid CourseId, Guid? UserId, string? UserName,
    Guid ShareTypeId, string ShareType, RevenueCalculationType CalculationType, decimal Value,
    bool IsActive, DateTime? EffectiveFrom, DateTime? EffectiveTo, string? Notes);

public sealed class SaveCourseRevenueShareDto
{
    public Guid? BeneficiaryUserId { get; set; }
    public Guid ShareTypeId { get; set; }
    public RevenueCalculationType CalculationType { get; set; }
    public decimal Value { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}

public sealed record RevenueByCourseDto(Guid CourseId, string CourseName, decimal Earned,
    decimal Pending, decimal Paid);

public sealed record MyRevenueDto(decimal TotalEarned, decimal Pending, decimal Paid,
    IReadOnlyList<RevenueByCourseDto> Courses);

public sealed record RevenueShareBreakdownDto(Guid? UserId, string? UserName, string Type,
    decimal? Percentage, decimal Amount, decimal Pending, decimal Paid);

public sealed record CourseRevenueSummaryDto(Guid CourseId, decimal TotalRevenue,
    decimal DistributedRevenue, decimal PendingRevenue, decimal PaidRevenue,
    IReadOnlyList<RevenueShareBreakdownDto> Shares);

public sealed class CourseRevenueDetailsDto
{
    public CourseDto Course { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public decimal DistributedRevenue { get; set; }
    public decimal PendingRevenue { get; set; }
    public decimal PaidRevenue { get; set; }
    public IReadOnlyList<CourseRevenueShareDto> RevenueShares { get; set; } = [];
    public IReadOnlyList<RevenueShareBreakdownDto> DistributionDetails { get; set; } = [];
}

public sealed record AssignedDashboardDto(int AssignedCourses, int ActiveCourses,
    int StudentsCount, int ReservationsCount, decimal TotalRevenue, decimal MyRevenue,
    decimal PendingRevenue, decimal PaidRevenue, int UpcomingLiveSessions);

public sealed record RevenueSettlementDto(Guid Oid, Guid BeneficiaryUserId, string BeneficiaryName,
    string SettlementNumber, DateTime PeriodFrom, DateTime PeriodTo, decimal TotalAmount,
    RevenueSettlementStatus Status, DateTime? PaidAt, string? PaymentReference, string? Notes);

public sealed class CreateRevenueSettlementDto
{
    public Guid BeneficiaryUserId { get; set; }
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}

public sealed class UpdateRevenueSettlementStatusDto
{
    public RevenueSettlementStatus Status { get; set; }
    [MaxLength(100)] public string? PaymentReference { get; set; }
}
