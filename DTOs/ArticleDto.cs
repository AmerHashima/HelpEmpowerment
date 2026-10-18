using System.ComponentModel.DataAnnotations;

namespace HelpEmpowermentApi.DTOs;

public sealed class ArticleBlockDto
{
    [Required, MaxLength(30)] public string Type { get; set; } = "details"; // main-header, sub-header, details, bullet-list, numbered-list, image
    [MaxLength(5000)] public string? TextEn { get; set; }
    [MaxLength(5000)] public string? TextAr { get; set; }
    public List<string> ItemsEn { get; set; } = [];
    public List<string> ItemsAr { get; set; } = [];
    [MaxLength(500)] public string? ImagePath { get; set; }
    [MaxLength(300)] public string? AltEn { get; set; }
    [MaxLength(300)] public string? AltAr { get; set; }
    [MaxLength(500)] public string? CaptionEn { get; set; }
    [MaxLength(500)] public string? CaptionAr { get; set; }
    public int OrderNo { get; set; }
}

public class SaveArticleDto
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
    [Range(0, 999)] public int ReadingTimeMinutes { get; set; }
    [Required, RegularExpression("^(Draft|Published|Archived)$", ErrorMessage = "Status must be Draft, Published, or Archived.")] public string Status { get; set; } = "Draft";
    public bool IsFeatured { get; set; }
    [MaxLength(250)] public List<ArticleBlockDto> Blocks { get; set; } = [];
}

public sealed class UpdateArticleDto : SaveArticleDto { [Required] public Guid Oid { get; set; } }

public sealed class ArticleDto : SaveArticleDto
{
    public Guid Oid { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
