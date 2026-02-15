namespace HelpEmpowermentApi.DTOs
{
    public class StudentBasketDto
    {
        public Guid Oid { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseImage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? CouponCode { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class AddToBasketDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public string? CouponCode { get; set; }
    }

    public class UpdateBasketDto
    {
        public Guid Oid { get; set; }
        public int Quantity { get; set; }
        public string? CouponCode { get; set; }
    }

    public class BasketSummaryDto
    {
        public List<StudentBasketDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }
}