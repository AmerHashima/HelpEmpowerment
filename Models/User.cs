using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        public Guid? RoleLookupId { get; set; }

        public Guid? StatusLookupId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RoleLookupId))]
        public virtual AppLookupDetail? RoleLookup { get; set; }

        [ForeignKey(nameof(StatusLookupId))]
        public virtual AppLookupDetail? StatusLookup { get; set; }

        public virtual ICollection<Course> InstructorCourses { get; set; } = new List<Course>();
    }
}