using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Checkout
{
    [Table("Shipment")]
    public class Shipment : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? ShippedDateUtc { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new HashSet<ShipmentItem>();
    }
}
