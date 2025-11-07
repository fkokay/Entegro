namespace Entegro.Application.DTOs.Product
{
    public class ProductIntegrationMatrixDto
    {
        public int IntegrationId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string IntegrationCode { get; set; } = string.Empty;
        public decimal ListPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? StoreSalePrice { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ProductVariantAttributeCombinationId { get; set; }
    }

}
