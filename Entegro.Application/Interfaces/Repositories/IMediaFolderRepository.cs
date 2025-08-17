using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IMediaFolderRepository
    {
        Task<MediaFolder?> GetByIdAsync(int id);
        Task<List<MediaFolder>> GetAllAsync();
        Task<PagedResult<MediaFolder>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(MediaFolder mediaFolder);
        Task UpdateAsync(MediaFolder mediaFolder);
        Task DeleteAsync(MediaFolder mediaFolder);
    }
}
