using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.Payments.Domain;

namespace HelpEmpowermentApi.Models;

[Table("course_revenue_distributions")]
public class CourseRevenueDistribution : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid InvoiceItemId { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public Guid CourseId { get; set; }
    public Guid RevenueShareId { get; set; }
    public Guid? BeneficiaryUserId { get; set; }
    public Guid ShareTypeLookupId { get; set; }
    public RevenueCalculationType CalculationType { get; set; }
    [Column(TypeName = "decimal(9,4)")] public decimal? AppliedPercentage { get; set; }
    [Column(TypeName = "decimal(18,4)")] public decimal? AppliedFixedAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal BaseAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal ShareAmount { get; set; }
    public RevenueDistributionStatus Status { get; set; } = RevenueDistributionStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public Guid? SettlementId { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public InvoiceItem InvoiceItem { get; set; } = null!;
    public PaymentTransaction PaymentTransaction { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public CourseRevenueShare RevenueShare { get; set; } = null!;
    public User? BeneficiaryUser { get; set; }
    public AppLookupDetail ShareType { get; set; } = null!;
    public RevenueSettlement? Settlement { get; set; }
}
