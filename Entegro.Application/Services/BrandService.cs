using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IMapper _mapper;
        public BrandService(IBrandRepository brandRepository, IMapper mapper, IMediaFileRepository mediaFileRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediaFileRepository = mediaFileRepository ?? throw new ArgumentNullException(nameof(mediaFileRepository));
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

        public async Task<BrandDto> GetBrandByNameAsync(string brandName)
        {
            var brand = await _brandRepository.GetByNameAsync(brandName);
            if (brand == null)
            {
                throw new KeyNotFoundException($"Brand with Name {brandName} not found.");
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

        public async Task<bool> ExistsByNameAsync(string brandName)
        {
            return await _brandRepository.ExistsByNameAsync(brandName);
        }

        public async Task UpdateBrandImageAsync(int brandId, int mediaFileId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand != null)
            {
                brand.MediaFileId = mediaFileId;
                await _brandRepository.UpdateAsync(brand);
            }
        }

        public async Task<BrandDto?> GetByIdWithMediaAsync(int id)
        {
            var brand = await _brandRepository.GetByIdWithMediaAsync(id);
            if (brand == null)
            {
                return null;
            }
            var brandDto = _mapper.Map<BrandDto>(brand);
            return brandDto;
        }

        public async Task DeleteBrandImageAsync(int brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand != null)
            {
                brand.MediaFileId = null;
                await _brandRepository.UpdateAsync(brand);
            }
        }
    }
}
