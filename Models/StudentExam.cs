using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_exams")]
    public class StudentExam : BaseEntity
    {
        [Required]
        public Guid StudentOid { get; set; }

        [Required]
        public Guid CoursesMasterExamOid { get; set; }

        public int AttemptNo { get; set; } = 1;

        public int? TotalScore { get; set; }

        public int? ObtainedScore { get; set; }

        public int? PassPercent { get; set; }

        public bool? IsPassed { get; set; }

        public Guid? ExamStatusLookupId { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(StudentOid))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(CoursesMasterExamOid))]
        public virtual CoursesMasterExam? MasterExam { get; set; }

        [ForeignKey(nameof(ExamStatusLookupId))]
        public virtual AppLookupDetail? ExamStatusLookup { get; set; }

        public virtual ICollection<StudentExamQuestion> ExamQuestions { get; set; } = new List<StudentExamQuestion>();
    }
}