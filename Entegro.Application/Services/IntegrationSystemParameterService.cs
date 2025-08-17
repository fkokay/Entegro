using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class IntegrationSystemParameterService : IIntegrationSystemParameterService
    {
        private readonly IIntegrationSystemParameterRepository _integrationSystemParameterRepository;
        private readonly IMapper _mapper;

        public IntegrationSystemParameterService(IIntegrationSystemParameterRepository integrationSystemParameter, IMapper mapper)
        {
            _integrationSystemParameterRepository = integrationSystemParameter ?? throw new ArgumentNullException(nameof(integrationSystemParameter));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateIntegrationSystemParameterDto integrationSystemParameter)
        {
            var model = _mapper.Map<IntegrationSystemParameter>(integrationSystemParameter);
            await _integrationSystemParameterRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int integrationSystemParameterId)
        {
            var model = await _integrationSystemParameterRepository.GetByIdAsync(integrationSystemParameterId);

            if (model == null)
            {
                throw new KeyNotFoundException($"IntegrationSystemParameter with ID {integrationSystemParameterId} not found.");
            }
            await _integrationSystemParameterRepository.DeleteAsync(model);
            return true;
        }

        public async Task<List<IntegrationSystemParameterDto>> GetAllAsync()
        {
            var integrationSystemParameter = await _integrationSystemParameterRepository.GetAllAsync();
            var IntegrationSystemParameterDto = _mapper.Map<IEnumerable<IntegrationSystemParameterDto>>(integrationSystemParameter);
            return IntegrationSystemParameterDto.ToList();
        }

        public async Task<PagedResult<IntegrationSystemParameterDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var integrationSystemParameters = await _integrationSystemParameterRepository.GetAllAsync(pageNumber, pageSize);
            var integrationSystemParameterDto = _mapper.Map<PagedResult<IntegrationSystemParameterDto>>(integrationSystemParameters);
            return integrationSystemParameterDto;
        }

        public async Task<IntegrationSystemParameterDto?> GetByIdAsync(int id)
        {
            var integrationSystemParameter = await _integrationSystemParameterRepository.GetByIdAsync(id);
            if (integrationSystemParameter == null)
            {
                throw new KeyNotFoundException($"IntegrationSystemParameter with ID {id} not found.");
            }

            var integrationSystemParameterDto = _mapper.Map<IntegrationSystemParameterDto>(integrationSystemParameter);
            return integrationSystemParameterDto;
        }

        public async Task<IntegrationSystemParameterDto?> GetByKeyAsync(string key)
        {
            var integrationSystemParameter = await _integrationSystemParameterRepository.GetByKeyAsync(key);
            if (integrationSystemParameter == null)
            {
                return null;
            }

            var integrationSystemParameterDto = _mapper.Map<IntegrationSystemParameterDto>(integrationSystemParameter);
            return integrationSystemParameterDto;
        }

        public async Task<bool> UpdateAsync(UpdateIntegrationSystemParameterDto integrationSystemParameter)
        {
            await _integrationSystemParameterRepository.UpdateAsync(_mapper.Map<IntegrationSystemParameter>(integrationSystemParameter));
            return true;
        }
    }
}
