using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("student_course_reservations")]
    public class StudentCourseReservation : BaseEntity
    {
        [Required]
        public Guid StudentCourseId { get; set; }    // FK → StudentCourse

        [Required]
        public Guid CourseServiceId { get; set; }    // FK → CourseService (replaces ServiceId)

        public DateTime? ReservationDate { get; set; }

        public bool IsReserved { get; set; } = false;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(StudentCourseId))]
        public virtual StudentCourse StudentCourse { get; set; } = null!;

        [ForeignKey(nameof(CourseServiceId))]
        public virtual CourseService CourseService { get; set; } = null!;
    }
}
