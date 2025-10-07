using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.CrossSellProduct;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface ICrossSellProductService
    {
        Task<bool> ExistsByIdAsync(int productId1, int productId2);
        Task<CrossSellProductDto?> GetByIdAsync(int id);
        Task<CrossSellProductDto?> GetByIdAsync(int productId1, int productId2);
        Task<PagedResult<CrossSellProductDto>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task<CrossSellProductDto> AddAsync(CreateCrossSellProductDto model);
        Task<CrossSellProductDto> UpdateAsync(UpdateCrossSellProductDto model);
        Task DeleteAsync(int id);
        Task DeleteAllAsync(List<int> idList);
    }
}

