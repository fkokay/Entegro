using Entegro.Application.DTOs.Product;
using Entegro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Application.DTOs.ReturnRequestItem
{
    public class ReturnRequestItemListDto
    {
        public int Id { get; set; }
        public int ReturnRequestId { get; set; }
        public int? ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string MerchantSku { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }
        public decimal Price { get; set; }
        public int VatBaseAmount { get; set; }
        public int VatRate { get; set; }
        public int SalesCampaignId { get; set; }
        public string ProductCategory { get; set; }

        public string? ProductImageUrl { get; set; }
        public string CustomerClaimReasonName { get; set; }
        public string CustomerClaimReasonCode { get; set; }
        public string PlatformClaimReasonName { get; set; }
        public string PlatformClaimReasonCode { get; set; }
        public string PlatformName { get; set; }
        public DateTime? AutoApproveDate { get; set; }
        public string Note { get; set; }
        public string CustomerNote { get; set; }
        public bool Resolved { get; set; }
        public bool? AcceptedBySeller { get; set; }
        public int ReturnRequestStatusId { get; set; }
        [NotMapped]
        public ReturnRequestStatus ReturnRequestStatus
        {
            get => (ReturnRequestStatus)ReturnRequestStatusId;
            set => ReturnRequestStatusId = (int)value;
        }
    }
}
