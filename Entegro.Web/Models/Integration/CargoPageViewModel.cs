namespace Entegro.Web.Models.Integration
{
    public class CargoPageViewModel
    {
        public IntegrationSystemViewModel CurrentCargo { get; set; }
        public List<IntegrationSystemViewModel> MyCargoList { get; set; } = new();
    }
}
