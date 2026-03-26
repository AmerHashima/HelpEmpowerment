using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var totalUsers = await _context.Users.Where(x => !x.IsDeleted).CountAsync();
            var totalStudentCourses = await _context.StudentCourses.Where(x => !x.IsDeleted).CountAsync();
            var totalCourses = await _context.Courses.Where(x => !x.IsDeleted).CountAsync();

            var certificateExamCounts = await _context.CoursesMasterExams
                .Where(e => !e.IsDeleted)
                .GroupBy(e => new { e.Oid, e.CourseName })
                .Select(g => new CertificateExamCountDto
                {
                    CertificateId = g.Key.Oid,
                    CertificateName = g.Key.CourseName,
                    ExamCount = _context.StudentExams.Count(se => se.CoursesMasterExamOid == g.Key.Oid && !se.IsDeleted)
                })
                .ToListAsync();

            var dto = new DashboardSummaryDto
            {
                TotalUsers = totalUsers,
                TotalStudentCourses = totalStudentCourses,
                TotalCourses = totalCourses,
                CertificateExamCounts = certificateExamCounts
            };
            return Ok(dto);
        }
    }
}
