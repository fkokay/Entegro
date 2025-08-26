using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System.Xml.Linq;

namespace Entegro.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _brandRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            return await _brandRepository.ExistsByNameAsync(name);
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var brand = await _brandRepository.GetByIdAsync(id);
            var brandDto = _mapper.Map<BrandDto>(brand);

            return brandDto;
        }

        public async Task<BrandDto?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var brand = await _brandRepository.GetByNameAsync(name);
            var brandDto = _mapper.Map<BrandDto>(brand);

            return brandDto;
        }

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            var brandDtos = _mapper.Map<IEnumerable<BrandDto>>(brands);
            return brandDtos;
        }

        public async Task<PagedResult<BrandDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            var brands = await _brandRepository.GetAllAsync(pageNumber, pageSize);
            var brandDtos = _mapper.Map<PagedResult<BrandDto>>(brands);
            return brandDtos;
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto model)
        {
            var brand = _mapper.Map<Brand>(model);
            await _brandRepository.AddAsync(brand);
            var brandDto = _mapper.Map<BrandDto>(brand);

            return brandDto;
        }

        public async Task<BrandDto> UpdateAsync(UpdateBrandDto model)
        {
            var brand = _mapper.Map<Brand>(model);
            await _brandRepository.UpdateAsync(brand);
            var brandDto = _mapper.Map<BrandDto>(brand);

            return brandDto;
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand != null)
            {
                await _brandRepository.DeleteAsync(brand);
            }
        }
    }
}
