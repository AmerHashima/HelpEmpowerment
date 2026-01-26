using StandardArticture.Common;
using StandardArticture.DTOs;
using StandardArticture.IRepositories;
using StandardArticture.IServices;
using StandardArticture.Models;

namespace StandardArticture.Services
{
    public class AppLookupService : IAppLookupService
    {
        private readonly IAppLookupHeaderRepository _headerRepository;
        private readonly IAppLookupDetailRepository _detailRepository;

        public AppLookupService(IAppLookupHeaderRepository headerRepository, IAppLookupDetailRepository detailRepository)
        {
            _headerRepository = headerRepository;
            _detailRepository = detailRepository;
        }

        // Header methods
        public async Task<PagedResponse<AppLookupHeaderDto>> GetHeadersPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _headerRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapHeaderToDto).ToList();

                return new PagedResponse<AppLookupHeaderDto>
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
                return new PagedResponse<AppLookupHeaderDto>
                {
                    Success = false,
                    Message = $"Error retrieving lookup headers: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AppLookupHeaderDto>> GetHeaderByIdAsync(Guid id)
        {
            try
            {
                var header = await _headerRepository.GetWithDetailsAsync(id);
                if (header == null)
                    return ApiResponse<AppLookupHeaderDto>.ErrorResponse("Lookup header not found");

                return ApiResponse<AppLookupHeaderDto>.SuccessResponse(MapHeaderToDto(header));
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupHeaderDto>.ErrorResponse($"Error retrieving lookup header: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AppLookupHeaderDto>> GetHeaderByCodeAsync(string lookupCode)
        {
            try
            {
                var header = await _headerRepository.GetByCodeAsync(lookupCode);
                if (header == null)
                    return ApiResponse<AppLookupHeaderDto>.ErrorResponse("Lookup header not found");

                return ApiResponse<AppLookupHeaderDto>.SuccessResponse(MapHeaderToDto(header));
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupHeaderDto>.ErrorResponse($"Error retrieving lookup header: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AppLookupHeaderDto>> CreateHeaderAsync(CreateAppLookupHeaderDto dto)
        {
            try
            {
                if (!await _headerRepository.IsLookupCodeUniqueAsync(dto.LookupCode))
                    return ApiResponse<AppLookupHeaderDto>.ErrorResponse("Lookup code already exists");

                var header = new AppLookupHeader
                {
                    LookupCode = dto.LookupCode,
                    LookupNameAr = dto.LookupNameAr,
                    LookupNameEn = dto.LookupNameEn,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _headerRepository.AddAsync(header);
                return ApiResponse<AppLookupHeaderDto>.SuccessResponse(MapHeaderToDto(created), "Lookup header created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupHeaderDto>.ErrorResponse($"Error creating lookup header: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AppLookupHeaderDto>> UpdateHeaderAsync(UpdateAppLookupHeaderDto dto)
        {
            try
            {
                var header = await _headerRepository.GetByIdAsync(dto.Oid);
                if (header == null)
                    return ApiResponse<AppLookupHeaderDto>.ErrorResponse("Lookup header not found");

                if (!await _headerRepository.IsLookupCodeUniqueAsync(dto.LookupCode, dto.Oid))
                    return ApiResponse<AppLookupHeaderDto>.ErrorResponse("Lookup code already exists");

                header.LookupCode = dto.LookupCode;
                header.LookupNameAr = dto.LookupNameAr;
                header.LookupNameEn = dto.LookupNameEn;
                header.IsActive = dto.IsActive;

                var updated = await _headerRepository.UpdateAsync(header);
                return ApiResponse<AppLookupHeaderDto>.SuccessResponse(MapHeaderToDto(updated), "Lookup header updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupHeaderDto>.ErrorResponse($"Error updating lookup header: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteHeaderAsync(Guid id)
        {
            try
            {
                var result = await _headerRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Lookup header not found");

                return ApiResponse<bool>.SuccessResponse(true, "Lookup header deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting lookup header: {ex.Message}");
            }
        }

        // Detail methods
        public async Task<PagedResponse<AppLookupDetailDto>> GetDetailsPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _detailRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapDetailToDto).ToList();

                return new PagedResponse<AppLookupDetailDto>
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
                return new PagedResponse<AppLookupDetailDto>
                {
                    Success = false,
                    Message = $"Error retrieving lookup details: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<AppLookupDetailDto>> GetDetailByIdAsync(Guid id)
        {
            try
            {
                var detail = await _detailRepository.GetByIdAsync(id);
                if (detail == null)
                    return ApiResponse<AppLookupDetailDto>.ErrorResponse("Lookup detail not found");

                return ApiResponse<AppLookupDetailDto>.SuccessResponse(MapDetailToDto(detail));
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupDetailDto>.ErrorResponse($"Error retrieving lookup detail: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<AppLookupDetailDto>>> GetDetailsByHeaderIdAsync(Guid headerId)
        {
            try
            {
                var details = await _detailRepository.GetByHeaderIdAsync(headerId);
                var dtos = details.Select(MapDetailToDto).ToList();

                return ApiResponse<List<AppLookupDetailDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AppLookupDetailDto>>.ErrorResponse($"Error retrieving lookup details: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<AppLookupDetailDto>>> GetDetailsByHeaderCodeAsync(string headerCode)
        {
            try
            {
                var details = await _detailRepository.GetByHeaderCodeAsync(headerCode);
                var dtos = details.Select(MapDetailToDto).ToList();

                return ApiResponse<List<AppLookupDetailDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AppLookupDetailDto>>.ErrorResponse($"Error retrieving lookup details: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AppLookupDetailDto>> CreateDetailAsync(CreateAppLookupDetailDto dto)
        {
            try
            {
                var detail = new AppLookupDetail
                {
                    LookupHeaderId = dto.LookupHeaderId,
                    LookupValue = dto.LookupValue,
                    LookupNameEn = dto.LookupNameEn,
                    LookupNameAr = dto.LookupNameAr,
                    OrderNo = dto.OrderNo,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _detailRepository.AddAsync(detail);
                return ApiResponse<AppLookupDetailDto>.SuccessResponse(MapDetailToDto(created), "Lookup detail created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupDetailDto>.ErrorResponse($"Error creating lookup detail: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AppLookupDetailDto>> UpdateDetailAsync(UpdateAppLookupDetailDto dto)
        {
            try
            {
                var detail = await _detailRepository.GetByIdAsync(dto.Oid);
                if (detail == null)
                    return ApiResponse<AppLookupDetailDto>.ErrorResponse("Lookup detail not found");

                detail.LookupHeaderId = dto.LookupHeaderId;
                detail.LookupValue = dto.LookupValue;
                detail.LookupNameEn = dto.LookupNameEn;
                detail.LookupNameAr = dto.LookupNameAr;
                detail.OrderNo = dto.OrderNo;
                detail.IsActive = dto.IsActive;

                var updated = await _detailRepository.UpdateAsync(detail);
                return ApiResponse<AppLookupDetailDto>.SuccessResponse(MapDetailToDto(updated), "Lookup detail updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AppLookupDetailDto>.ErrorResponse($"Error updating lookup detail: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDetailAsync(Guid id)
        {
            try
            {
                var result = await _detailRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Lookup detail not found");

                return ApiResponse<bool>.SuccessResponse(true, "Lookup detail deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting lookup detail: {ex.Message}");
            }
        }

        private static AppLookupHeaderDto MapHeaderToDto(AppLookupHeader header)
        {
            return new AppLookupHeaderDto
            {
                Oid = header.Oid,
                LookupCode = header.LookupCode,
                LookupNameAr = header.LookupNameAr,
                LookupNameEn = header.LookupNameEn,
                IsActive = header.IsActive,
                CreatedAt = header.CreatedAt,
                CreatedBy = header.CreatedBy,
                Details = header.LookupDetails?.Select(MapDetailToDto).ToList() ?? new()
            };
        }

        private static AppLookupDetailDto MapDetailToDto(AppLookupDetail detail)
        {
            return new AppLookupDetailDto
            {
                Oid = detail.Oid,
                LookupHeaderId = detail.LookupHeaderId,
                LookupHeaderCode = detail.LookupHeader?.LookupCode,
                LookupValue = detail.LookupValue,
                LookupNameEn = detail.LookupNameEn,
                LookupNameAr = detail.LookupNameAr,
                OrderNo = detail.OrderNo,
                IsActive = detail.IsActive,
                CreatedAt = detail.CreatedAt,
                CreatedBy = detail.CreatedBy
            };
        }
    }
}