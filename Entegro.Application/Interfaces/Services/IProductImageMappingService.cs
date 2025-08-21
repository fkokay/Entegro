using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductImage;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductImageMappingService
    {
        Task<ProductImageDto?> GetByIdAsync(int id);
        Task<List<ProductImageDto>> GetAllAsync();
        Task<int> AddAsync(CreateProductImageDto productImage);
        Task<bool> UpdateAsync(UpdateProductImageDto productImage);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<ProductImageDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductImageDto> GetByPictureIdProductIdAsync(int pictureId, int productId);
    }
}
