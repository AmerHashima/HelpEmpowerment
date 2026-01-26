using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StandardArticture.Common;

namespace StandardArticture.Models
{
    [Table("AppLookupH")]
    public class AppLookupHeader : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string LookupCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LookupNameAr { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LookupNameEn { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<AppLookupDetail> LookupDetails { get; set; } = new List<AppLookupDetail>();
    }
}