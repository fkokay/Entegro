using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Microsoft.AspNetCore.Http;

namespace Entegro.Application.Interfaces.Services
{
    public interface IMediaFileService
    {
        Task<MediaFileDto?> GetByIdAsync(int id);
        Task<List<MediaFileDto>> GetAllAsync();
        Task<PagedResult<MediaFileDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<MediaFileDto> AddAsync(CreateMediaFileDto mediaFile);
        Task<MediaFileDto> UpdateAsync(UpdateMediaFileDto mediaFile);
        Task DeleteAsync(int mediaFileId);
        Task<MediaFileDto?> GetByNameAndFolderAsync(string name, int? folderId);
        Task<CreateMediaFileDto> BuildMediaFileDtoAsync(IFormFile file, string fileName, int? folderId);
        Task<bool> OverwriteByNameAsync(string name, int? folderId, CreateMediaFileDto builtDto);
        Task<string> GetUrl(int id);
    }
}
