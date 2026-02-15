using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("service_contact_us")]
    public class ServiceContactUs : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FullNameAr { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public string? Mobile { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? SubjectAr { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? MessageAr { get; set; }

        // Contact Type: General, Support, Sales, Partnership, Feedback, Complaint
        public Guid? ContactTypeLookupId { get; set; }

        // Priority: Low, Medium, High, Urgent
        public Guid? PriorityLookupId { get; set; }

        // Status: New, InProgress, Resolved, Closed
        public Guid? StatusLookupId { get; set; }

        // Response Information
        [Column(TypeName = "nvarchar(max)")]
        public string? Response { get; set; }

        public DateTime? RespondedAt { get; set; }

        public Guid? RespondedBy { get; set; }

        // Tracking
        [MaxLength(50)]
        public string? TicketNumber { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        // If logged in student
        public Guid? StudentId { get; set; }

        // If logged in user
        public Guid? UserId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        public Guid? ReadBy { get; set; }

        // Attachments (JSON array of file URLs)
        [Column(TypeName = "nvarchar(max)")]
        public string? Attachments { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ContactTypeLookupId))]
        public virtual AppLookupDetail? ContactType { get; set; }

        [ForeignKey(nameof(PriorityLookupId))]
        public virtual AppLookupDetail? Priority { get; set; }

        [ForeignKey(nameof(StatusLookupId))]
        public virtual AppLookupDetail? Status { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(RespondedBy))]
        public virtual User? Responder { get; set; }
    }
}