namespace HelpEmpowermentApi.DTOs
{
    public class AppLookupHeaderDto
    {
        public Guid Oid { get; set; }
        public string LookupCode { get; set; } = string.Empty;
        public string LookupNameAr { get; set; } = string.Empty;
        public string LookupNameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public List<AppLookupDetailDto> Details { get; set; } = new();
    }

    public class CreateAppLookupHeaderDto
    {
        public string LookupCode { get; set; } = string.Empty;
        public string LookupNameAr { get; set; } = string.Empty;
        public string LookupNameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateAppLookupHeaderDto
    {
        public Guid Oid { get; set; }
        public string LookupCode { get; set; } = string.Empty;
        public string LookupNameAr { get; set; } = string.Empty;
        public string LookupNameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AppLookupDetailDto
    {
        public Guid Oid { get; set; }
        public Guid LookupHeaderId { get; set; }
        public string? LookupHeaderCode { get; set; }
        public string LookupValue { get; set; } = string.Empty;
        public string? LookupNameEn { get; set; }
        public string? LookupNameAr { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class CreateAppLookupDetailDto
    {
        public Guid LookupHeaderId { get; set; }
        public string LookupValue { get; set; } = string.Empty;
        public string? LookupNameEn { get; set; }
        public string? LookupNameAr { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateAppLookupDetailDto
    {
        public Guid Oid { get; set; }
        public Guid LookupHeaderId { get; set; }
        public string LookupValue { get; set; } = string.Empty;
        public string? LookupNameEn { get; set; }
        public string? LookupNameAr { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
    }
}