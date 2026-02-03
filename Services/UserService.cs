using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace HelpEmpowermentApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;

        public UserService(IUserRepository userRepository, IAppLookupDetailRepository lookupDetailRepository)
        {
            _userRepository = userRepository;
            _lookupDetailRepository = lookupDetailRepository;
        }

        public async Task<PagedResponse<UserDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _userRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<UserDto>
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
                return new PagedResponse<UserDto>
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<UserDto>.ErrorResponse("User not found");

                return ApiResponse<UserDto>.SuccessResponse(MapToDto(user));
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                    return ApiResponse<UserDto>.ErrorResponse("User not found");

                return ApiResponse<UserDto>.SuccessResponse(MapToDto(user));
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto)
        {
            try
            {
                // Validate unique username
                if (!await _userRepository.IsUsernameUniqueAsync(dto.Username))
                    return ApiResponse<UserDto>.ErrorResponse("Username already exists");

                // Validate unique email
                if (!string.IsNullOrWhiteSpace(dto.Email) && !await _userRepository.IsEmailUniqueAsync(dto.Email))
                    return ApiResponse<UserDto>.ErrorResponse("Email already exists");

                // Validate Role Lookup
                if (dto.RoleLookupId.HasValue)
                {
                    var roleExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.RoleLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!roleExists)
                        return ApiResponse<UserDto>.ErrorResponse("Invalid Role. Please select a valid role.");
                }

                // Validate Status Lookup
                if (dto.StatusLookupId.HasValue)
                {
                    var statusExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.StatusLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!statusExists)
                        return ApiResponse<UserDto>.ErrorResponse("Invalid Status. Please select a valid status.");
                }

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = HashPassword(dto.Password),
                    Email = dto.Email,
                    RoleLookupId = dto.RoleLookupId,
                    StatusLookupId = dto.StatusLookupId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.AddAsync(user);
                return ApiResponse<UserDto>.SuccessResponse(MapToDto(createdUser), "User created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error creating user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateAsync(UpdateUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.Oid);
                if (user == null)
                    return ApiResponse<UserDto>.ErrorResponse("User not found");

                // Validate unique username
                if (!await _userRepository.IsUsernameUniqueAsync(dto.Username, dto.Oid))
                    return ApiResponse<UserDto>.ErrorResponse("Username already exists");

                // Validate unique email
                if (!string.IsNullOrWhiteSpace(dto.Email) && !await _userRepository.IsEmailUniqueAsync(dto.Email, dto.Oid))
                    return ApiResponse<UserDto>.ErrorResponse("Email already exists");

                // Validate Role Lookup
                if (dto.RoleLookupId.HasValue)
                {
                    var roleExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.RoleLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!roleExists)
                        return ApiResponse<UserDto>.ErrorResponse("Invalid Role. Please select a valid role.");
                }

                // Validate Status Lookup
                if (dto.StatusLookupId.HasValue)
                {
                    var statusExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.StatusLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!statusExists)
                        return ApiResponse<UserDto>.ErrorResponse("Invalid Status. Please select a valid status.");
                }

                user.Username = dto.Username;
                user.Email = dto.Email;
                user.RoleLookupId = dto.RoleLookupId;
                user.StatusLookupId = dto.StatusLookupId;
                user.IsActive = dto.IsActive;
                user.UpdatedBy = dto.UpdatedBy;
                user.UpdatedAt = DateTime.UtcNow;

                var updatedUser = await _userRepository.UpdateAsync(user);
                return ApiResponse<UserDto>.SuccessResponse(MapToDto(updatedUser), "User updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _userRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("User not found");

                return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.Oid);
                if (user == null)
                    return ApiResponse<bool>.ErrorResponse("User not found");

                // Verify current password
                if (user.PasswordHash != HashPassword(dto.CurrentPassword))
                    return ApiResponse<bool>.ErrorResponse("Current password is incorrect");

                user.PasswordHash = HashPassword(dto.NewPassword);
                user.UpdatedBy = dto.UpdatedBy;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error changing password: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> AuthenticateAsync(string username, string password)
        {
            try
            {
                var passwordHash = HashPassword(password);
                var user = await _userRepository.AuthenticateAsync(username, passwordHash);
                
                if (user == null)
                    return ApiResponse<UserDto>.ErrorResponse("Invalid username or password");

                return ApiResponse<UserDto>.SuccessResponse(MapToDto(user), "Authentication successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error authenticating user: {ex.Message}");
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Oid = user.Oid,
                Username = user.Username,
                Email = user.Email,
                RoleLookupId = user.RoleLookupId,
                RoleName = user.RoleLookup?.LookupNameEn,
                StatusLookupId = user.StatusLookupId,
                StatusName = user.StatusLookup?.LookupNameEn,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy
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