using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("live_webinars")]
    public class LiveWebinar : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string WebinarName { get; set; } = string.Empty;

        public Guid? CourseOid { get; set; }

        [MaxLength(50)]
        public string? WebinarFormat { get; set; }

        public DateTime? WebinarDate { get; set; }

        [MaxLength(50)]
        public string? WebinarStartTime { get; set; }

        [MaxLength(50)]
        public string? WebinarEndTime { get; set; }

        [MaxLength(10)]
        public string TimeZone { get; set; } = "KSA";

        [MaxLength(255)]
        public string? WhatsAppLink { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }
    }
}
