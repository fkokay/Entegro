using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;

namespace Entegro.Application.Interfaces.Services
{
    public interface IIntegrationSystemService
    {
        Task<IntegrationSystemDto?> GetByIdAsync(int id);
        Task<List<IntegrationSystemDto>> GetAllAsync();
        Task<PagedResult<IntegrationSystemDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateIntegrationSystemDto integrationSystem);
        Task<bool> UpdateAsync(UpdateIntegrationSystemDto integrationSystem);
        Task<bool> DeleteAsync(int integrationSystemId);
    }
}
