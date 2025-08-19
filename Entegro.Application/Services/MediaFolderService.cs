using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class MediaFolderService : IMediaFolderService
    {
        private readonly IMediaFolderRepository _mediaFolderRepository;
        private readonly IMapper _mapper;

        public MediaFolderService(IMediaFolderRepository mediaFolderRepository, IMapper mapper)
        {
            _mediaFolderRepository = mediaFolderRepository ?? throw new ArgumentNullException(nameof(mediaFolderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateMediaFolderDto mediaFolder)
        {
            var model = _mapper.Map<MediaFolder>(mediaFolder);
            await _mediaFolderRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<MediaFolderDto> CreateFolderAsync(string folderName, int? parentId = null)
        {
            var folder = new MediaFolder
            {
                Name = folderName,
                Slug = folderName.ToLower().Replace(" ", "-"),
                TreePath = "pending",
                CanDetectTracks = false,
                Metadata = null,
                FilesCount = 0,
                Discriminator = "MediaAlbum",
                ResKey = "Catalog",
                IncludePath = true,
                Order = null,
                ParentId = parentId
            };
            int id = await _mediaFolderRepository.AddAsync(folder);
            string treePath;
            if (parentId == null)
            {
                treePath = $"/{folder.Id}/";
            }
            else
            {
                var parentPath = await _mediaFolderRepository.GetTreePathByIdAsync(parentId.Value);
                treePath = $"{parentPath}{folder.Id}/";
            }
            folder.TreePath = treePath;
            await _mediaFolderRepository.UpdateAsync(folder);
            return _mapper.Map<MediaFolderDto>(folder);
        }

        public async Task<bool> DeleteAsync(int mediaFolderId)
        {
            var mediaFolder = await _mediaFolderRepository.GetByIdAsync(mediaFolderId);

            if (mediaFolder == null)
            {
                throw new KeyNotFoundException($"MediaFolder with ID {mediaFolderId} not found.");
            }
            await _mediaFolderRepository.DeleteAsync(mediaFolder);
            return true;
        }

        public async Task<List<MediaFolderDto>> GetAllAsync()
        {
            var mediaFolders = await _mediaFolderRepository.GetAllAsync();
            var mediaFolderDtos = _mapper.Map<IEnumerable<MediaFolderDto>>(mediaFolders);
            return mediaFolderDtos.ToList();
        }

        public async Task<PagedResult<MediaFolderDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var mediaFolders = await _mediaFolderRepository.GetAllAsync(pageNumber, pageSize);
            var mediaFolderDtos = _mapper.Map<PagedResult<MediaFolderDto>>(mediaFolders);
            return mediaFolderDtos;
        }

        public async Task<MediaFolderDto?> GetByIdAsync(int id)
        {
            var mediaFolder = await _mediaFolderRepository.GetByIdAsync(id);
            if (mediaFolder == null)
            {
                throw new KeyNotFoundException($"MediaFolder with ID {id} not found.");
            }

            var mediaFolderDto = _mapper.Map<MediaFolderDto>(mediaFolder);
            return mediaFolderDto;
        }

        public async Task<MediaFolderDto?> GetMediaFolderByNameAsync(string folderName)
        {
            var folder = await _mediaFolderRepository.GetMediaFolderByNameAsync(folderName);
            if (folder == null)
            {
                return null;
            }
            var folderDto = _mapper.Map<MediaFolderDto>(folder);
            return folderDto;
        }

        public async Task<bool> UpdateAsync(UpdateMediaFolderDto mediaFolder)
        {
            await _mediaFolderRepository.UpdateAsync(_mapper.Map<MediaFolder>(mediaFolder));
            return true; ;
        }

        public async Task UpdateFilesCountAsync(int folderId, int filesCount)
        {
            var folder = await _mediaFolderRepository.GetByIdAsync(folderId);

            if (folder == null)
                throw new Exception("Klasör bulunamadı.");

            folder.FilesCount = filesCount;
            await _mediaFolderRepository.UpdateAsync(folder);
        }
    }
}
