namespace HelpEmpowermentApi.DTOs
{
    public class CourseVideoDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? VideoUrl { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? DurationSeconds { get; set; }
        public int? OrderNo { get; set; }
        public Guid? VideoTypeLookupId { get; set; }
        public string? VideoTypeName { get; set; }
        public bool IsPreview { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<CourseVideoAttachmentDto> Attachments { get; set; } = new();
    }

    public class CreateCourseVideoDto
    {
        public Guid CourseOid { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? VideoUrl { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? DurationSeconds { get; set; }
        public int? OrderNo { get; set; }
        public Guid? VideoTypeLookupId { get; set; }
        public bool IsPreview { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCourseVideoDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? VideoUrl { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? DurationSeconds { get; set; }
        public int? OrderNo { get; set; }
        public Guid? VideoTypeLookupId { get; set; }
        public bool IsPreview { get; set; }
        public bool IsActive { get; set; }
    }
}