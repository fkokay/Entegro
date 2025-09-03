using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IN11Service : IMarketplaceCategoryReader, IMarketplaceBrandReader, IMarketplaceCategoryAttributeReader, IMarketplaceCargoCompanyReader
    {
        Task<IEnumerable<N11ProductDto>> GetProductsAsync(int pageSize = 50);
        Task<N11ProductDto?> GetProductWithN11CodeAsync(string n11Code);
        Task<N11ProductDto?> GetProductWithStockCodeAsync(string stockCode);
        Task UpdatePriceAndStockAsync(N11PriceAndStockUpdatePayload payload);
        Task<IEnumerable<N11ShipmentPackageDto>> GetShipmentPackagesAsync(int pageSize = 50);
    }
}
