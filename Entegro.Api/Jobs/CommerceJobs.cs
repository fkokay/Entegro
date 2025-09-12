using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class CommerceJobs : IJob
    {
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IN11Service _n11Service;
        public CommerceJobs(IN11Service n11Service,IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
            _n11Service = n11Service;   
        }

        public async Task Execute(IJobExecutionContext context)
        {
            N11ApiContext n11ApiContext = new N11ApiContext();
            n11ApiContext.AppKey = "2bdb87dd-4b7b-4942-8119-43769b4f4dee";
            n11ApiContext.AppSecret = "NuKAxv0lroMfnSgP";

           var products = await _n11Service.GetProductsAsync(n11ApiContext);
        }
    }
}
