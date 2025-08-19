using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IMediaFolderRepository
    {
        Task<MediaFolder?> GetByIdAsync(int id);
        Task<MediaFolder?> GetMediaFolderByNameAsync(string folderName);
        Task<string?> GetTreePathByIdAsync(int id);
        Task<List<MediaFolder>> GetAllAsync();
        Task<PagedResult<MediaFolder>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(MediaFolder mediaFolder);
        Task UpdateAsync(MediaFolder mediaFolder);
        Task DeleteAsync(MediaFolder mediaFolder);

        Task UpdateFilesCountAsync(int folderId, int filesCount);
    }
}
