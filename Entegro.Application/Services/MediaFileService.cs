using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Entegro.Application.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IMediaFolderRepository _mediaFolderRepository;
        private readonly IMapper _mapper;

        public MediaFileService(IMediaFileRepository mediaFileRepository, IMapper mapper, IMediaFolderRepository mediaFolderRepository)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediaFolderRepository = mediaFolderRepository ?? throw new ArgumentNullException(nameof(mediaFolderRepository));
        }

        public async Task<int> AddAsync(CreateMediaFileDto mediaFile)
        {
            var model = _mapper.Map<MediaFile>(mediaFile);
            await _mediaFileRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<CreateMediaFileDto> BuildMediaFileDtoAsync(IFormFile file,string fileName, int? folderId)
        {
            var extension = Path.GetExtension(file.FileName) ?? "";
            var mimeType = file.ContentType ?? "application/octet-stream";
            int width = 0;
            int height = 0;
            int pixelSize = 0;
            string mediaType = "image";

            if (mimeType.StartsWith("image"))
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());
                width = image.Width;
                height = image.Height;
                pixelSize = width * height;
                mediaType = "image";
            }


            return new CreateMediaFileDto
            {
                FolderId = folderId,
                Name = fileName,
                Alt = "",
                Title = "",
                Extension = extension,
                MimeType = mimeType,
                MediaType = mediaType,
                Size = (int)file.Length,
                PixelSize = pixelSize,
                Width = width,
                Height = height,
                Metadata = null,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsTransient = false,
                Deleted = false
            };
        }

        public async Task<bool> DeleteAsync(int mediaFileId)
        {
            var mediaFile = await _mediaFileRepository.GetByIdAsync(mediaFileId);
            if (mediaFile == null)
            {
                throw new KeyNotFoundException($"MediaFile with ID {mediaFileId} not found.");
            }

            var mediaFolder = mediaFile.FolderId.HasValue ? await _mediaFolderRepository.GetByIdAsync(mediaFile.FolderId.Value) : null;


            string folderName = mediaFile.Folder?.Name ?? ""; // default Brand olabilir
            string uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            string filePath = Path.Combine(uploadsRoot, mediaFile.Name);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await _mediaFileRepository.DeleteAsync(mediaFile);

            if (mediaFolder != null)
            {
                mediaFolder.FilesCount--;
                await _mediaFolderRepository.UpdateAsync(mediaFolder);
            }
            return true;
        }

        public async Task<List<MediaFileDto>> GetAllAsync()
        {
            var mediaFiles = await _mediaFileRepository.GetAllAsync();
            var mediaFileDtos = _mapper.Map<IEnumerable<MediaFileDto>>(mediaFiles);
            return mediaFileDtos.ToList();
        }

        public async Task<PagedResult<MediaFileDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var mediaFiles = await _mediaFileRepository.GetAllAsync(pageNumber, pageSize);
            var mediaFileDtos = _mapper.Map<PagedResult<MediaFileDto>>(mediaFiles);
            return mediaFileDtos;
        }

        public async Task<MediaFileDto?> GetByIdAsync(int id)
        {
            var mediaFile = await _mediaFileRepository.GetByIdAsync(id);
            if (mediaFile == null)
            {
                throw new KeyNotFoundException($"MediaFile with ID {id} not found.");
            }

            var mediaFileDto = _mapper.Map<MediaFileDto>(mediaFile);
            return mediaFileDto;
        }

        public async Task<MediaFileDto?> GetByNameAndFolderAsync(string name, int? folderId)
        {
            var entity = await _mediaFileRepository.GetByNameAndFolderAsync(name, folderId);
            return entity is null ? null : _mapper.Map<MediaFileDto>(entity);
        }

        public async Task<bool> OverwriteByNameAsync(string name, int? folderId, CreateMediaFileDto builtDto)
        {
            var entity = await _mediaFileRepository.GetByNameAndFolderAsync(name, folderId);
            if (entity is null) return false;


            entity.Size = builtDto.Size;
            entity.MimeType = builtDto.MimeType;
            entity.MediaType = builtDto.MediaType;
            entity.PixelSize = builtDto.PixelSize;
            entity.Width = builtDto.Width;
            entity.Height = builtDto.Height;
            entity.Extension = builtDto.Extension;
            entity.Metadata = builtDto.Metadata;
            entity.UpdatedOn = DateTime.UtcNow;
            entity.Version = (entity.Version <= 0 ? 1 : entity.Version + 1);

            await _mediaFileRepository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(UpdateMediaFileDto mediaFile)
        {
            await _mediaFileRepository.UpdateAsync(_mapper.Map<MediaFile>(mediaFile));
            return true;
        }
    }
}
