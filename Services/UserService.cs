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
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IAppLookupDetailRepository lookupDetailRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _roleRepository = roleRepository;
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
                var user = await _userRepository.GetByIdWithDetailsAsync(id);
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

                // Validate application role
                if (dto.RoleId.HasValue)
                {
                    var roleExists = await _roleRepository.ExistsAsync(
                        role => role.Oid == dto.RoleId.Value && !role.IsDeleted && role.IsActive);
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
                    RoleId = dto.RoleId,
                    StatusLookupId = dto.StatusLookupId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.AddAsync(user);
                var createdUserWithDetails = await _userRepository.GetByIdWithDetailsAsync(createdUser.Oid) ?? createdUser;
                return ApiResponse<UserDto>.SuccessResponse(MapToDto(createdUserWithDetails), "User created successfully");
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
                var user = await _userRepository.GetByIdWithDetailsAsync(dto.Oid);
                if (user == null)
                    return ApiResponse<UserDto>.ErrorResponse("User not found");

                // Validate unique username
                if (!await _userRepository.IsUsernameUniqueAsync(dto.Username, dto.Oid))
                    return ApiResponse<UserDto>.ErrorResponse("Username already exists");

                // Validate unique email
                if (!string.IsNullOrWhiteSpace(dto.Email) && !await _userRepository.IsEmailUniqueAsync(dto.Email, dto.Oid))
                    return ApiResponse<UserDto>.ErrorResponse("Email already exists");

                // Validate application role
                if (dto.RoleId.HasValue)
                {
                    var roleExists = await _roleRepository.ExistsAsync(
                        role => role.Oid == dto.RoleId.Value && !role.IsDeleted && role.IsActive);
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
                user.RoleId = dto.RoleId;
                user.StatusLookupId = dto.StatusLookupId;
                user.IsActive = dto.IsActive;
                user.UpdatedBy = dto.UpdatedBy;
                user.UpdatedAt = DateTime.UtcNow;

                var updatedUser = await _userRepository.UpdateAsync(user);
                var updatedUserWithDetails = await _userRepository.GetByIdWithDetailsAsync(updatedUser.Oid) ?? updatedUser;
                return ApiResponse<UserDto>.SuccessResponse(MapToDto(updatedUserWithDetails), "User updated successfully");
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

                if (dto.NewPassword != dto.ConfirmPassword)
                    return ApiResponse<bool>.ErrorResponse("Passwords do not match");

                // BCrypt hashes are salted, so hashing the same password again and
                // comparing the strings will never work. Verify against the stored hash.
                if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
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
                var user = await _userRepository.GetByUsernameAsync(username);
                
                if (user == null || !VerifyPassword(password, user.PasswordHash))
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
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
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
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private static bool VerifyPassword(string password, string hash)
        {
            if (hash.StartsWith("$2", StringComparison.Ordinal))
                return BCrypt.Net.BCrypt.Verify(password, hash);

            // Compatibility with users created before BCrypt was adopted.
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var legacyHash = Convert.ToBase64String(hashedBytes);
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(legacyHash),
                Encoding.UTF8.GetBytes(hash));
        }
    }
}
