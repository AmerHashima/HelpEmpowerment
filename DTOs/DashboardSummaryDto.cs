namespace HelpEmpowermentApi.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalStudentCourses { get; set; }
        public int TotalCourses { get; set; }
        public List<CertificateExamCountDto> CertificateExamCounts { get; set; } = new();
    }

    public class CertificateExamCountDto
    {
        public Guid CertificateId { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public int ExamCount { get; set; }
    }
}
