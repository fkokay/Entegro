using AutoMapper;
using Entegro.Application.DTOs.City;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityService(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<CityDto>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllAsync();
            return _mapper.Map<List<CityDto>>(cities);
        }

        public async Task<CityDto> GetByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            return _mapper.Map<CityDto>(city);
        }

        public async Task AddAsync(CreateCityDto dto)
        {
            var city = _mapper.Map<City>(dto);
            await _cityRepository.AddAsync(city);
        }

        public async Task UpdateAsync(UpdateCityDto dto)
        {
            var city = _mapper.Map<City>(dto);
            await _cityRepository.UpdateAsync(city);
        }

        public async Task DeleteAsync(int id)
        {
            await _cityRepository.DeleteAsync(id);
        }
    }
}
