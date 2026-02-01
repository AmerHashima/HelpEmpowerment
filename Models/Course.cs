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

        public Guid? CourseLevelLookupId { get; set; }

        public Guid? CourseCategoryLookupId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseLevelLookupId))]
        public virtual AppLookupDetail? CourseLevelLookup { get; set; }

        [ForeignKey(nameof(CourseCategoryLookupId))]
        public virtual AppLookupDetail? CourseCategoryLookup { get; set; }

        public virtual ICollection<CoursesMasterExam> MasterExams { get; set; } = new List<CoursesMasterExam>();
    }
}