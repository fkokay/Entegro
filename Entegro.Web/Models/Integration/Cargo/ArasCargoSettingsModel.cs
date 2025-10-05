namespace Entegro.Web.Models.Integration.Cargo
{
    public class ArasCargoSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string CargoType  { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

    }
}
