using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_question_images")]
    public class CourseQuestionImage : BaseEntity
    {
        [Required]
        public Guid CourseQuestionOid { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public int OrderNo { get; set; }

        [ForeignKey(nameof(CourseQuestionOid))]
        public virtual CourseQuestion CourseQuestion { get; set; } = null!;
    }
}
