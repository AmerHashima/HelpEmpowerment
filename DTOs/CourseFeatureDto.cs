namespace HelpEmpowermentApi.DTOs
{
    public class CourseFeatureDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? FeatureHeader { get; set; }
        public string? FeatureDescription { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateCourseFeatureDto
    {
        public Guid CourseOid { get; set; }
        public string? FeatureHeader { get; set; }
        public string? FeatureDescription { get; set; }
        public int? OrderNo { get; set; }
    }

    public class UpdateCourseFeatureDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? FeatureHeader { get; set; }
        public string? FeatureDescription { get; set; }
        public int? OrderNo { get; set; }
    }
}