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

        public Guid? QuestionStatusLookupId { get; set; }  

        public bool? IsCorrect { get; set; }

        public int? QuestionScore { get; set; }

        public int? ObtainedScore { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }


        [ForeignKey(nameof(QuestionStatusLookupId))]
        public virtual AppLookupDetail? QuestionStatus { get; set; }
        // Navigation properties
        [ForeignKey(nameof(StudentExamOid))]
        public virtual StudentExam? StudentExam { get; set; }

        [ForeignKey(nameof(QuestionOid))]
        public virtual CourseQuestion? Question { get; set; }
        //[ForeignKey(nameof(PaymentStatusLookupId))]
        //public virtual AppLookupDetail? PaymentStatus { get; set; }

        public virtual ICollection<StudentExamQuestionAnswer> Answers { get; set; } = new List<StudentExamQuestionAnswer>();
    }
}