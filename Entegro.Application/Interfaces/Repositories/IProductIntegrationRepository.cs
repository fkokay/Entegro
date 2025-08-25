using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductIntegrationRepository
    {
        Task<ProductIntegration?> GetByIdAsync(int id);
        Task<ProductIntegration?> GetByIntegrationSystemIdandIntegrationCodeAsync(int integrationSystemId, string integrationCode);
        Task<ProductIntegration?> GetByIntegrationCodeAsync(string integrationCode);
        Task<ProductIntegration?> GetByProductIdandIntegrationSystemIdAsync(int productId, int integrationSystemId);
        Task<List<ProductIntegration>> GetAllAsync();
        Task AddAsync(ProductIntegration productIntegration);
        Task UpdateAsync(ProductIntegration productIntegration);
        Task DeleteAsync(ProductIntegration productIntegration);
        Task<PagedResult<ProductIntegration>> GetAllAsync(int pageNumber, int pageSize);
    }
}
