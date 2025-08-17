using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class IntegrationSystemService : IIntegrationSystemService
    {
        private readonly IIntegrationSystemRepository _integrationSystemRepository;
        private readonly IMapper _mapper;

        public IntegrationSystemService(IIntegrationSystemRepository integrationSystem, IMapper mapper)
        {
            _integrationSystemRepository = integrationSystem ?? throw new ArgumentNullException(nameof(integrationSystem));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateIntegrationSystemDto integrationSystem)
        {
            var model = _mapper.Map<IntegrationSystem>(integrationSystem);
            await _integrationSystemRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int integrationSystemId)
        {
            var model = await _integrationSystemRepository.GetByIdAsync(integrationSystemId);

            if (model == null)
            {
                throw new KeyNotFoundException($"IntegrationSystem with ID {integrationSystemId} not found.");
            }
            await _integrationSystemRepository.DeleteAsync(model);
            return true;
        }

        public async Task<List<IntegrationSystemDto>> GetAllAsync()
        {
            var integrationSystem = await _integrationSystemRepository.GetAllAsync();
            var IntegrationSystemDto = _mapper.Map<IEnumerable<IntegrationSystemDto>>(integrationSystem);
            return IntegrationSystemDto.ToList();
        }

        public async Task<PagedResult<IntegrationSystemDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var integrationSystems = await _integrationSystemRepository.GetAllAsync(pageNumber, pageSize);
            var integrationSystemDto = _mapper.Map<PagedResult<IntegrationSystemDto>>(integrationSystems);
            return integrationSystemDto;
        }

        public async Task<IntegrationSystemDto?> GetByIdAsync(int id)
        {
            var integrationSystem = await _integrationSystemRepository.GetByIdAsync(id);
            if (integrationSystem == null)
            {
                throw new KeyNotFoundException($"IntegrationSystem with ID {id} not found.");
            }

            var integrationSystemDto = _mapper.Map<IntegrationSystemDto>(integrationSystem);
            return integrationSystemDto;
        }

        public async Task<IntegrationSystemDto?> GetByTypeIdAsync(int typeId)
        {
            var integrationSystem = await _integrationSystemRepository.GetByTypeIdAsync(typeId);
            if (integrationSystem == null)
            {
                return null;
            }

            var integrationSystemDto = _mapper.Map<IntegrationSystemDto>(integrationSystem);
            return integrationSystemDto;
        }

        public async Task<bool> UpdateAsync(UpdateIntegrationSystemDto integrationSystem)
        {
            await _integrationSystemRepository.UpdateAsync(_mapper.Map<IntegrationSystem>(integrationSystem));
            return true;
        }
    }
}
