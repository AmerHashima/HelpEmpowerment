namespace HelpEmpowermentApi.DTOs
{
    public class LiveCourseDto
    {
        public Guid Oid { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseFormat { get; set; }
        public DateTime? StartDate { get; set; }
        public string? StartTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public int? NumberOfSessions { get; set; }
        public int? TotalHours { get; set; }
        public string? ScheduleNotes { get; set; }
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateLiveCourseDto
    {
        public string CourseName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? CourseFormat { get; set; }
        public DateTime? StartDate { get; set; }
        public string? StartTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public int? NumberOfSessions { get; set; }
        public int? TotalHours { get; set; }
        public string? ScheduleNotes { get; set; }
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateLiveCourseDto
    {
        public Guid Oid { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? CourseFormat { get; set; }
        public DateTime? StartDate { get; set; }
        public string? StartTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public int? NumberOfSessions { get; set; }
        public int? TotalHours { get; set; }
        public string? ScheduleNotes { get; set; }
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
