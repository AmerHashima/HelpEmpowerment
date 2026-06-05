namespace HelpEmpowermentApi.DTOs
{
    public class CourseServiceDto
    {
        public Guid Oid { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal Price { get; set; }
        public int? ActiveTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseServiceDto
    {
        public Guid CourseId { get; set; }
        public Guid ServiceId { get; set; }
        public decimal Price { get; set; } = 0;
        public int? ActiveTime { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseServiceDto
    {
        public Guid Oid { get; set; }
        public Guid ServiceId { get; set; }
        public decimal Price { get; set; }
        public int? ActiveTime { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
