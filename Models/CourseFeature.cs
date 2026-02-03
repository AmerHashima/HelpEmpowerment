using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_features")]
    public class CourseFeature : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        [MaxLength(150)]
        public string? FeatureHeader { get; set; }

        [Column(TypeName = "text")]
        public string? FeatureDescription { get; set; }

        public int? OrderNo { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }
    }
}