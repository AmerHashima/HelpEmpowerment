using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models
{
    [Table("course_Live_Session")]
    public class CourseLiveSession : BaseEntity
    {
        [Required]
        public Guid CourseOid { get; set; }

        public DateTime? TimeFrom { get; set; }

        public DateTime? TimeTo { get; set; }

        public DateTime? Date { get; set; }

        public int? MaxNumberReservation { get; set; }

        public int? NumberOfReservations { get; set; }

        public bool Active { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CourseOid))]
        public virtual Course? Course { get; set; }

        public virtual ICollection<CourseLiveSessionStudent> SessionStudents { get; set; } = new List<CourseLiveSessionStudent>();
    }
}