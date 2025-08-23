using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductIntegration;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductIntegrationService
    {
        Task<ProductIntegrationDto> GetByIdAsync(int productIntegrationId);
        Task<IEnumerable<ProductIntegrationDto>> GetProductIntegrationAsync();
        Task<PagedResult<ProductIntegrationDto>> GetProductIntegrationAsync(int pageNumber, int pageSize);
        Task<ProductIntegrationDto?> GetByProductIdandIntegrationSystemIdAsync(int productId, int integrationSystemId);
        Task<int> CreateProductIntegrationAsync(CreateProductIntegrationDto createProductIntegration);
        Task<bool> UpdateProductIntegrationAsync(UpdateProductIntegrationDto updateProductIntegration);
        Task<bool> DeleteProductIntegrationAsync(int productIntegrationId);
    }
}
