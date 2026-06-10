namespace HelpEmpowermentApi.DTOs
{
    public class LinkDto
    {
        public Guid Oid { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public int? Icon { get; set; }
        public int? Type { get; set; }
        public bool? Active { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateLinkDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public int? Icon { get; set; }
        public int? Type { get; set; }
        public bool? Active { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateLinkDto
    {
        public Guid Oid { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public int? Icon { get; set; }
        public int? Type { get; set; }
        public bool? Active { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}