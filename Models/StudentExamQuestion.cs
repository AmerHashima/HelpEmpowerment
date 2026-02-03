using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_exam_questions")]
    public class StudentExamQuestion : BaseEntity
    {
        [Required]
        public Guid StudentExamOid { get; set; }

        [Required]
        public Guid QuestionOid { get; set; }

        public Guid? SelectedAnswerOid { get; set; }

        public bool? IsCorrect { get; set; }

        public int? QuestionScore { get; set; }

        public int? ObtainedScore { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(StudentExamOid))]
        public virtual StudentExam? StudentExam { get; set; }

        [ForeignKey(nameof(QuestionOid))]
        public virtual CourseQuestion? Question { get; set; }

        [ForeignKey(nameof(SelectedAnswerOid))]
        public virtual CourseAnswer? SelectedAnswer { get; set; }
    }
}