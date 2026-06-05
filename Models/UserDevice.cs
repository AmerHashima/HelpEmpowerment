using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("user_devices")]
    public class UserDevice : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeviceId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DeviceName { get; set; }

        [MaxLength(200)]
        public string? Browser { get; set; }

        [MaxLength(200)]
        public string? OperatingSystem { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime FirstLoginDate { get; set; } = DateTime.UtcNow;

        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
