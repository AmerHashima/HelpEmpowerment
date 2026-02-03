using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_outline")]
    public class CourseOutline : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        public string? TitleEn { get; set; }

        public string? TitleAr { get; set; }

        public int? OrderNo { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }

        public virtual ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
    }
}