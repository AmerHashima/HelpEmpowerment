using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_answers")]
    public class CourseAnswer : BaseEntity
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string AnswerText { get; set; } = string.Empty;
        public bool Question_Ask { get; set; } = false;

        public Guid? CorrectAnswerOid { get; set; }
        public bool IsCorrect { get; set; } = false;

        public int? OrderNo { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(QuestionId))]
        public virtual CourseQuestion Question { get; set; } = null!;

        [ForeignKey(nameof(CorrectAnswerOid))]
        public virtual CourseAnswer CourseAnswerData { get; set; } = null!;
    }
}