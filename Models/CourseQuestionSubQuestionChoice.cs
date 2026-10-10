using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

[Table("course_question_sub_question_choices")]
public class CourseQuestionSubQuestionChoice : BaseEntity
{
    [Required]
    public Guid SubQuestionOid { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string ChoiceText { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(max)")]
    public string ChoiceTextAr { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
    public int OrderNo { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    [ForeignKey(nameof(SubQuestionOid))]
    public virtual CourseQuestionSubQuestion SubQuestion { get; set; } = null!;
}
