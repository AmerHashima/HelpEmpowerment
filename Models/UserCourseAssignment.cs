using System.ComponentModel.DataAnnotations.Schema;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Models;

[Table("user_course_assignments")]
public class UserCourseAssignment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public Guid AssignmentTypeLookupId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public AppLookupDetail AssignmentType { get; set; } = null!;
}
