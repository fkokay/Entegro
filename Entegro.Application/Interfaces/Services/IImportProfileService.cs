using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ImportProfile;

namespace Entegro.Application.Interfaces.Services
{
    public interface IImportProfileService
    {
        Task<ImportProfileDto?> GetByIdAsync(int id);
        Task<IEnumerable<ImportProfileDto>> GetAllAsync();
        Task<PagedResult<ImportProfileDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<ImportProfileDto>> GetPagedAsync(GridCommand gridCommand);
        Task<ImportProfileDto> AddAsync(CreateImportProfileDto model);
        Task<ImportProfileDto> UpdateAsync(UpdateImportProfileDto model);
        Task DeleteAsync(int id);
    }
}
