using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

[Table("course_tab_contents")]
public class CourseTabContent : BaseEntity
{
    [Required, MaxLength(50)] public string CourseCode { get; set; } = string.Empty;
    [Required, MaxLength(40)] public string TabKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int OrderNo { get; set; }
    [Required, Column(TypeName = "nvarchar(max)")] public string ContentJson { get; set; } = "{}";
    [MaxLength(20)] public string Status { get; set; } = "Published";
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
