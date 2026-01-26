namespace StandardArticture.DTOs
{
    public class CoursesMasterExamDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseCode { get; set; }
        public Guid? CourseLevelLookupId { get; set; }
        public string? CourseLevelName { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public string? CourseCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCoursesMasterExamDto
    {
        public Guid CourseOid { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid? CourseLevelLookupId { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCoursesMasterExamDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public Guid? CourseLevelLookupId { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}