using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IMediaFileRepository
    {
        Task<MediaFile?> GetByIdAsync(int id);
        Task<List<MediaFile>> GetAllAsync();
        Task<PagedResult<MediaFile>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(MediaFile mediaFile);
        Task UpdateAsync(MediaFile mediaFile);
        Task DeleteAsync(MediaFile mediaFile);
    }
}
