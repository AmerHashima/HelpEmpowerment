namespace HelpEmpowermentApi.DTOs
{
    public class CourseInstructorDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public string? HeaderAr { get; set; }
        public string? HeaderEn { get; set; }
        public string? BioEn { get; set; }
        public string? BioAr { get; set; }
        public int? ExperienceYears { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseInstructorDto
    {
        public Guid CourseOid { get; set; }
        public string? HeaderAr { get; set; }
        public string? HeaderEn { get; set; }
        public string? BioEn { get; set; }
        public string? BioAr { get; set; }
        public int? ExperienceYears { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseInstructorDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? HeaderAr { get; set; }
        public string? HeaderEn { get; set; }
        public string? BioEn { get; set; }
        public string? BioAr { get; set; }
        public int? ExperienceYears { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}