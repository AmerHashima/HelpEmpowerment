using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StandardArticture.Common;

namespace StandardArticture.Models
{
    [Table("course_questions")]
    public class CourseQuestion : BaseEntity
    {
        [Required]
        public Guid CoursesMasterExamOid { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string QuestionText { get; set; } = string.Empty;

        public Guid? QuestionTypeLookupId { get; set; }

        public int QuestionScore { get; set; } = 1;

        public int? OrderNo { get; set; }

        public bool IsActive { get; set; } = true;

        public bool CorrectAnswer { get; set; } = false;

        /// <summary>
        /// Indicates if this is a question
        /// </summary>
        public bool Question { get; set; } = false;

        public Guid? CorrectChoiceOid { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CoursesMasterExamOid))]
        public virtual CoursesMasterExam MasterExam { get; set; } = null!;

        [ForeignKey(nameof(QuestionTypeLookupId))]
        public virtual AppLookupDetail? QuestionTypeLookup { get; set; }

        [ForeignKey(nameof(CorrectChoiceOid))]
        public virtual CourseQuestion? CorrectChoice { get; set; }

        public virtual ICollection<CourseAnswer> Answers { get; set; } = new List<CourseAnswer>();
    }
}