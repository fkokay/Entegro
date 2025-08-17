using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IIntegrationSystemLogRepository
    {
        Task<IntegrationSystemLog?> GetByIdAsync(int id);
        Task<List<IntegrationSystemLog>> GetAllAsync();
        Task<PagedResult<IntegrationSystemLog>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(IntegrationSystemLog integrationSystemLog);
        Task UpdateAsync(IntegrationSystemLog integrationSystemLog);
        Task DeleteAsync(IntegrationSystemLog integrationSystemLog);
    }
}
