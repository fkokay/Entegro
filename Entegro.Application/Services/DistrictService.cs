using AutoMapper;
using Entegro.Application.DTOs.District;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly IMapper _mapper;
        public DistrictService(IDistrictRepository districtRepository, IMapper mapper)
        {
            _districtRepository = districtRepository ?? throw new ArgumentNullException(nameof(districtRepository)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DistrictDto>> GetAllAsync()
        {
            var districts = await _districtRepository.GetAllAsync();
            return _mapper.Map<List<DistrictDto>>(districts);
        }

        public async Task<List<DistrictDto>> GetByTownIdAsync(int townId)
        {
            var districts = await _districtRepository.GetByTownIdAsync(townId);
            return _mapper.Map<List<DistrictDto>>(districts);
        }

        public async Task<DistrictDto> GetByIdAsync(int id)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            return _mapper.Map<DistrictDto>(district);
        }

        public async Task AddAsync(CreateDistrictDto dto)
        {
            var district = _mapper.Map<District>(dto);
            await _districtRepository.AddAsync(district);
        }

        public async Task UpdateAsync(UpdateDistrictDto dto)
        {
            var district = _mapper.Map<District>(dto);
            await _districtRepository.UpdateAsync(district);
        }

        public async Task DeleteAsync(int id)
        {
            await _districtRepository.DeleteAsync(id);
        }
    }
}
