namespace HelpEmpowermentApi.DTOs
{
    public class CourseContentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOutlineOid { get; set; }
        public string? OutlineTitle { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public Guid? ContentTypeLookupId { get; set; }
        public string? ContentTypeName { get; set; }
        public Guid? ContentOid { get; set; }
        public int? OrderNo { get; set; }
        public bool IsFree { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateCourseContentDto
    {
        public Guid CourseOutlineOid { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public Guid? ContentTypeLookupId { get; set; }
        public Guid? ContentOid { get; set; }
        public int? OrderNo { get; set; }
        public bool IsFree { get; set; } = false;
    }

    public class UpdateCourseContentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOutlineOid { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public Guid? ContentTypeLookupId { get; set; }
        public Guid? ContentOid { get; set; }
        public int? OrderNo { get; set; }
        public bool IsFree { get; set; }
    }
}