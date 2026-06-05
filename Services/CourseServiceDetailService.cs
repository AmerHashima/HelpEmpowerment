using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseServiceDetailService : ICourseServiceDetailService
    {
        private readonly ICourseServiceRepository _repository;
        private readonly ICourseRepository _courseRepository;

        public CourseServiceDetailService(
            ICourseServiceRepository repository,
            ICourseRepository courseRepository)
        {
            _repository = repository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseServiceDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                return new PagedResponse<CourseServiceDto>
                {
                    Success = true,
                    Data = pagedResult.Items.Select(MapToDto).ToList(),
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<CourseServiceDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<CourseServiceDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetWithDetailsAsync(id);
                if (entity == null)
                    return ApiResponse<CourseServiceDto>.ErrorResponse("Course service not found");

                return ApiResponse<CourseServiceDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseServiceDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseServiceDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var entities = await _repository.GetByCourseIdAsync(courseId);
                return ApiResponse<List<CourseServiceDto>>.SuccessResponse(entities.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseServiceDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseServiceDto>> CreateAsync(CreateCourseServiceDto dto)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return ApiResponse<CourseServiceDto>.ErrorResponse("Course not found");

                var entity = new Models.CourseService
                {
                    CourseId = dto.CourseId,
                    ServiceId = dto.ServiceId,
                    Price = dto.Price,
                    ActiveTime = dto.ActiveTime,
                    IsActive = true,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                return ApiResponse<CourseServiceDto>.SuccessResponse(MapToDto(result!), "Course service created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseServiceDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseServiceDto>> UpdateAsync(UpdateCourseServiceDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null)
                    return ApiResponse<CourseServiceDto>.ErrorResponse("Course service not found");

                entity.ServiceId = dto.ServiceId;
                entity.Price = dto.Price;
                entity.ActiveTime = dto.ActiveTime;
                entity.IsActive = dto.IsActive;
                entity.UpdatedBy = dto.UpdatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(dto.Oid);
                return ApiResponse<CourseServiceDto>.SuccessResponse(MapToDto(result!), "Course service updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseServiceDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course service not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course service deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static CourseServiceDto MapToDto(Models.CourseService entity) => new()
        {
            Oid = entity.Oid,
            CourseId = entity.CourseId,
            CourseName = entity.Course?.CourseName,
            ServiceId = entity.ServiceId,
            ServiceName = entity.ServiceLookup?.LookupNameEn,
            Price = entity.Price,
            ActiveTime = entity.ActiveTime,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }
}
