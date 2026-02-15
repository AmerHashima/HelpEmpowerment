using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("students")]
    public class Student : BaseEntity
    {
        [MaxLength(150)]
        public string? NameEn { get; set; }

        [MaxLength(150)]
        public string? NameAr { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Mobile { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Auth fields
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public virtual ICollection<CourseLiveSessionStudent> LiveSessionEnrollments { get; set; } = new List<CourseLiveSessionStudent>();
        
        // ✅ NEW: Navigation properties
        public virtual ICollection<StudentCourse> EnrolledCourses { get; set; } = new List<StudentCourse>();
        public virtual ICollection<StudentBasket> BasketItems { get; set; } = new List<StudentBasket>();
        public virtual ICollection<ServiceContactUs> ContactRequests { get; set; } = new List<ServiceContactUs>();
    }
}