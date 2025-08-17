using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

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
