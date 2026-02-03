namespace HelpEmpowermentApi.DTOs
{
    public class CourseVideoAttachmentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseVideoOid { get; set; }
        public string? VideoName { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public Guid? FileTypeLookupId { get; set; }
        public string? FileTypeName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateCourseVideoAttachmentDto
    {
        public Guid CourseVideoOid { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public Guid? FileTypeLookupId { get; set; }
    }

    public class UpdateCourseVideoAttachmentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseVideoOid { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public Guid? FileTypeLookupId { get; set; }
    }
}