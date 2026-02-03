namespace HelpEmpowermentApi.DTOs
{
    public class CourseTargetAudienceDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseTargetAudienceDto
    {
        public Guid CourseOid { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? OrderNo { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseTargetAudienceDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public int? OrderNo { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}