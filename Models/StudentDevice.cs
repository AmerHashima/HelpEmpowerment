using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_devices")]
    public class StudentDevice : BaseEntity
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeviceId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime FirstLoginDate { get; set; } = DateTime.UtcNow;

        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;
    }
}
