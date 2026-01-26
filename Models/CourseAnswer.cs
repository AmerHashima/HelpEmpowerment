using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StandardArticture.Common;

namespace StandardArticture.Models
{
    [Table("course_answers")]
    public class CourseAnswer : BaseEntity
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string AnswerText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;

        public int? OrderNo { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(QuestionId))]
        public virtual CourseQuestion Question { get; set; } = null!;
    }
}