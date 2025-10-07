using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICrossSellProductRepository
    {
        Task<bool> ExistsByIdAsync(int productId1, int productId2);
        Task<CrossSellProduct?> GetByIdAsync(int id);
        Task<CrossSellProduct?> GetByIdAsync(int productId1, int productId2);
        Task<PagedResult<CrossSellProduct>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task AddAsync(CrossSellProduct crossSellProduct);
        Task UpdateAsync(CrossSellProduct crossSellProduct);
        Task DeleteAsync(CrossSellProduct crossSellProduct);
        Task DeleteAllAsync(List<CrossSellProduct> crossSellProduct);
    }
}
