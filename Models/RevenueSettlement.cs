using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Enums;

namespace HelpEmpowermentApi.Models;

[Table("revenue_settlements")]
public class RevenueSettlement : BaseEntity
{
    public Guid BeneficiaryUserId { get; set; }
    [MaxLength(50)] public string SettlementNumber { get; set; } = string.Empty;
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TotalAmount { get; set; }
    public RevenueSettlementStatus Status { get; set; } = RevenueSettlementStatus.Draft;
    public DateTime? PaidAt { get; set; }
    [MaxLength(100)] public string? PaymentReference { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public User BeneficiaryUser { get; set; } = null!;
    public ICollection<CourseRevenueDistribution> Distributions { get; set; } = [];
}
