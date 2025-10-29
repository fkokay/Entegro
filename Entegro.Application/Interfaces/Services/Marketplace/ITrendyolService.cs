using Entegro.Application.DTOs.Marketplace.Trendyol;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface ITrendyolService : IMarketplaceCategoryReader<TrendyolApiContext>, IMarketplaceBrandReader<TrendyolApiContext>, IMarketplaceCategoryAttributeReader<TrendyolApiContext>
    {
        Task<IEnumerable<TrendyolCargoCompanyDto>> GetCargoCompaniesAsync();
        Task<IEnumerable<TrendyolProductDto>> GetProductsAsync(TrendyolApiContext context, int pageSize = 50);
        Task<TrendyolProductDto?> GetProductWithBarcodeAsync(TrendyolApiContext context, string barcode);
        Task<IEnumerable<TrendyolShipmentPackageDto>> GetShipmentPackagesAsync(TrendyolApiContext context, int pageSize = 50);
        Task UpdatePriceAndStockAsync(TrendyolApiContext context, TrendyolPriceAndStockUpdateRequest request);

        Task<List<TrendyolCategoryAttributeDto>?> GetCategorySlicerAttributesAsync(TrendyolApiContext context, int categoryId);
        Task<TrendyolVariantDto?> GetProductVariantAsync(TrendyolApiContext context, string barcode);
        Task<IEnumerable<TrendyolProductDto>> GetProductsByProductMainIdAsync(TrendyolApiContext context, string productMainId);
    }
}
