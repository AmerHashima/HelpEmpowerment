using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("courses")]
    public class Course : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? CourseDescription { get; set; }

        // ✅ NEW FIELDS
        [Column(TypeName = "text")]
        public string? HeaderOne { get; set; }

        [Column(TypeName = "text")]
        public string? HeaderTwo { get; set; }

        [Column(TypeName = "text")]
        public string? Detail { get; set; }

        [Column(TypeName = "text")]
        public string? Price { get; set; }

        public Guid? CourseLevelLookupId { get; set; }

        public Guid? CourseCategoryLookupId { get; set; }

        // ✅ NEW FIELD
        public Guid? InstructorOid { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseLevelLookupId))]
        public virtual AppLookupDetail? CourseLevelLookup { get; set; }

        [ForeignKey(nameof(CourseCategoryLookupId))]
        public virtual AppLookupDetail? CourseCategoryLookup { get; set; }

        // ✅ NEW NAVIGATION
        [ForeignKey(nameof(InstructorOid))]
        public virtual User? Instructor { get; set; }

        public virtual ICollection<CoursesMasterExam> MasterExams { get; set; } = new List<CoursesMasterExam>();

        // ✅ NEW NAVIGATIONS
        public virtual ICollection<CourseFeature> Features { get; set; } = new List<CourseFeature>();
        public virtual ICollection<CourseOutline> Outlines { get; set; } = new List<CourseOutline>();
        public virtual ICollection<CourseVideo> Videos { get; set; } = new List<CourseVideo>();
        public virtual ICollection<CourseLiveSession> LiveSessions { get; set; } = new List<CourseLiveSession>();
        public virtual ICollection<CourseInstructor> Instructors { get; set; } = new List<CourseInstructor>();
        public virtual ICollection<CourseTargetAudience> TargetAudiences { get; set; } = new List<CourseTargetAudience>();
    }
}