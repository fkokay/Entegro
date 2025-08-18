using AutoMapper;
using Entegro.Application.DTOs.Town;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class TownService : ITownService
    {
        private readonly ITownRepository _townRepository;
        private readonly IMapper _mapper;

        public TownService(ITownRepository townRepository, IMapper mapper)
        {
            _townRepository = townRepository ?? throw new ArgumentNullException(nameof(townRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<TownDto>> GetAllAsync()
        {
            var towns = await _townRepository.GetAllAsync();
            return _mapper.Map<List<TownDto>>(towns);
        }

        public async Task<List<TownDto>> GetByCityIdAsync(int cityId)
        {
            var towns = await _townRepository.GetByCityIdAsync(cityId);
            return _mapper.Map<List<TownDto>>(towns);
        }

        public async Task<TownDto> GetByIdAsync(int id)
        {
            var town = await _townRepository.GetByIdAsync(id);
            return _mapper.Map<TownDto>(town);
        }

        public async Task AddAsync(CreateTownDto dto)
        {
            var town = _mapper.Map<Town>(dto);
            await _townRepository.AddAsync(town);
        }

        public async Task UpdateAsync(UpdateTownDto dto)
        {
            var town = _mapper.Map<Town>(dto);
            await _townRepository.UpdateAsync(town);
        }

        public async Task DeleteAsync(int id)
        {
            await _townRepository.DeleteAsync(id);
        }
    }
}
