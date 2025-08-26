namespace Entegro.Web.Models
{
    public class LogoErpSettingsViewModel
    {
        //mağaza bilgileri
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }


        //entegro entegrasyon sistemi bilgileri
        public int IntegrationSystemId { get; set; }
        public string ErpType { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }
}
