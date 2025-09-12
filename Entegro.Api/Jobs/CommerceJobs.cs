using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class CommerceJobs : IJob
    {
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IN11Service _n11Service;
        private readonly IPazaramaService _pazaramaService;
        private readonly IHepsiburadaService _hepsiburadaService;
        private readonly ITrendyolService _trendyolService;
        public CommerceJobs(IN11Service n11Service,IPazaramaService pazaramaService,IHepsiburadaService hepsiburadaService,ITrendyolService trendyolService,IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
            _n11Service = n11Service;   
            _pazaramaService = pazaramaService;
            _hepsiburadaService = hepsiburadaService;
            _trendyolService = trendyolService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            TrendyolApiContext trendyolApiContext = new TrendyolApiContext();
            trendyolApiContext.ApiUser = "9tjWr2F7zHJKnMDMbcqb";
            trendyolApiContext.ApiPassword = "09WZjNvN6ZJU4Tg2z53r";
            trendyolApiContext.SupplierId = "474352";

            var products = await _trendyolService.GetProductsAsync(trendyolApiContext);
        }

    

    }
}
