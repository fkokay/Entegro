using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Microsoft.AspNetCore.Http;

namespace Entegro.Application.Interfaces.Services
{
    public interface IMediaFileService
    {
        Task<MediaFileDto?> GetByIdAsync(int id);
        Task<List<MediaFileDto>> GetAllAsync();
        Task<PagedResult<MediaFileDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateMediaFileDto mediaFile);
        Task<bool> UpdateAsync(UpdateMediaFileDto mediaFile);
        Task<bool> DeleteAsync(int mediaFileId);
        Task<CreateMediaFileDto> BuildMediaFileDtoAsync(IFormFile file,string fileName, int? folderId);
        Task<MediaFileDto?> GetByNameAndFolderAsync(string name, int? folderId);
        Task<bool> OverwriteByNameAsync(string name, int? folderId, CreateMediaFileDto builtDto);
    }
}
