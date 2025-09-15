using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Import;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IImportProfileRepository
    {
        Task<ImportProfile?> GetByIdAsync(int id);
        Task<List<ImportProfile>> GetAllAsync();
        Task<PagedResult<ImportProfile>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ImportProfile>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(ImportProfile importProfile);
        Task UpdateAsync(ImportProfile importProfile);
        Task DeleteAsync(ImportProfile importProfile);
    }
}
