using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("live_courses")]
    public class LiveCourse : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CourseFormat { get; set; }

        public DateTime? StartDate { get; set; }

        /// <summary>Range stored as string, e.g. "7 PM to 11 PM"</summary>
        [MaxLength(50)]
        public string? StartTime { get; set; }

        [MaxLength(10)]
        public string TimeZone { get; set; } = "KSA";

        public int? NumberOfSessions { get; set; }

        public int? TotalHours { get; set; }

        /// <summary>e.g. "2 sessions per week (Sunday and Wednesday)"</summary>
        [MaxLength(255)]
        public string? ScheduleNotes { get; set; }

        [MaxLength(255)]
        public string? WhatsAppLink { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
