using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IIntegrationSystemParameterRepository
    {
        Task<IntegrationSystemParameter?> GetByIdAsync(int id);
        Task<List<IntegrationSystemParameter>> GetAllAsync();
        Task<PagedResult<IntegrationSystemParameter>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(IntegrationSystemParameter integrationSystemParameter);
        Task UpdateAsync(IntegrationSystemParameter integrationSystemParameter);
        Task DeleteAsync(IntegrationSystemParameter integrationSystemParameter);
    }
}
