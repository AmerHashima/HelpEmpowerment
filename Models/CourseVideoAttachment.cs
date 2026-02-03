using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_video_attachments")]
    public class CourseVideoAttachment : BaseEntity
    {
        [Required]
        public Guid CourseVideoOid { get; set; }

        public string? FileName { get; set; }

        public string? FileUrl { get; set; }

        public Guid? FileTypeLookupId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseVideoOid))]
        public virtual CourseVideo? CourseVideo { get; set; }

        [ForeignKey(nameof(FileTypeLookupId))]
        public virtual AppLookupDetail? FileTypeLookup { get; set; }
    }
}