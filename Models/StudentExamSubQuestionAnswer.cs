using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

[Table("student_exam_sub_question_answers")]
public class StudentExamSubQuestionAnswer : BaseEntity
{
    [Required]
    public Guid StudentExamQuestionOid { get; set; }

    [Required]
    public Guid SubQuestionOid { get; set; }

    [Required]
    public Guid SelectedChoiceOid { get; set; }

    public bool IsCorrect { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal AwardedScore { get; set; }

    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    [ForeignKey(nameof(StudentExamQuestionOid))]
    public virtual StudentExamQuestion StudentExamQuestion { get; set; } = null!;

    [ForeignKey(nameof(SubQuestionOid))]
    public virtual CourseQuestionSubQuestion SubQuestion { get; set; } = null!;

    [ForeignKey(nameof(SelectedChoiceOid))]
    public virtual CourseQuestionSubQuestionChoice SelectedChoice { get; set; } = null!;
}
