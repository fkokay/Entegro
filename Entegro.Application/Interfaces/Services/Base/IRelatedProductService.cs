using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.RelatedProduct;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IRelatedProductService
    {
        Task<bool> ExistsByIdAsync(int productId1, int productId2);
        Task<RelatedProductDto?> GetByIdAsync(int id);
        Task<RelatedProductDto?> GetByIdAsync(int productId1, int productId2);
        Task<PagedResult<RelatedProductDto>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task<RelatedProductDto> AddAsync(CreateRelatedProductDto model);
        Task<RelatedProductDto> UpdateAsync(UpdateRelatedProductDto model);
        Task DeleteAsync(int id);
        Task DeleteAllAsync(List<int> idList);
    }
}
