using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<CountryDto>> GetAllAsync()
        {
            var cities = await _countryRepository.GetAllAsync();
            return _mapper.Map<List<CountryDto>>(cities);
        }

        public async Task<CountryDto> GetByIdAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            return _mapper.Map<CountryDto>(country);
        }

        public async Task AddAsync(CreateCountryDto dto)
        {
            var country = _mapper.Map<Country>(dto);
            await _countryRepository.AddAsync(country);
        }

        public async Task UpdateAsync(UpdateCountryDto dto)
        {
            var country = _mapper.Map<Country>(dto);
            await _countryRepository.UpdateAsync(country);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);

            if (country == null)
            {
                throw new KeyNotFoundException($"Country with ID {id} not found.");
            }
            await _countryRepository.DeleteAsync(country.Id);
            return true;
        }

        public async Task<PagedResult<CountryDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var countries = await _countryRepository.GetAllAsync(pageNumber, pageSize);
            var countriesDto = _mapper.Map<PagedResult<CountryDto>>(countries);
            return countriesDto;
        }
    }
}
