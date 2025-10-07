using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IRelatedProductRepository
    {

        Task<bool> ExistsByIdAsync(int productId1, int productId2);
        Task<RelatedProduct?> GetByIdAsync(int id);
        Task<RelatedProduct?> GetByIdAsync(int productId1, int productId2);
        Task<PagedResult<RelatedProduct>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task AddAsync(RelatedProduct relatedProduct);
        Task UpdateAsync(RelatedProduct relatedProduct);
        Task DeleteAsync(RelatedProduct relatedProduct);
        Task DeleteAllAsync(List<RelatedProduct> relatedProduct);
    }
}
