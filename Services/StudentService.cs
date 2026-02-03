using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace HelpEmpowermentApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
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
                CreatedAt = student.CreatedAt,
                CreatedBy = student.CreatedBy,
                UpdatedAt = student.UpdatedAt,
                UpdatedBy = student.UpdatedBy
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}