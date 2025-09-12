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
        public CommerceJobs(IN11Service n11Service,IPazaramaService pazaramaService,IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
            _n11Service = n11Service;   
            _pazaramaService = pazaramaService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            PazaramaApiContext pazaramaApiContext = new PazaramaApiContext();
            pazaramaApiContext.ClientId = "f1b613316b0948d29ec81c8e64b1e595";
            pazaramaApiContext.ClientSecret = "d47b5a87351341d9a4abad85b98f7077";

           var products = await _pazaramaService.GetProductsAsync(pazaramaApiContext);
        }
    }
}
