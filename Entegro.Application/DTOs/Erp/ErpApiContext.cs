namespace Entegro.Application.DTOs.Erp
{
    public class ErpApiContext
    {
        public string BaseUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
        public string ErpType { get; set; }
        public int IntegrationSystemId { get; set; }
    }
}
