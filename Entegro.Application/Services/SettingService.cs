using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Setting;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Setttings;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;
        public SettingService(ISettingRepository settingRepository, IMapper mapper)
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SettingDto> CreateAsync(CreateSettingDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var setting = _mapper.Map<Setting>(model);
            await _settingRepository.AddAsync(setting);

            return _mapper.Map<SettingDto>(setting);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
                throw new KeyNotFoundException($"ID {id} ile Setting bulunamadı.");

            await _settingRepository.DeleteAsync(setting);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _settingRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key değeri boş olamaz", nameof(key));

            return await _settingRepository.ExistsByKeyAsync(key);
        }

        public async Task<bool> ExistsByValueAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value değeri boş olamaz", nameof(value));

            return await _settingRepository.ExistsByValueAsync(value);
        }

        public async Task<IEnumerable<SettingDto>> GetAllAsync()
        {
            var settings = await _settingRepository.GetAllAsync();
            var settingDtos = _mapper.Map<IEnumerable<SettingDto>>(settings);
            return settingDtos;
        }

        public async Task<SettingDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
            {
                return null;
            }
            var settingDto = _mapper.Map<SettingDto>(setting);

            return settingDto;
        }

        public async Task<SettingDto?> GetByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key Değeri Boş Olamaz.", nameof(key));
            }

            var setting = await _settingRepository.GetByKeyAsync(key);
            var settingDto = _mapper.Map<SettingDto>(setting);

            return settingDto;
        }

        public async Task<SettingDto?> GetByValueAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Key Değeri Boş Olamaz.", nameof(value));
            }

            var setting = await _settingRepository.GetByValueAsync(value);
            var settingDto = _mapper.Map<SettingDto>(setting);

            return settingDto;
        }

        public async Task<PagedResult<SettingDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var settings = await _settingRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<SettingDto>
            {
                Items = _mapper.Map<IEnumerable<SettingDto>>(settings.Items),
                TotalCount = settings.TotalCount,
                PageNumber = settings.PageNumber,
                PageSize = settings.PageSize
            };
        }

        public async Task<PagedResult<SettingDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var settings = await _settingRepository.GetPagedAsync(gridCommand);
            return new PagedResult<SettingDto>
            {
                Items = _mapper.Map<IEnumerable<SettingDto>>(settings.Items),
                TotalCount = settings.TotalCount,
                PageNumber = settings.PageNumber,
                PageSize = settings.PageSize
            };
        }

        public async Task<SettingDto> UpdateAsync(UpdateSettingDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingSetting = await _settingRepository.GetByIdAsync(model.Id);
            if (existingSetting == null)
                throw new KeyNotFoundException($"ID {model.Id} ile Setting bulunamadı.");

            _mapper.Map(model, existingSetting);
            await _settingRepository.UpdateAsync(existingSetting);

            return _mapper.Map<SettingDto>(existingSetting);
        }
    }
}
