namespace HelpEmpowermentApi.DTOs
{
    public class CourseLiveSessionStudentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public Guid StudentOid { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public bool Active { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateCourseLiveSessionStudentDto
    {
        public Guid CourseOid { get; set; }
        public Guid StudentOid { get; set; }
        public bool Active { get; set; } = true;
    }

    public class UpdateCourseLiveSessionStudentDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public Guid StudentOid { get; set; }
        public bool Active { get; set; }
    }
}