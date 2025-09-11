using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductImageMappingRepository
    {
        Task<ProductMediaFile?> GetByIdAsync(int id);
        Task<List<ProductMediaFile>> GetAllAsync();
        Task<List<ProductMediaFile>> GetAllAsync(int productId);
        Task AddAsync(ProductMediaFile productImage);
        Task UpdateAsync(ProductMediaFile productImage);
        Task DeleteAsync(ProductMediaFile productImage);
        Task<PagedResult<ProductMediaFile>> GetAllAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductMediaFile?> GetByPictureIdProductIdAsync(int pictureId, int productId);
    }
}
