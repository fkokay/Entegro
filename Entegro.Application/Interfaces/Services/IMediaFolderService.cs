using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFolder;

namespace Entegro.Application.Interfaces.Services
{
    public interface IMediaFolderService
    {
        Task<MediaFolderDto?> GetByIdAsync(int id);
        Task<List<MediaFolderDto>> GetAllAsync();
        Task<MediaFolderDto?> GetMediaFolderByNameAsync(string folderName);
        Task<MediaFolderDto> CreateFolderAsync(string folderName, int? parentId = null);
        Task UpdateFilesCountAsync(int folderId, int filesCount);
        Task<PagedResult<MediaFolderDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateMediaFolderDto mediaFolder);
        Task<bool> UpdateAsync(UpdateMediaFolderDto mediaFolder);
        Task<bool> DeleteAsync(int mediaFolderId);
    }
}
