using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_courses")]
    public class StudentCourse : BaseEntity
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        // Payment Information
        public Guid? PaymentStatusLookupId { get; set; }  // Pending, Paid, Failed, Refunded
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }
        
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }  // CreditCard, PayPal, BankTransfer, etc.
        
        [MaxLength(100)]
        public string? TransactionId { get; set; }
        
        public DateTime? PaymentDate { get; set; }

        // Enrollment Information
        public Guid? EnrollmentStatusLookupId { get; set; }  // Active, Expired, Suspended, Completed
        
        public DateTime? EnrollmentDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }

        // Progress Tracking
        public int ProgressPercentage { get; set; } = 0;
        
        public int CompletedLessons { get; set; } = 0;
        
        public int TotalLessons { get; set; } = 0;

        // Certificate
        public bool IsCertificateIssued { get; set; } = false;
        public bool ExamSimulationReserv { get; set; } = false;
        public bool RecordedCourseReserv { get; set; } = false;
        public bool LiveCourseReserv { get; set; } = false;
        
        public DateTime? CertificateIssuedDate { get; set; }
        
        [MaxLength(100)]
        public string? CertificateNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = null!;

        [ForeignKey(nameof(PaymentStatusLookupId))]
        public virtual AppLookupDetail? PaymentStatus { get; set; }

        [ForeignKey(nameof(EnrollmentStatusLookupId))]
        public virtual AppLookupDetail? EnrollmentStatus { get; set; }
    }
}