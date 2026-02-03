using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_instructors")]
    public class CourseInstructor : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        [Column(TypeName = "text")]
        public string? HeaderAr { get; set; }

        [Column(TypeName = "text")]
        public string? HeaderEn { get; set; }

        [Column(TypeName = "text")]
        public string? BioEn { get; set; }

        [Column(TypeName = "text")]
        public string? BioAr { get; set; }

        public int? ExperienceYears { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }
    }
}