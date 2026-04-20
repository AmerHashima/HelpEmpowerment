namespace HelpEmpowermentApi.DTOs
{
    public class LiveWebinarDto
    {
        public Guid Oid { get; set; }
        public string WebinarName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? WebinarFormat { get; set; }
        public DateTime? WebinarDate { get; set; }
        public string? WebinarStartTime { get; set; }
        public string? WebinarEndTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateLiveWebinarDto
    {
        public string WebinarName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? WebinarFormat { get; set; }
        public DateTime? WebinarDate { get; set; }
        public string? WebinarStartTime { get; set; }
        public string? WebinarEndTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateLiveWebinarDto
    {
        public Guid Oid { get; set; }
        public string WebinarName { get; set; } = string.Empty;
        public Guid? CourseOid { get; set; }
        public string? WebinarFormat { get; set; }
        public DateTime? WebinarDate { get; set; }
        public string? WebinarStartTime { get; set; }
        public string? WebinarEndTime { get; set; }
        public string TimeZone { get; set; } = "KSA";
        public string? WhatsAppLink { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
