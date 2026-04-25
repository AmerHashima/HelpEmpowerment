    namespace HelpEmpowermentApi.DTOs
{
    public class StudentDto
    {
        public Guid Oid { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? PromoCode { get; set; }
        public float? PromoDiscount { get; set; }
        public int UsersUsedPromo { get; set; }
        public float TotalMoneyWithPromo { get; set; }
        public DateTime? PromoToDateValid { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
   

    public class StudentWithCoursesDto
    {
        public Guid Oid { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Courses { get; set; } = new();
    }

    public class CreateStudentDto
    {
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? PromoCode { get; set; }
        public float? PromoDiscount { get; set; }
        public DateTime? PromoToDateValid { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentDto
    {
        public Guid Oid { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? PromoCode { get; set; }
        public float? PromoDiscount { get; set; }
        public int UsersUsedPromo { get; set; }
        public float TotalMoneyWithPromo { get; set; }
        public DateTime? PromoToDateValid { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}