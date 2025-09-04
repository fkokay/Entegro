using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IPazaramaService : IMarketplaceCategoryReader, IMarketplaceBrandReader, IMarketplaceCategoryAttributeReader, IMarketplaceCargoCompanyReader
    {
        Task<PazaramaProductDto?> GetProductWithStockCodeAsync(string stockCode);
    }
}
