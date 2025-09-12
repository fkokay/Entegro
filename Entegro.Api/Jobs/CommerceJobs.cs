using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class CommerceJobs : IJob
    {
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IN11Service _n11Service;
        private readonly IPazaramaService _pazaramaService;
        private readonly IHepsiburadaService _hepsiburadaService;
        public CommerceJobs(IN11Service n11Service,IPazaramaService pazaramaService,IHepsiburadaService hepsiburadaService,IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
            _n11Service = n11Service;   
            _pazaramaService = pazaramaService;
            _hepsiburadaService = hepsiburadaService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            HepsiburadaApiContext hepsiburadaApiContext = new HepsiburadaApiContext();
            hepsiburadaApiContext.MerchantId = "885a0ad1-8935-4521-b6b4-b251333881fc";
            hepsiburadaApiContext.ApiUser = "885a0ad1-8935-4521-b6b4-b251333881fc";
            hepsiburadaApiContext.ApiPassword = "UksXKFhbtDK3";
            hepsiburadaApiContext.UserAgent = "mevamagaza_dev";

           var products = await _hepsiburadaService.GetProductsAsync(hepsiburadaApiContext);
        }
    }
}
