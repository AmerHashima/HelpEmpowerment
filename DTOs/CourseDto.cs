namespace HelpEmpowermentApi.DTOs
{
    public class CourseDto
    {
        public Guid Oid { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public Guid? CourseLevelLookupId { get; set; }
        public string? CourseLevelName { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public string? CourseCategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseDto
    {
        public string? CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public Guid? CourseLevelLookupId { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseDto
    {
        public Guid Oid { get; set; }
        public string? CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public Guid? CourseLevelLookupId { get; set; }
        public Guid? CourseCategoryLookupId { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}