using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Marketplace
{
    public class N11Service : IN11Service, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IProductIntegrationService _productIntegrationService;
        public N11Service(IProductIntegrationService productIntegrationService)
        {
            _productIntegrationService = productIntegrationService;
        }

        public async Task HandleAsync(ProductIntegrationRecordUpdatedEvent recordUpdatedEvent)
        {
            var productIntegration = await _productIntegrationService.GetByIdAsync(recordUpdatedEvent.ProductIntegrationId);
            if (productIntegration == null)
            {
                return;
            }

            if (productIntegration.IntegrationSystem.IntegrationSystemType == IntegrationSystemType.Marketplace)
            {
                string marketplaceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();

                if (marketplaceType == "N11")
                {

                }
            }
        }


        public Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryAttributeDto> GetCategoryAttibutesAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePriceAndStockAsync(N11PriceAndStockUpdatePayload payload)
        {
            throw new NotImplementedException();
        }

        public Task<N11ProductDto?> GetProductWithN11CodeAsync(string n11Code)
        {
            throw new NotImplementedException();
        }

        public Task<N11ProductDto?> GetProductWithStockCodeAsync(string stockCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<N11ProductDto>> GetProductsAsync(int pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<N11ShipmentPackageDto>> GetShipmentPackagesAsync(int pageSize = 50)
        {
            throw new NotImplementedException();
        }
    }
}
