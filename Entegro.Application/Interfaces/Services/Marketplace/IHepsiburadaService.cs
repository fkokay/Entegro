using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IHepsiburadaService : IMarketplaceCategoryReader<HepsiburadaApiContext>, IMarketplaceBrandReader<HepsiburadaApiContext>, IMarketplaceCategoryAttributeReader<HepsiburadaApiContext>
    {
        Task<IEnumerable<HepsiburadaProductDto>> GetProductsAsync(HepsiburadaApiContext context, int pageSize = 50);
        Task<HepsiburadaProductDto?> GetProductWitHbSkuAsync(HepsiburadaApiContext context, string hbSku);
        Task<HepsiburadaProductDto?> GetProductWithMerchantSkuAsync(HepsiburadaApiContext context, string merchantSku);
        Task<IEnumerable<HepsiburadaShipmentPackageDto>> GetShipmentPackagesAsync(HepsiburadaApiContext context, int pageSize = 50);
        Task UpdatePriceAsync(HepsiburadaApiContext context, List<HepsiburadaPriceUpdateDto> hepsiburadaPriceUpdates);
        Task UpdateStockAsync(HepsiburadaApiContext context, List<HepsiburadaStockUpdateDto> hepsiburadaStockUpdates);

    }
}
