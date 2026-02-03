namespace HelpEmpowermentApi.DTOs
{
    public class CourseLiveSessionDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public string? CourseName { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public DateTime? Date { get; set; }
        public int? MaxNumberReservation { get; set; }
        public int? NumberOfReservations { get; set; }
        public bool Active { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<CourseLiveSessionStudentDto> SessionStudents { get; set; } = new();
    }

    public class CreateCourseLiveSessionDto
    {
        public Guid CourseOid { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public DateTime? Date { get; set; }
        public int? MaxNumberReservation { get; set; }
        public int? NumberOfReservations { get; set; } = 0;
        public bool Active { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseLiveSessionDto
    {
        public Guid Oid { get; set; }
        public Guid CourseOid { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public DateTime? Date { get; set; }
        public int? MaxNumberReservation { get; set; }
        public int? NumberOfReservations { get; set; }
        public bool Active { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}