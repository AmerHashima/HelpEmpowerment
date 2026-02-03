namespace HelpEmpowermentApi.DTOs
{
    public class CourseOutlineDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<CourseContentDto> Contents { get; set; } = new();
    }

    public class CreateCourseOutlineDto
    {
        public Guid CourseOid { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public int? OrderNo { get; set; }
    }

    public class UpdateCourseOutlineDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public int? OrderNo { get; set; }
    }
}