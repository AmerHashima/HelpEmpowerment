using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_Live_Session_Studient")]
    public class CourseLiveSessionStudent : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        [Required]
        public Guid StudentOid { get; set; }

        public bool Active { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual CourseLiveSession? LiveSession { get; set; }

        [ForeignKey(nameof(StudentOid))]
        public virtual Student? Student { get; set; }
    }
}