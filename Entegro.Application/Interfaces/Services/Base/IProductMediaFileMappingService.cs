using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductMediaFile;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductMediaFileMappingService
    {
        Task<ProductMediaFileDto?> GetByIdAsync(int id);
        Task<List<ProductMediaFileDto>> GetAllAsync();
        Task<List<ProductMediaFileDto>> GetAllAsync(int productId);
        Task<ProductMediaFileDto> AddAsync(CreateProductMediaFileDto productImage);
        Task<ProductMediaFileDto> UpdateAsync(UpdateProductMediaFileDto productImage);
        Task DeleteAsync(int id);
        Task<PagedResult<ProductMediaFileDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ProductMediaFileDto> GetByPictureIdProductIdAsync(int pictureId, int productId);
        Task<ProductMediaFileDto> GetByPictureIdSortAsync(int pictureId, int productId);
    }
}
