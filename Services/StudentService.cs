using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using System.Security.Cryptography;
using System.Text;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly ApplicationDbContext _db;

        public StudentService(IStudentRepository studentRepository, IStudentCourseRepository studentCourseRepository, ApplicationDbContext db)
        {
            _studentRepository = studentRepository;
            _studentCourseRepository = studentCourseRepository;
            _db = db;
        }

        public async Task<PagedResponse<StudentDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _studentRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<StudentDto>
                {
                    Success = true,
                    Data = dtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<StudentDto>
                {
                    Success = false,
                    Message = $"Error retrieving students: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<StudentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                    return ApiResponse<StudentDto>.ErrorResponse("Student not found");

                return ApiResponse<StudentDto>.SuccessResponse(MapToDto(student));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.ErrorResponse($"Error retrieving student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> GetByUsernameAsync(string username)
        {
            try
            {
                var student = await _studentRepository.GetByUsernameAsync(username);
                if (student == null)
                    return ApiResponse<StudentDto>.ErrorResponse("Student not found");

                return ApiResponse<StudentDto>.SuccessResponse(MapToDto(student));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.ErrorResponse($"Error retrieving student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> CreateAsync(CreateStudentDto dto)
        {
            try
            {
                // Validate unique username
                if (!await _studentRepository.IsUsernameUniqueAsync(dto.Username))
                    return ApiResponse<StudentDto>.ErrorResponse("Username already exists");

                // Validate unique email
                if (!string.IsNullOrWhiteSpace(dto.Email) && !await _studentRepository.IsEmailUniqueAsync(dto.Email))
                    return ApiResponse<StudentDto>.ErrorResponse("Email already exists");

                var student = new Student
                {
                    NameEn = dto.NameEn,
                    NameAr = dto.NameAr,
                    Email = dto.Email,
                    Mobile = dto.Mobile,
                    Username = dto.Username,
                    PasswordHash = HashPassword(dto.Password),
                    IsActive = dto.IsActive,
                    PromoCode = dto.PromoCode,
                    PromoDiscount = dto.PromoDiscount,
                    PromoToDateValid = dto.PromoToDateValid,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdStudent = await _studentRepository.AddAsync(student);
                return ApiResponse<StudentDto>.SuccessResponse(MapToDto(createdStudent), "Student created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.ErrorResponse($"Error creating student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> UpdateAsync(UpdateStudentDto dto)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(dto.Oid);
                if (student == null)
                    return ApiResponse<StudentDto>.ErrorResponse("Student not found");

                // Validate unique username
                if (!await _studentRepository.IsUsernameUniqueAsync(dto.Username, dto.Oid))
                    return ApiResponse<StudentDto>.ErrorResponse("Username already exists");

                // Validate unique email
                if (!string.IsNullOrWhiteSpace(dto.Email) && !await _studentRepository.IsEmailUniqueAsync(dto.Email, dto.Oid))
                    return ApiResponse<StudentDto>.ErrorResponse("Email already exists");

                student.NameEn = dto.NameEn;
                student.NameAr = dto.NameAr;
                student.Email = dto.Email;
                student.Mobile = dto.Mobile;
                student.Username = dto.Username;
                student.IsActive = dto.IsActive;
                student.PromoCode = dto.PromoCode;
                student.PromoDiscount = dto.PromoDiscount;
                student.UsersUsedPromo = dto.UsersUsedPromo;
                student.TotalMoneyWithPromo = dto.TotalMoneyWithPromo;
                student.PromoToDateValid = dto.PromoToDateValid;
                student.UpdatedBy = dto.UpdatedBy;
                student.UpdatedAt = DateTime.UtcNow;

                var updatedStudent = await _studentRepository.UpdateAsync(student);
                return ApiResponse<StudentDto>.SuccessResponse(MapToDto(updatedStudent), "Student updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.ErrorResponse($"Error updating student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _studentRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Student not found");

                return ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> AuthenticateAsync(string username, string password)
        {
            try
            {
                var passwordHash = HashPassword(password);
                var student = await _studentRepository.AuthenticateAsync(username, passwordHash);
                
                if (student == null)
                    return ApiResponse<StudentDto>.ErrorResponse("Invalid username or password");

                return ApiResponse<StudentDto>.SuccessResponse(MapToDto(student), "Authentication successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentDto>.ErrorResponse($"Error authenticating student: {ex.Message}");
            }
        }

        private StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                Oid = student.Oid,
                NameEn = student.NameEn,
                NameAr = student.NameAr,
                Email = student.Email,
                Mobile = student.Mobile,
                Username = student.Username,
                IsActive = student.IsActive,
                PromoCode = student.PromoCode,
                PromoDiscount = student.PromoDiscount,
                UsersUsedPromo = student.UsersUsedPromo,
                TotalMoneyWithPromo = student.TotalMoneyWithPromo,
                PromoToDateValid = student.PromoToDateValid,
                CreatedAt = student.CreatedAt,
                CreatedBy = student.CreatedBy,
                UpdatedAt = student.UpdatedAt,
                UpdatedBy = student.UpdatedBy
            };
        }

        public async Task<PagedResponse<StudentWithCoursesDto>> GetStudentsWithCoursesAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _studentRepository.GetPagedAsync(request);

                var dtos = new List<StudentWithCoursesDto>();
                foreach (var student in pagedResult.Items)
                {
                    var studentCourses = await _studentCourseRepository.GetByStudentIdAsync(student.Oid);
                    var courseNames = studentCourses
                        .Where(sc => sc.Course != null)
                        .Select(sc => sc.Course!.CourseName)
                        .Distinct()
                        .ToList();

                    dtos.Add(new StudentWithCoursesDto
                    {
                        Oid = student.Oid,
                        NameEn = student.NameEn,
                        NameAr = student.NameAr,
                        Email = student.Email,
                        PromoCode = student.PromoCode,
                        PromoDiscount = student.PromoDiscount,
                        Mobile = student.Mobile,
                        Username = student.Username,
                        IsActive = student.IsActive,
                        Courses = courseNames
                    });
                }

                return new PagedResponse<StudentWithCoursesDto>
                {
                    Success = true,
                    Data = dtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<StudentWithCoursesDto>
                {
                    Success = false,
                    Message = $"Error retrieving students with courses: {ex.Message}"
                };
            }
        }

        public async Task<PaginatedStudentExportResponse> SearchExportReportAsync(
            StudentExportSearchRequest request,
            CancellationToken cancellationToken)
        {
            var errors = ValidateExportRequest(request);
            if (errors.Count > 0)
            {
                return new PaginatedStudentExportResponse
                {
                    Success = false,
                    Message = "Invalid export report request.",
                    Errors = errors
                };
            }

            var query = _db.Students.AsNoTracking().Where(student => !student.IsDeleted);

            foreach (var filter in request.Filters)
            {
                var value = filter.Value.Trim();
                query = filter.PropertyName.ToLowerInvariant() switch
                {
                    "nameen" => query.Where(student => student.NameEn != null && student.NameEn.Contains(value)),
                    "email" => query.Where(student => student.Email != null && student.Email.Contains(value)),
                    "isactive" => query.Where(student => student.IsActive == bool.Parse(value)),
                    _ => query
                };
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var sort = request.Sort.FirstOrDefault();
            query = (sort?.SortBy.ToLowerInvariant(), sort?.SortDirection.ToLowerInvariant()) switch
            {
                ("nameen", "desc") => query.OrderByDescending(student => student.NameEn).ThenBy(student => student.Oid),
                ("nameen", _) => query.OrderBy(student => student.NameEn).ThenBy(student => student.Oid),
                ("email", "desc") => query.OrderByDescending(student => student.Email).ThenBy(student => student.Oid),
                ("email", _) => query.OrderBy(student => student.Email).ThenBy(student => student.Oid),
                ("isactive", "desc") => query.OrderByDescending(student => student.IsActive).ThenBy(student => student.Oid),
                ("isactive", _) => query.OrderBy(student => student.IsActive).ThenBy(student => student.Oid),
                ("createdat", "asc") => query.OrderBy(student => student.CreatedAt).ThenBy(student => student.Oid),
                _ => query.OrderByDescending(student => student.CreatedAt).ThenBy(student => student.Oid)
            };

            if (!request.Pagination.GetAll)
            {
                query = query
                    .Skip(request.Pagination.PageNumber * request.Pagination.PageSize)
                    .Take(request.Pagination.PageSize);
            }

            var students = await query.Select(student => new StudentExportReportDto
            {
                StudentId = student.Oid,
                NameEn = student.NameEn,
                NameAr = student.NameAr,
                Email = student.Email,
                Mobile = student.Mobile,
                Username = student.Username,
                IsActive = student.IsActive,
                CreatedAt = student.CreatedAt,
                PromoCode = student.PromoCode,
                PromoDiscount = student.PromoDiscount,
                PromoValidTo = student.PromoToDateValid,
                NumberOfPeopleUsedPromo = string.IsNullOrWhiteSpace(student.PromoCode)
                    ? 0
                    : _db.Invoices
                        .Where(invoice => invoice.OwnerId.HasValue
                            && invoice.OwnerId.Value != student.Oid
                            && invoice.IsPaid
                            && invoice.PromoCode != null
                            && invoice.PromoCode.Trim().ToUpper() == student.PromoCode.Trim().ToUpper()
                            && invoice.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Authorised))
                        .Select(invoice => invoice.OwnerId!.Value)
                        .Distinct()
                        .Count(),
                TotalMoneyWithPromo = string.IsNullOrWhiteSpace(student.PromoCode)
                    ? 0m
                    : _db.Invoices
                        .Where(invoice => invoice.OwnerId.HasValue
                            && invoice.OwnerId.Value != student.Oid
                            && invoice.IsPaid
                            && invoice.PromoCode != null
                            && invoice.PromoCode.Trim().ToUpper() == student.PromoCode.Trim().ToUpper()
                            && invoice.PaymentTransactions.Any(payment => payment.Status == PaymentStatus.Authorised))
                        .Sum(invoice => (decimal?)invoice.TotalAmount) ?? 0m,
                Courses = student.EnrolledCourses
                    .Where(course => !course.IsDeleted)
                    .Select(course => new StudentCourseExportDto
                    {
                        StudentCourseId = course.Oid,
                        CourseId = course.CourseId,
                        CourseName = course.Course.CourseName,
                        PaymentStatusName = course.PaymentStatus != null ? course.PaymentStatus.LookupNameEn : null,
                        PaidAmount = course.PaidAmount,
                        EnrollmentStatusName = course.EnrollmentStatus != null ? course.EnrollmentStatus.LookupNameEn : null,
                        EnrollmentDate = course.EnrollmentDate,
                        ExpiryDate = course.ExpiryDate,
                        Reservations = course.Reservations
                            .Where(reservation => !reservation.IsDeleted)
                            .Select(reservation => new CourseReservationExportDto
                            {
                                ReservationId = reservation.Oid,
                                CourseServiceId = reservation.CourseServiceId,
                                ServiceName = reservation.CourseService.ServiceLookup.LookupNameEn,
                                ReservationDate = reservation.ReservationDate,
                                ReservationExpiryDate = reservation.ReservationExpiryDate,
                                IsReserved = reservation.IsReserved,
                                ServicePrice = reservation.ServicePrice,
                                AddedBy = reservation.CreatedBy.HasValue
                                    ? _db.Users.Where(user => user.Oid == reservation.CreatedBy.Value)
                                        .Select(user => user.Username).FirstOrDefault()
                                    : null
                            }).ToList()
                    }).ToList()
            }).ToListAsync(cancellationToken);

            return new PaginatedStudentExportResponse
            {
                Success = true,
                Data = students,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.GetAll ? Math.Max(totalCount, 1) : request.Pagination.PageSize
            };
        }

        private static List<string> ValidateExportRequest(StudentExportSearchRequest request)
        {
            var errors = new List<string>();
            var filterFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "nameEn", "email", "isActive" };
            var sortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "nameEn", "email", "isActive", "createdAt" };

            if (!request.Pagination.GetAll && (request.Pagination.PageNumber < 0 || request.Pagination.PageSize is < 1 or > 500))
                errors.Add("Pagination requires pageNumber >= 0 and pageSize between 1 and 500.");

            foreach (var filter in request.Filters)
            {
                if (!filterFields.Contains(filter.PropertyName))
                    errors.Add($"Unsupported filter field '{filter.PropertyName}'.");
                else if (filter.PropertyName.Equals("isActive", StringComparison.OrdinalIgnoreCase)
                    && (filter.Operation != FilterOperation.Equal || !bool.TryParse(filter.Value, out _)))
                    errors.Add("The isActive filter requires operation Equal and a true or false value.");
                else if (!filter.PropertyName.Equals("isActive", StringComparison.OrdinalIgnoreCase)
                    && filter.Operation != FilterOperation.Contains)
                    errors.Add($"The {filter.PropertyName} filter only supports Contains.");
            }

            foreach (var sort in request.Sort)
            {
                if (!sortFields.Contains(sort.SortBy)) errors.Add($"Unsupported sort field '{sort.SortBy}'.");
                if (!sort.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    && !sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    errors.Add($"Unsupported sort direction '{sort.SortDirection}'.");
            }

            return errors;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
