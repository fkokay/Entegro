using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("OrderNote")]
    public class OrderNote : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
