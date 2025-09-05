using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface ITrendyolService : IMarketplaceCategoryReader<TrendyolApiContext>, IMarketplaceBrandReader<TrendyolApiContext>, IMarketplaceCategoryAttributeReader<TrendyolApiContext>
    {
        Task<IEnumerable<TrendyolCargoCompanyDto>> GetCargoCompaniesAsync();
        Task<IEnumerable<TrendyolProductDto>> GetProductsAsync(TrendyolApiContext context, int pageSize = 50);
        Task<TrendyolProductDto?> GetProductWithBarcodeAsync(TrendyolApiContext context, string barcode);
        Task<IEnumerable<TrendyolShipmentPackageDto>> GetShipmentPackagesAsync(TrendyolApiContext context, int pageSize = 50);
        Task UpdatePriceAndStockAsync(TrendyolApiContext context, TrendyolPriceAndStockUpdateRequest request);
    }
}
