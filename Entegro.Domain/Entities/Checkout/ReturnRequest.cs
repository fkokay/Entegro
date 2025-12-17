using Entegro.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Checkout
{

    public class ReturnRequestMap : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {

        }
    }

    [Table("ReturnRequest")]
    public class ReturnRequest : BaseEntity, IAuditable
    {
        public int? IntegrationSystemId { get; set; }
        public virtual IntegrationSystem? IntegrationSystem { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string? CargoTrackingNumber { get; set; }
        public string? CargoProviderName { get; set; }
        public string? CargoTrackingLink { get; set; }
        public long OrderShipmentPackageId { get; set; }
        public long OrderOutboundPackageId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual ICollection<ReturnRequestItem> Items { get; set; } = new HashSet<ReturnRequestItem>();
    }
}
