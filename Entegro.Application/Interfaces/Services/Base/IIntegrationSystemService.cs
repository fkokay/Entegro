using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IIntegrationSystemService
    {
        Task<IntegrationSystemDto?> GetByIdAsync(int id);
        Task<IntegrationSystemDto?> GetByTypeIdAsync(int typeId);
        Task<List<IntegrationSystemDto>> GetAllAsync(int? integrationSystemTypeId);
        Task<PagedResult<IntegrationSystemDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateIntegrationSystemDto integrationSystem);
        Task<bool> UpdateAsync(UpdateIntegrationSystemDto integrationSystem);
        Task<bool> DeleteAsync(int integrationSystemId);
    }
}
