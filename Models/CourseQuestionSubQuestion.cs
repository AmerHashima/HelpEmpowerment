using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

[Table("course_question_sub_questions")]
public class CourseQuestionSubQuestion : BaseEntity
{
    [Required]
    public Guid CourseQuestionOid { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string QuestionText { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(max)")]
    public string QuestionTextAr { get; set; } = string.Empty;

    public int OrderNo { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    [ForeignKey(nameof(CourseQuestionOid))]
    public virtual CourseQuestion CourseQuestion { get; set; } = null!;

    public virtual ICollection<CourseQuestionSubQuestionChoice> Choices { get; set; } = new List<CourseQuestionSubQuestionChoice>();
}
