using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.DTOs;

public sealed class StudentExportReportDto
{
    public Guid StudentId { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? PromoCode { get; set; }
    public float? PromoDiscount { get; set; }
    public DateTime? PromoValidTo { get; set; }
    public int NumberOfPeopleUsedPromo { get; set; }
    public decimal TotalMoneyWithPromo { get; set; }
    public List<StudentCourseExportDto> Courses { get; set; } = [];
}

public sealed class StudentCourseExportDto
{
    public Guid StudentCourseId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? PaymentStatusName { get; set; }
    public decimal? PaidAmount { get; set; }
    public string? EnrollmentStatusName { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<CourseReservationExportDto> Reservations { get; set; } = [];
}

public sealed class CourseReservationExportDto
{
    public Guid ReservationId { get; set; }
    public Guid CourseServiceId { get; set; }
    public string? ServiceName { get; set; }
    public DateTime? ReservationDate { get; set; }
    public DateTime? ReservationExpiryDate { get; set; }
    public bool IsReserved { get; set; }
    public decimal? ServicePrice { get; set; }
    public string? AddedBy { get; set; }
}

public sealed class StudentExportSearchRequest : DataRequest;

/// <summary>
/// TotalMoneyWithPromo is the total amount paid on authorised invoices that used the promo code.
/// </summary>
public sealed class PaginatedStudentExportResponse : PagedResponse<StudentExportReportDto>;
