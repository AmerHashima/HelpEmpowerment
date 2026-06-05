using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_services")]
    public class CourseService : BaseEntity
    {
        [Required]
        public Guid CourseId { get; set; }           // FK → Course

        [Required]
        public Guid ServiceId { get; set; }          // FK → AppLookupDetail (EXAM_SIMULATION, RECORDED_COURSE, LIVE_COURSE)

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Active time in minutes (how long a reservation is valid or access duration)
        /// </summary>
        public int? ActiveTime { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = null!;

        [ForeignKey(nameof(ServiceId))]
        public virtual AppLookupDetail ServiceLookup { get; set; } = null!;

        public virtual ICollection<StudentCourseReservation> Reservations { get; set; } = new List<StudentCourseReservation>();
    }
}
