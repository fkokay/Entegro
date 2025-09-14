using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.N11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface ICicekSepetiService : IMarketplaceCategoryReader<CicekSepetiApiContext>, IMarketplaceBrandReader<CicekSepetiApiContext>, IMarketplaceCategoryAttributeReader<CicekSepetiApiContext>
    {
        Task<IEnumerable<CicekSepetiProductDto>> GetProductsAsync(CicekSepetiApiContext context, int pageSize = 50);
        Task<CicekSepetiProductDto?> GetProductWithStockCodeAsync(CicekSepetiApiContext context, string stockCode);
        Task UpdatePriceAndStockAsync(CicekSepetiApiContext context, CicekSepetiPriceAndStockUpdateRequest priceAndStockUpdateRequest);
        Task<IEnumerable<CicekSepetiOrderDto>> GetOrdersAsync(CicekSepetiApiContext context);
    }
}
