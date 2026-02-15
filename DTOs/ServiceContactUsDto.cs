namespace HelpEmpowermentApi.DTOs
{
    public class ServiceContactUsDto
    {
        public Guid Oid { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? FullNameAr { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? SubjectAr { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MessageAr { get; set; }
        public Guid? ContactTypeLookupId { get; set; }
        public string? ContactTypeName { get; set; }
        public Guid? PriorityLookupId { get; set; }
        public string? PriorityName { get; set; }
        public Guid? StatusLookupId { get; set; }
        public string? StatusName { get; set; }
        public string? Response { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? TicketNumber { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateContactUsDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? FullNameAr { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? SubjectAr { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MessageAr { get; set; }
        public Guid? ContactTypeLookupId { get; set; }
        public Guid? StudentId { get; set; }
    }

    public class RespondContactUsDto
    {
        public Guid Oid { get; set; }
        public string Response { get; set; } = string.Empty;
        public Guid? StatusLookupId { get; set; }
        public Guid RespondedBy { get; set; }
    }
}