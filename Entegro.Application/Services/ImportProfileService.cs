using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Import;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ImportProfileService : IImportProfileService
    {
        private readonly IImportProfileRepository _importProfileRepository;
        private readonly IMapper _mapper;
        public ImportProfileService(IImportProfileRepository importProfileRepository, IMapper mapper)
        {
            _importProfileRepository = importProfileRepository ?? throw new ArgumentNullException(nameof(importProfileRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ImportProfileDto> CreateAsync(CreateImportProfileDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var importProfile = _mapper.Map<ImportProfile>(model);
            await _importProfileRepository.AddAsync(importProfile);

            return _mapper.Map<ImportProfileDto>(importProfile);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var importProfile = await _importProfileRepository.GetByIdAsync(id);
            if (importProfile == null)
                throw new KeyNotFoundException($"ID {id} ile profil bulunamadı.");

            await _importProfileRepository.DeleteAsync(importProfile);
        }

        public async Task<IEnumerable<ImportProfileDto>> GetAllAsync()
        {
            var importProfiles = await _importProfileRepository.GetAllAsync();
            var ImportProfileDtos = _mapper.Map<IEnumerable<ImportProfileDto>>(importProfiles);
            return ImportProfileDtos;
        }

        public async Task<ImportProfileDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var importProfile = await _importProfileRepository.GetByIdAsync(id);
            if (importProfile == null)
            {
                return null;
            }
            var importProfileDtoDto = _mapper.Map<ImportProfileDto>(importProfile);

            return importProfileDtoDto;
        }

        public async Task<PagedResult<ImportProfileDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var brands = await _importProfileRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<ImportProfileDto>
            {
                Items = _mapper.Map<IEnumerable<ImportProfileDto>>(brands.Items),
                TotalCount = brands.TotalCount,
                PageNumber = brands.PageNumber,
                PageSize = brands.PageSize
            };
        }

        public async Task<PagedResult<ImportProfileDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var importProfile = await _importProfileRepository.GetPagedAsync(gridCommand);
            return new PagedResult<ImportProfileDto>
            {
                Items = _mapper.Map<IEnumerable<ImportProfileDto>>(importProfile.Items),
                TotalCount = importProfile.TotalCount,
                PageNumber = importProfile.PageNumber,
                PageSize = importProfile.PageSize
            };
        }

        public async Task<ImportProfileDto> UpdateAsync(UpdateImportProfileDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingProfile = await _importProfileRepository.GetByIdAsync(model.Id);
            if (existingProfile == null)
                throw new KeyNotFoundException($"ID {model.Id} ile Profile bulunamadı.");

            _mapper.Map(model, existingProfile);
            await _importProfileRepository.UpdateAsync(existingProfile);

            return _mapper.Map<ImportProfileDto>(existingProfile);
        }
    }
}
