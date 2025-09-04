using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductMediaFile;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductImageMappingService
    {
        Task<ProductMediaFileDto?> GetByIdAsync(int id);
        Task<List<ProductMediaFileDto>> GetAllAsync();
        Task<ProductMediaFileDto> AddAsync(CreateProductMediaFileDto productImage);
        Task<ProductMediaFileDto> UpdateAsync(UpdateProductMediaFileDto productImage);
        Task DeleteAsync(int id);
        Task<PagedResult<ProductMediaFileDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ProductMediaFileDto> GetByPictureIdProductIdAsync(int pictureId, int productId);
    }
}
