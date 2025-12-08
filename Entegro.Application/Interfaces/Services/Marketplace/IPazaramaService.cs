using Entegro.Application.DTOs.Marketplace.Pazarama;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IPazaramaService : IMarketplaceCategoryReader<PazaramaApiContext>, IMarketplaceBrandReader<PazaramaApiContext>, IMarketplaceCategoryAttributeReader<PazaramaApiContext>
    {
        Task<PazaramaTokenDto> GetToken(PazaramaApiContext context);
        Task<IEnumerable<PazaramaProductDto>> GetProductsAsync(PazaramaApiContext context, int pageSize = 50);
        Task<PazaramaProductDto?> GetProductWithStockCodeAsync(PazaramaApiContext context, string stockCode);
        Task UpdatePriceAsync(PazaramaApiContext context, PazaramaPriceUpdateRequest pazaramaPriceUpdateRequest);
        Task UpdateStockAsync(PazaramaApiContext context, PazaramaStockUpdateRequest pazaramaStockUpdateRequest);
        Task<IEnumerable<OrderData>> GetOrdersAsync(PazaramaApiContext context);
    }
}
