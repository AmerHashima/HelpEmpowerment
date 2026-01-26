using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StandardArticture.Common;

namespace StandardArticture.Models
{
    [Table("AppLookupD")]
    public class AppLookupDetail : BaseEntity
    {
        [Required]
        public Guid LookupHeaderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LookupValue { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LookupNameEn { get; set; }

        [MaxLength(100)]
        public string? LookupNameAr { get; set; }

        public int? OrderNo { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LookupHeaderId))]
        public virtual AppLookupHeader LookupHeader { get; set; } = null!;
    }
}