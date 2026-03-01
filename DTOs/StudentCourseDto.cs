namespace HelpEmpowermentApi.DTOs
{
    public class StudentCourseDto
    {
        public Guid Oid { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid? PaymentStatusLookupId { get; set; }
        public string? PaymentStatusName { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public Guid? EnrollmentStatusLookupId { get; set; }
        public string? EnrollmentStatusName { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int ProgressPercentage { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public bool ExamSimulationReserv { get; set; } = false;
        public bool RecordedCourseReserv { get; set; } = false;
        public bool LiveCourseReserv { get; set; } = false;
        public bool IsCertificateIssued { get; set; }
        public DateTime? CertificateIssuedDate { get; set; }
        public string? CertificateNumber { get; set; }
    }

    public class CreateStudentCourseDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public decimal? Price { get; set; }
        public bool ExamSimulationReserv { get; set; } = false;
        public bool RecordedCourseReserv { get; set; } = false;
        public bool LiveCourseReserv { get; set; } = false;
        public decimal? DiscountAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentCourseDto
    {
        public Guid Oid { get; set; }
        public Guid? PaymentStatusLookupId { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool ExamSimulationReserv { get; set; } = false;
        public bool RecordedCourseReserv { get; set; } = false;
        public bool LiveCourseReserv { get; set; } = false;
        public Guid? EnrollmentStatusLookupId { get; set; }
        public int ProgressPercentage { get; set; }
        public int CompletedLessons { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}