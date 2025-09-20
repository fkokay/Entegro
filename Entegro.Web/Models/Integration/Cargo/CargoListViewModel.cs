using Entegro.Web.Models.Integration.Common;

namespace Entegro.Web.Models.Integration.Cargo
{
    public class CargoListModel
    {
        public List<CargoIntegrationSystemModel> CargoList { get; set; } = new();
    }
}
