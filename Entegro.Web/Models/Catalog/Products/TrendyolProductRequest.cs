namespace Entegro.Web.Models.Catalog.Products
{

    public class DispatchProductIntegrationRequest
    {
        public int IntegrationSystemId { get; set; }
        public string ProductIntegrationSku { get; set; }

    }
    public class ImportAndMatchProductFromTrendyol: DispatchProductIntegrationRequest{}
    public class ImportAndMatchProductFromPazarama: DispatchProductIntegrationRequest{}
}
