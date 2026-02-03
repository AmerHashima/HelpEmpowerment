using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_target_audience")]
    public class CourseTargetAudience : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        [Column(TypeName = "text")]
        public string? DescriptionEn { get; set; }

        [Column(TypeName = "text")]
        public string? DescriptionAr { get; set; }

        public int? OrderNo { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }
    }
}