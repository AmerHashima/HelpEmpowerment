using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseOutlineService : ICourseOutlineService
    {
        private readonly ICourseOutlineRepository _courseOutlineRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseOutlineService(ICourseOutlineRepository courseOutlineRepository, ICourseRepository courseRepository)
        {
            _courseOutlineRepository = courseOutlineRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseOutlineDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _courseOutlineRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseOutlineDto>
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
                return new PagedResponse<CourseOutlineDto>
                {
                    Success = false,
                    Message = $"Error retrieving course outlines: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseOutlineDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var outline = await _courseOutlineRepository.GetByIdAsync(id);
                if (outline == null)
                    return ApiResponse<CourseOutlineDto>.ErrorResponse("Course outline not found");

                return ApiResponse<CourseOutlineDto>.SuccessResponse(MapToDto(outline));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseOutlineDto>.ErrorResponse($"Error retrieving course outline: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseOutlineDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var outlines = await _courseOutlineRepository.GetByCourseIdAsync(courseId);
                var dtos = outlines.Select(MapToDto).ToList();

                return ApiResponse<List<CourseOutlineDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseOutlineDto>>.ErrorResponse($"Error retrieving course outlines: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseOutlineDto>> GetWithContentsAsync(Guid id)
        {
            try
            {
                var outline = await _courseOutlineRepository.GetWithContentsAsync(id);
                if (outline == null)
                    return ApiResponse<CourseOutlineDto>.ErrorResponse("Course outline not found");

                return ApiResponse<CourseOutlineDto>.SuccessResponse(MapToDtoWithContents(outline));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseOutlineDto>.ErrorResponse($"Error retrieving course outline: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseOutlineDto>> CreateAsync(CreateCourseOutlineDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseOutlineDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                var outline = new CourseOutline
                {
                    CourseOid = dto.CourseOid,
                    TitleEn = dto.TitleEn,
                    TitleAr = dto.TitleAr,
                    OrderNo = dto.OrderNo,
                    CreatedAt = DateTime.UtcNow
                };

                var createdOutline = await _courseOutlineRepository.AddAsync(outline);
                return ApiResponse<CourseOutlineDto>.SuccessResponse(MapToDto(createdOutline), "Course outline created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseOutlineDto>.ErrorResponse($"Error creating course outline: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseOutlineDto>> UpdateAsync(UpdateCourseOutlineDto dto)
        {
            try
            {
                var outline = await _courseOutlineRepository.GetByIdAsync(dto.Oid);
                if (outline == null)
                    return ApiResponse<CourseOutlineDto>.ErrorResponse("Course outline not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseOutlineDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                outline.CourseOid = dto.CourseOid;
                outline.TitleEn = dto.TitleEn;
                outline.TitleAr = dto.TitleAr;
                outline.OrderNo = dto.OrderNo;
                outline.UpdatedAt = DateTime.UtcNow;

                var updatedOutline = await _courseOutlineRepository.UpdateAsync(outline);
                return ApiResponse<CourseOutlineDto>.SuccessResponse(MapToDto(updatedOutline), "Course outline updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseOutlineDto>.ErrorResponse($"Error updating course outline: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _courseOutlineRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course outline not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course outline deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course outline: {ex.Message}");
            }
        }

        private CourseOutlineDto MapToDto(CourseOutline outline)
        {
            return new CourseOutlineDto
            {
                Oid = outline.Oid,
                CourseOid = outline.CourseOid,
                CourseName = outline.Course?.CourseName,
                TitleEn = outline.TitleEn,
                TitleAr = outline.TitleAr,
                OrderNo = outline.OrderNo,
                CreatedAt = outline.CreatedAt
            };
        }

        private CourseOutlineDto MapToDtoWithContents(CourseOutline outline)
        {
            return new CourseOutlineDto
            {
                Oid = outline.Oid,
                CourseOid = outline.CourseOid,
                CourseName = outline.Course?.CourseName,
                TitleEn = outline.TitleEn,
                TitleAr = outline.TitleAr,
                OrderNo = outline.OrderNo,
                CreatedAt = outline.CreatedAt,
                Contents = outline.Contents.Select(c => new CourseContentDto
                {
                    Oid = c.Oid,
                    CourseOutlineOid = c.CourseOutlineOid,
                    TitleEn = c.TitleEn,
                    TitleAr = c.TitleAr,
                    ContentTypeLookupId = c.ContentTypeLookupId,
                    ContentTypeName = c.ContentTypeLookup?.LookupNameEn,
                    ContentOid = c.ContentOid,
                    OrderNo = c.OrderNo,
                    IsFree = c.IsFree,
                    CreatedAt = c.CreatedAt
                }).ToList()
            };
        }
    }
}