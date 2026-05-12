using System.Text.Json;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.Services
{
    public class ServiceContactUsService : IServiceContactUsService
    {
        private readonly IServiceContactUsRepository _repository;
        private readonly IConfiguration _configuration;

        public ServiceContactUsService(IServiceContactUsRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<PagedResponse<ServiceContactUsDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<ServiceContactUsDto>
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
                return new PagedResponse<ServiceContactUsDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ServiceContactUsDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetWithDetailsAsync(id);
                if (entity == null)
                    return ApiResponse<ServiceContactUsDto>.ErrorResponse("Contact request not found");

                return ApiResponse<ServiceContactUsDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceContactUsDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ServiceContactUsDto>> GetByTicketNumberAsync(string ticketNumber)
        {
            try
            {
                var entity = await _repository.GetByTicketNumberAsync(ticketNumber);
                if (entity == null)
                    return ApiResponse<ServiceContactUsDto>.ErrorResponse("Ticket not found");

                return ApiResponse<ServiceContactUsDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceContactUsDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ServiceContactUsDto>>> GetByStudentIdAsync(Guid studentId)
        {
            try
            {
                var entities = await _repository.GetByStudentIdAsync(studentId);
                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponse<List<ServiceContactUsDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ServiceContactUsDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ServiceContactUsDto>>> GetUnreadAsync()
        {
            try
            {
                var entities = await _repository.GetUnreadAsync();
                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponse<List<ServiceContactUsDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ServiceContactUsDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync()
        {
            try
            {
                var count = await _repository.GetUnreadCountAsync();
                return ApiResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ServiceContactUsDto>> CreateAsync(CreateContactUsDto dto)
        {
            try
            {
                var ticketNumber = await _repository.GenerateTicketNumberAsync();

                var entity = new ServiceContactUs
                {
                    FullName = dto.FullName,
                    FullNameAr = dto.FullNameAr,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Mobile = dto.Mobile,
                    Subject = dto.Subject,
                    SubjectAr = dto.SubjectAr,
                    Message = dto.Message,
                    MessageAr = dto.MessageAr,
                    ContactTypeLookupId = dto.ContactTypeLookupId,
                    StudentId = dto.StudentId,
                    TicketNumber = ticketNumber,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                return ApiResponse<ServiceContactUsDto>.SuccessResponse(MapToDto(result!), $"Your request has been submitted. Ticket Number: {ticketNumber}");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceContactUsDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ServiceContactUsDto>> RespondAsync(RespondContactUsDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null)
                    return ApiResponse<ServiceContactUsDto>.ErrorResponse("Contact request not found");

                entity.Response = dto.Response;
                entity.RespondedAt = DateTime.UtcNow;
                entity.RespondedBy = dto.RespondedBy;
                entity.StatusLookupId = dto.StatusLookupId;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = dto.RespondedBy;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(dto.Oid);
                return ApiResponse<ServiceContactUsDto>.SuccessResponse(MapToDto(result!), "Response sent successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceContactUsDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(Guid id, Guid readBy)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<bool>.ErrorResponse("Contact request not found");

                entity.IsRead = true;
                entity.ReadAt = DateTime.UtcNow;
                entity.ReadBy = readBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, Guid statusLookupId)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<bool>.ErrorResponse("Contact request not found");

                entity.StatusLookupId = statusLookupId;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                return ApiResponse<bool>.SuccessResponse(true, "Status updated");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Contact request not found");

                return ApiResponse<bool>.SuccessResponse(true, "Contact request deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> UploadAttachmentAsync(Guid id, IFormFile file)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<string>.ErrorResponse("Contact request not found");

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return ApiResponse<string>.ErrorResponse($"File type '{ext}' is not allowed");

                var basePath = _configuration["FileStorage:ContactAttachmentsPath"] ?? "/var/www/attachments/contact";
                Directory.CreateDirectory(basePath);

                // Delete old file if exists
                if (!string.IsNullOrEmpty(entity.Attachments))
                {
                    var oldPath = Path.Combine(basePath, entity.Attachments);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(basePath, fileName);

                // Path traversal guard
                if (!Path.GetFullPath(filePath).StartsWith(Path.GetFullPath(basePath)))
                    return ApiResponse<string>.ErrorResponse("Invalid file path");

                await using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                entity.Attachments = fileName;
                entity.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);

                return ApiResponse<string>.SuccessResponse(fileName, "Attachment uploaded");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GetAttachmentFileNameAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<string>.ErrorResponse("Contact request not found");

                if (string.IsNullOrEmpty(entity.Attachments))
                    return ApiResponse<string>.ErrorResponse("No attachment found");

                return ApiResponse<string>.SuccessResponse(entity.Attachments);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAttachmentAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<bool>.ErrorResponse("Contact request not found");

                if (string.IsNullOrEmpty(entity.Attachments))
                    return ApiResponse<bool>.ErrorResponse("No attachment to delete");

                var basePath = _configuration["FileStorage:ContactAttachmentsPath"] ?? "/var/www/attachments/contact";
                var filePath = Path.Combine(basePath, entity.Attachments);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                entity.Attachments = null;
                entity.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);

                return ApiResponse<bool>.SuccessResponse(true, "Attachment deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static ServiceContactUsDto MapToDto(ServiceContactUs entity)
        {
            return new ServiceContactUsDto
            {
                Oid = entity.Oid,
                FullName = entity.FullName,
                FullNameAr = entity.FullNameAr,
                Email = entity.Email,
                Phone = entity.Phone,
                Mobile = entity.Mobile,
                Subject = entity.Subject,
                SubjectAr = entity.SubjectAr,
                Message = entity.Message,
                MessageAr = entity.MessageAr,
                ContactTypeLookupId = entity.ContactTypeLookupId,
                ContactTypeName = entity.ContactType?.LookupNameEn,
                PriorityLookupId = entity.PriorityLookupId,
                PriorityName = entity.Priority?.LookupNameEn,
                StatusLookupId = entity.StatusLookupId,
                StatusName = entity.Status?.LookupNameEn,
                Response = entity.Response,
                RespondedAt = entity.RespondedAt,
                TicketNumber = entity.TicketNumber,
                IsRead = entity.IsRead,
                AttachmentFileName = entity.Attachments,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}