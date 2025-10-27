using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductIntegration;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductIntegrationService
    {
        Task<ProductIntegrationDto?> GetByIdAsync(int productIntegrationId);
        Task<ProductIntegrationDto?> GetByIntegrationSystemAndCodeAsync(int integrationSystemId, string integrationCode);
        Task<ProductIntegrationDto?> GetByProductAndIntegrationSystemAsync(int productId, int integrationSystemId);
        Task<ProductIntegrationDto?> GetByProductAndIntegrationSystemAsync(int productId, int integrationSystemId, int productVariantAttributeCombinationId);
        Task<ProductIntegrationDto?> GetByIntegrationCodeAsync(string productIntegrationCode);
        Task<IEnumerable<ProductIntegrationDto>> GetProductIntegrationAsync();
        Task<IEnumerable<ProductIntegrationDto>> GetProductIntegrationAllWithProductIdAsync(int productId);
        Task<PagedResult<ProductIntegrationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductIntegrationDto> AddAsync(CreateProductIntegrationDto createProductIntegration);
        Task<ProductIntegrationDto> UpdateAsync(UpdateProductIntegrationDto updateProductIntegration);
        Task DeleteAsync(int productIntegrationId);
    }
}
