namespace Entegro.Web.Models.Integration
{
    public class ErpPageViewModel
    {
        public IntegrationSystemViewModel CurrentErp { get; set; }
        public List<IntegrationSystemViewModel> MyErpList { get; set; } = new();
    }
}
