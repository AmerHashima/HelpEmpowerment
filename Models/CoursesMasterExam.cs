using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("courses_Master_Exam")]
    public class CoursesMasterExam : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = string.Empty;

        public Guid? CourseLevelLookupId { get; set; }

        public Guid? CourseCategoryLookupId { get; set; }

        // ADD THESE PROPERTIES
        public int? PassPercent { get; set; } = 60;

        public int? DurationMinutes { get; set; }
        public int? QuestionCount { get; set; }

        public int? MaxAttempts { get; set; }

        public bool ShuffleQuestions { get; set; } = true;

        public bool ShuffleAnswers { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }

        [ForeignKey(nameof(CourseLevelLookupId))]
        public virtual AppLookupDetail? CourseLevelLookup { get; set; }

        [ForeignKey(nameof(CourseCategoryLookupId))]
        public virtual AppLookupDetail? CourseCategoryLookup { get; set; }

        public virtual ICollection<CourseQuestion> Questions { get; set; } = new List<CourseQuestion>();
    }
}