using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_videos")]
    public class CourseVideo : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        public string? NameEn { get; set; }

        public string? NameAr { get; set; }

        public string? VideoUrl { get; set; }

        [Column(TypeName = "text")]
        public string? DescriptionEn { get; set; }

        [Column(TypeName = "text")]
        public string? DescriptionAr { get; set; }

        public int? DurationSeconds { get; set; }

        public int? OrderNo { get; set; }

        public Guid? VideoTypeLookupId { get; set; }

        public bool IsPreview { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }

        [ForeignKey(nameof(VideoTypeLookupId))]
        public virtual AppLookupDetail? VideoTypeLookup { get; set; }

        public virtual ICollection<CourseVideoAttachment> Attachments { get; set; } = new List<CourseVideoAttachment>();
    }
}