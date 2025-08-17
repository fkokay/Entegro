using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystemParameter;

namespace Entegro.Application.Interfaces.Services
{
    public interface IIntegrationSystemParameterService
    {
        Task<IntegrationSystemParameterDto?> GetByIdAsync(int id);
        Task<List<IntegrationSystemParameterDto>> GetAllAsync();
        Task<PagedResult<IntegrationSystemParameterDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateIntegrationSystemParameterDto integrationSystemParameter);
        Task<bool> UpdateAsync(UpdateIntegrationSystemParameterDto integrationSystemParameter);
        Task<bool> DeleteAsync(int integrationSystemParameterId);
    }
}
