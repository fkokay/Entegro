using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ShipmentItem")]
    public class ShipmentItem : BaseEntity
    {
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
