using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Integration;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IIntegrationSystemRepository
    {
        Task<IntegrationSystem?> GetByIdAsync(int id);
        Task<IntegrationSystem?> GetByTypeIdAsync(int typeId);
        Task<List<IntegrationSystem>> GetAllAsync(int? integrationSystemTypeId);
        Task<PagedResult<IntegrationSystem>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(IntegrationSystem IntegrationSystem);
        Task UpdateAsync(IntegrationSystem IntegrationSystem);
        Task DeleteAsync(IntegrationSystem IntegrationSystem);
    }
}
