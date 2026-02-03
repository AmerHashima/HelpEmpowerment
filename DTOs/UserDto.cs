namespace HelpEmpowermentApi.DTOs
{
    public class UserDto
    {
        public Guid Oid { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid? RoleLookupId { get; set; }
        public string? RoleName { get; set; }
        public Guid? StatusLookupId { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid? RoleLookupId { get; set; }
        public Guid? StatusLookupId { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateUserDto
    {
        public Guid Oid { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid? RoleLookupId { get; set; }
        public Guid? StatusLookupId { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class ChangePasswordDto
    {
        public Guid Oid { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public Guid? UpdatedBy { get; set; }
    }
}