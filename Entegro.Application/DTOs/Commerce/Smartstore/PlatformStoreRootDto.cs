namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class PlatformStoreRootDto
    {
        public List<PlatformStoreItemDto> PlatformStore { get; set; } = new();
    }

    public class PlatformStoreItemDto
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public PricingDto pricing { get; set; }
    }

    public class PricingDto
    {
        public decimal profitRate { get; set; }
        public decimal commission { get; set; }
        public decimal cargoPrice { get; set; }
        public decimal extraCost { get; set; }
        public bool applyAutoPrice { get; set; }
    }
}
