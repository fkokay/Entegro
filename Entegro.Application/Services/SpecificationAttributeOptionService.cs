
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttributeOption;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class SpecificationAttributeOptionService : ISpecificationAttributeOptionService
    {
        private readonly ISpecificationAttributeOptionRepository _specificationAttributeOptionRepository;
        private readonly IMapper _mapper;
        public SpecificationAttributeOptionService(ISpecificationAttributeOptionRepository specificationAttributeOptionRepository, IMapper mapper)
        {
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository ?? throw new ArgumentNullException(nameof(specificationAttributeOptionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SpecificationAttributeOptionDto> CreateAsync(CreateSpecificationAttributeOptionDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var specificationAttribute = _mapper.Map<SpecificationAttributeOption>(model);
            await _specificationAttributeOptionRepository.AddAsync(specificationAttribute);

            return _mapper.Map<SpecificationAttributeOptionDto>(specificationAttribute);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var value = await _specificationAttributeOptionRepository.GetByIdAsync(id);

            if (value is null)
            {
                throw new KeyNotFoundException($"specificationAttributeOption with ID {value} not found.");
            }
            await _specificationAttributeOptionRepository.DeleteAsync(value);
            return true;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _specificationAttributeOptionRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("SpecificationAttributeOption name boş olamaz.", nameof(name));

            return await _specificationAttributeOptionRepository.ExistsByNameAsync(name);
        }

        public async Task<IEnumerable<SpecificationAttributeOptionDto>> GetAllAsync()
        {
            var specificationAttribute = await _specificationAttributeOptionRepository.GetAllAsync();
            var specificationAttributeDtos = _mapper.Map<IEnumerable<SpecificationAttributeOptionDto>>(specificationAttribute);
            return specificationAttributeDtos;
        }

        public async Task<SpecificationAttributeOptionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var specificationAttribute = await _specificationAttributeOptionRepository.GetByIdAsync(id);
            if (specificationAttribute == null)
            {
                return null;
            }
            var specificationAttributeDto = _mapper.Map<SpecificationAttributeOptionDto>(specificationAttribute);

            return specificationAttributeDto;
        }

        public async Task<SpecificationAttributeOptionDto?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("SpecificationAttributeOption adı boş olamaz.", nameof(name));
            }

            var specificationAttribute = await _specificationAttributeOptionRepository.GetByNameAsync(name);
            if (specificationAttribute == null)
            {
                return null;
            }

            var specificationAttributeDto = _mapper.Map<SpecificationAttributeOptionDto>(specificationAttribute);

            return specificationAttributeDto;
        }

        public async Task<PagedResult<SpecificationAttributeOptionDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var brands = await _specificationAttributeOptionRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<SpecificationAttributeOptionDto>
            {
                Items = _mapper.Map<IEnumerable<SpecificationAttributeOptionDto>>(brands.Items),
                TotalCount = brands.TotalCount,
                PageNumber = brands.PageNumber,
                PageSize = brands.PageSize
            };
        }

        public async Task<SpecificationAttributeOptionDto> UpdateAsync(UpdateSpecificationAttributeOptionDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingSpecificationAttributeOption = await _specificationAttributeOptionRepository.GetByIdAsync(model.Id);
            if (existingSpecificationAttributeOption == null)
                throw new KeyNotFoundException($"ID {model.Id} ile SpecificationAttributeOption bulunamadı.");

            _mapper.Map(model, existingSpecificationAttributeOption);
            await _specificationAttributeOptionRepository.UpdateAsync(existingSpecificationAttributeOption);

            return _mapper.Map<SpecificationAttributeOptionDto>(existingSpecificationAttributeOption);
        }
    }
}
