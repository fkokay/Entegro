using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class CommerceJob : IJob
    {
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IN11Service _n11Service;
        private readonly IPazaramaService _pazaramaService;
        private readonly IHepsiburadaService _hepsiburadaService;
        private readonly ITrendyolService _trendyolService;
        private readonly ICicekSepetiService _cicekSepetiService;
        public CommerceJob(IN11Service n11Service,IPazaramaService pazaramaService,IHepsiburadaService hepsiburadaService,ITrendyolService trendyolService,ICicekSepetiService cicekSepetiService,IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
            _n11Service = n11Service;   
            _pazaramaService = pazaramaService;
            _hepsiburadaService = hepsiburadaService;
            _trendyolService = trendyolService;
            _cicekSepetiService = cicekSepetiService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            CicekSepetiApiContext cicekSepetiApiContext = new CicekSepetiApiContext();
            cicekSepetiApiContext.ApiUser = "0gCXnE95x4SNj6wLPhB6piCM5ApqQ1Lk3JYgiEWj";
            cicekSepetiApiContext.SupplierId = "1500056600";

            var products = await _cicekSepetiService.GetProductsAsync(cicekSepetiApiContext);
        }

    

    }
}
