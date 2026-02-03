using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_contents")]
    public class CourseContent : BaseEntity
    {
        [Required]
        public Guid CourseOutlineOid { get; set; }

        public string? TitleEn { get; set; }

        public string? TitleAr { get; set; }

        public Guid? ContentTypeLookupId { get; set; }

        public Guid? ContentOid { get; set; }

        public int? OrderNo { get; set; }

        public bool IsFree { get; set; } = false;

        // Navigation properties
        [ForeignKey(nameof(CourseOutlineOid))]
        public virtual CourseOutline? CourseOutline { get; set; }

        [ForeignKey(nameof(ContentTypeLookupId))]
        public virtual AppLookupDetail? ContentTypeLookup { get; set; }
    }
}