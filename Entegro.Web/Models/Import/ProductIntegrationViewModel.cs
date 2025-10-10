namespace Entegro.Web.Models.Import
{
    public class ProductIntegrationViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsChanged { get; set; }
        public Dictionary<string, IntegrationPriceInfo> IntegrationPrices { get; set; } = new();
    }

    public class IntegrationPriceInfo
    {
        public int IntegrationSystemId { get; set; }
        public decimal? Price { get; set; }
    }

    public class ExcelImportRequest
    {
        public IFormFile File { get; set; }
        public int BrandId { get; set; }
        public List<string> SelectedHeaders { get; set; }
    }

}
