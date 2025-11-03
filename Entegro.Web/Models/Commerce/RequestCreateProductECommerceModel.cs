namespace Entegro.Web.Models.Commerce
{
    public class RequestCreateProductECommerceModel
    {
        public int IntegrationSystemId { get; set; }
        public string CommerceType { get; set; }
        public string Code { get; set; } = null!;
    }
}
