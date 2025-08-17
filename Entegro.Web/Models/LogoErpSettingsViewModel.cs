namespace Entegro.Web.Models
{
    public class LogoErpSettingsViewModel
    {
        public int IntegrationSystemId { get; set; }
        public string ErpType { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }
}
