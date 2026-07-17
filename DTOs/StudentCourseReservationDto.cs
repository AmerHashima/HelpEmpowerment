namespace HelpEmpowermentApi.DTOs
{
    public class StudentCourseReservationDto
    {
        public Guid Oid { get; set; }
        public Guid StudentCourseId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid CourseServiceId { get; set; }
        public string? CourseName { get; set; }      // from CourseService.Course
        public string? ServiceName { get; set; }     // from CourseService.ServiceLookup
        public DateTime? ReservationExpiryDate { get; set; }
        public decimal? ServicePrice { get; set; }
        public int? ActiveTime { get; set; }        // from CourseService
        public DateTime? ReservationDate { get; set; }
        public bool IsReserved { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateStudentCourseReservationDto
    {
        public Guid StudentCourseId { get; set; }
        public Guid CourseServiceId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public decimal? ServicePrice { get; set; }

        public bool IsReserved { get; set; } = false;
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentCourseReservationDto
    {
        public Guid Oid { get; set; }
        public Guid CourseServiceId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public bool IsReserved { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
