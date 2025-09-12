namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolApiContext
    {
        public string BaseUrl = "https://apigw.trendyol.com/integration/";
        public string SupplierId { get; set; } = default!;
        public string ApiUser { get; set; } = default!;
        public string ApiPassword { get; set; } = default!;
    }

}
