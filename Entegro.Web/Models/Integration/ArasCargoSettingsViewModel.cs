namespace Entegro.Web.Models
{
    public class ArasCargoSettingsViewModel
    {
        //mağaza bilgileri
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        //entegro entegrasyon sistemi bilgileri
        public int IntegrationSystemId { get; set; }
        public string CommerceType { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

    }
}
