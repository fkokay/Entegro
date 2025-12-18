using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.ReturnRequestItem;

namespace Entegro.Application.DTOs.ReturnRequest
{
    public class ReturnRequestListDto
    {
        public int Id { get; set; }
        public int? IntegrationSystemId { get; set; }
        public virtual IntegrationSystemDto? IntegrationSystem { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CargoTrackingNumber { get; set; }
        public string CargoProviderName { get; set; }
        public string CargoTrackingLink { get; set; }
        public long OrderShipmentPackageId { get; set; }
        public long OrderOutboundPackageId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual ICollection<ReturnRequestItemListDto> Items { get; set; } = new List<ReturnRequestItemListDto>();
        public decimal SubTotal { get; set; }
    }
}
