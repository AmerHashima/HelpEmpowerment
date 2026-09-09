using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Enums;

namespace HelpEmpowermentApi.Models;

[Table("course_revenue_shares")]
public class CourseRevenueShare : BaseEntity
{
    public Guid CourseId { get; set; }
    public Guid? BeneficiaryUserId { get; set; }
    public Guid ShareTypeLookupId { get; set; }
    public RevenueCalculationType CalculationType { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal Value { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Course Course { get; set; } = null!;
    public User? BeneficiaryUser { get; set; }
    public AppLookupDetail ShareType { get; set; } = null!;
    public ICollection<CourseRevenueDistribution> Distributions { get; set; } = [];
}
