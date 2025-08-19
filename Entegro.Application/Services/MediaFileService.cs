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
        private readonly IMapper _mapper;

        public MediaFileService(IMediaFileRepository mediaFileRepository, IMapper mapper)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateMediaFileDto mediaFile)
        {
            var model = _mapper.Map<MediaFile>(mediaFile);
            await _mediaFileRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<CreateMediaFileDto> BuildMediaFileDtoAsync(IFormFile file, int folderId)
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
            else { }


            return new CreateMediaFileDto
            {
                FolderId = folderId,
                Name = Path.GetFileName(file.FileName),
                Alt = Path.GetFileNameWithoutExtension(file.FileName),
                Title = Path.GetFileName(file.FileName),
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
            await _mediaFileRepository.DeleteAsync(mediaFile);
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

        public async Task<bool> UpdateAsync(UpdateMediaFileDto mediaFile)
        {
            await _mediaFileRepository.UpdateAsync(_mapper.Map<MediaFile>(mediaFile));
            return true;
        }
    }
}
