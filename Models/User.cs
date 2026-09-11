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

        public Guid? RoleId { get; set; }

        public Guid? StatusLookupId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        // Navigation properties
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }

        [ForeignKey(nameof(StatusLookupId))]
        public virtual AppLookupDetail? StatusLookup { get; set; }

        public virtual ICollection<Course> InstructorCourses { get; set; } = new List<Course>();
        public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
        public virtual ICollection<UserCourseAssignment> CourseAssignments { get; set; } = new List<UserCourseAssignment>();
        public virtual ICollection<CourseRevenueShare> RevenueShares { get; set; } = new List<CourseRevenueShare>();
        public virtual ICollection<CourseRevenueDistribution> RevenueDistributions { get; set; } = new List<CourseRevenueDistribution>();
        public virtual ICollection<RevenueSettlement> RevenueSettlements { get; set; } = new List<RevenueSettlement>();
    }
}
