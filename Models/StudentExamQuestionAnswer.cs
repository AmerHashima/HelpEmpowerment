using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_exam_question_answers")]
    public class StudentExamQuestionAnswer : BaseEntity
    {
        [Required]
        public Guid StudentExamQuestionOid { get; set; }

        [Required]
        public Guid SelectedAnswerOid { get; set; }

        public Guid? AnswerSelectedAnswerOid { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(StudentExamQuestionOid))]
        public virtual StudentExamQuestion? StudentExamQuestion { get; set; }

        [ForeignKey(nameof(SelectedAnswerOid))]
        public virtual CourseAnswer? SelectedAnswer { get; set; }

        [ForeignKey(nameof(AnswerSelectedAnswerOid))]
        public virtual CourseAnswer? AnswerSelectedAnswer { get; set; }
    }
}
