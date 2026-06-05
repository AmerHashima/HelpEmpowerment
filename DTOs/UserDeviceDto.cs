namespace HelpEmpowermentApi.DTOs
{
    public class UserDeviceDto
    {
        public Guid Oid { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime FirstLoginDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }

    public class RemoveUserDeviceDto
    {
        public Guid DeviceOid { get; set; }
        public Guid UserId { get; set; }
    }
}
