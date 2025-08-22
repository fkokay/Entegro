using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductImageMappingRepository
    {
        Task<ProductMediaFile?> GetByIdAsync(int id);
        Task<List<ProductMediaFile>> GetAllAsync();
        Task AddAsync(ProductMediaFile productImage);
        Task UpdateAsync(ProductMediaFile productImage);
        Task DeleteAsync(ProductMediaFile productImage);
        Task<PagedResult<ProductMediaFile>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductMediaFile> GetByPictureIdProductIdAsync(int pictureId, int productId);
    }
}
