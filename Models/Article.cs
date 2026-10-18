using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

/// <summary>Public article. Card metadata is deliberately separate from the structured detail blocks.</summary>
[Table("articles")]
public class Article : BaseEntity
{
    [Required, MaxLength(80)] public string Category { get; set; } = "general";
    [MaxLength(180)] public string? CourseCode { get; set; }
    [Required, MaxLength(180)] public string Slug { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string TitleEn { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string TitleAr { get; set; } = string.Empty;
    [MaxLength(1200)] public string? SummaryEn { get; set; }
    [MaxLength(1200)] public string? SummaryAr { get; set; }
    [MaxLength(500)] public string? CoverImagePath { get; set; }
    [MaxLength(300)] public string? CoverImageAltEn { get; set; }
    [MaxLength(300)] public string? CoverImageAltAr { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ReadingTimeMinutes { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Draft";
    public bool IsFeatured { get; set; }
    [Required, Column(TypeName = "nvarchar(max)")] public string ContentJson { get; set; } = "[]";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
