using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HelpEmpowermentApi.DTOs;

public sealed class CourseTabContentDto
{
    public Guid Oid { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string TabKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int OrderNo { get; set; }
    public string Status { get; set; } = "Published";
    public JsonElement Content { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class UpdateCourseTabContentDto
{
    [Required, MaxLength(50)] public string CourseCode { get; set; } = string.Empty;
    [Required, MaxLength(40)] public string TabKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int OrderNo { get; set; }
    [Required] public JsonElement Content { get; set; }
    [RegularExpression("Draft|Published|Archived")] public string Status { get; set; } = "Published";
    public Guid? UpdatedBy { get; set; }
}
