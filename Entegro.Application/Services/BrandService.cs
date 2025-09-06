using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using MapsterMapper;

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
                throw new ArgumentException("Brand name boş olamaz.", nameof(name));

            return await _brandRepository.ExistsByNameAsync(name);
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return null;
            }
            var brandDto = _mapper.Map<BrandDto>(brand);

            return brandDto;
        }

        public async Task<BrandDto?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Brand adı boş olamaz.", nameof(name));
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
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var brands = await _brandRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<BrandDto>
            {
                Items = _mapper.Map<IEnumerable<BrandDto>>(brands.Items),
                TotalCount = brands.TotalCount,
                PageNumber = brands.PageNumber,
                PageSize = brands.PageSize
            };
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var brand = _mapper.Map<Brand>(model);
            await _brandRepository.AddAsync(brand);

            return _mapper.Map<BrandDto>(brand);
        }

        public async Task<BrandDto> UpdateAsync(UpdateBrandDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingBrand = await _brandRepository.GetByIdAsync(model.Id);
            if (existingBrand == null)
                throw new KeyNotFoundException($"ID {model.Id} ile Brand bulunamadı.");

            _mapper.Map(model, existingBrand);
            await _brandRepository.UpdateAsync(existingBrand);

            return _mapper.Map<BrandDto>(existingBrand);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                throw new KeyNotFoundException($"ID {id} ile Brand bulunamadı.");

            await _brandRepository.DeleteAsync(brand);
        }

        public async Task<PagedResult<BrandDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var brands = await _brandRepository.GetPagedAsync(gridCommand);
            return new PagedResult<BrandDto>
            {
                Items = _mapper.Map<IEnumerable<BrandDto>>(brands.Items),
                TotalCount = brands.TotalCount,
                PageNumber = brands.PageNumber,
                PageSize = brands.PageSize
            };
        }
    }
}
