using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductMediaFile;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductImageMappingService
    {
        Task<ProductMediaFileDto?> GetByIdAsync(int id);
        Task<List<ProductMediaFileDto>> GetAllAsync();
        Task<int> AddAsync(CreateProductMediaFileDto productImage);
        Task<bool> UpdateAsync(UpdateProductMediaFileeDto productImage);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<ProductMediaFileDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductMediaFileDto> GetByPictureIdProductIdAsync(int pictureId, int productId);
    }
}
