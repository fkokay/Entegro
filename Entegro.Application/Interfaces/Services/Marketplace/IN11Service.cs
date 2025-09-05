using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IN11Service : IMarketplaceCategoryReader<N11ApiContext>, IMarketplaceBrandReader<N11ApiContext>, IMarketplaceCategoryAttributeReader<N11ApiContext>
    {
        Task<IEnumerable<N11ProductDto>> GetProductsAsync(N11ApiContext context,int pageSize = 50);
        Task<N11ProductDto?> GetProductWithN11CodeAsync(N11ApiContext context, string n11Code);
        Task<N11ProductDto?> GetProductWithStockCodeAsync(N11ApiContext context, string stockCode);
        Task UpdatePriceAndStockAsync(N11ApiContext context, N11PriceAndStockUpdatePayload payload);
        Task<IEnumerable<N11ShipmentPackageDto>> GetShipmentPackagesAsync(N11ApiContext context, int pageSize = 50);
    }
}
