using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StandardArticture.Common
{


    public abstract class BaseEntity
    {
        [Key]
        public Guid Oid { get; set; } = Guid.NewGuid();

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
       

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

    }
}
