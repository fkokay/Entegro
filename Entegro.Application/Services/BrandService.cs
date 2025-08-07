using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entegro.Application.DTOs.Brand;

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
        public async Task<int> CreateBrandAsync(CreateBrandDto createBrand)
        {
            var brand = _mapper.Map<Brand>(createBrand);
            await _brandRepository.AddAsync(brand);

            return brand.Id;
        }

        public async Task<bool> DeleteBrandAsync(int brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);

            if (brand == null)
            {
                throw new KeyNotFoundException($"Brand with ID {brandId} not found.");
            }
            await _brandRepository.DeleteAsync(brand);
            return true;
        }

        public async Task<BrandDto> GetBrandByIdAsync(int brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null)
            {
                throw new KeyNotFoundException($"Brand with ID {brandId} not found.");
            }

            var brandDto = _mapper.Map<BrandDto>(brand);
            return brandDto;
        }

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            var brandDtos = _mapper.Map<IEnumerable<BrandDto>>(brands);
            return brandDtos;
        }

        public async Task<PagedResult<BrandDto>> GetBrandsAsync(int pageNumber, int pageSize)
        {
            var brands = await _brandRepository.GetAllAsync(pageNumber, pageSize);
            var brandDtos = _mapper.Map<PagedResult<BrandDto>>(brands);
            return brandDtos;
        }

        public async Task<bool> UpdateBrandAsync(UpdateBrandDto updateBrand)
        {
            await _brandRepository.UpdateAsync(_mapper.Map<Brand>(updateBrand));
            return true;
        }
    }
}
