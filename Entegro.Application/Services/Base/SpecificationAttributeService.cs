using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttribute;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class SpecificationAttributeService : ISpecificationAttributeService
    {
        private readonly ISpecificationAttributeRepository _specificationAttributeRepository;
        private readonly IMapper _mapper;
        public SpecificationAttributeService(ISpecificationAttributeRepository specificationAttributeRepository, IMapper mapper)
        {
            _specificationAttributeRepository = specificationAttributeRepository ?? throw new ArgumentNullException(nameof(specificationAttributeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SpecificationAttributeDto> AddAsync(CreateSpecificationAttributeDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var specificationAttribute = _mapper.Map<SpecificationAttribute>(model);
            await _specificationAttributeRepository.AddAsync(specificationAttribute);

            return _mapper.Map<SpecificationAttributeDto>(specificationAttribute);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var specificationAttribute = await _specificationAttributeRepository.GetByIdAsync(id);
            if (specificationAttribute == null)
                throw new KeyNotFoundException($"ID {id} ile SpecificationAttribute bulunamadı.");

            await _specificationAttributeRepository.DeleteAsync(specificationAttribute);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _specificationAttributeRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("SpecificationAttribute name boş olamaz.", nameof(name));

            return await _specificationAttributeRepository.ExistsByNameAsync(name);
        }

        public async Task<IEnumerable<SpecificationAttributeDto>> GetAllAsync()
        {
            var specificationAttribute = await _specificationAttributeRepository.GetAllAsync();
            var specificationAttributeDtos = _mapper.Map<IEnumerable<SpecificationAttributeDto>>(specificationAttribute);
            return specificationAttributeDtos;
        }

        public async Task<SpecificationAttributeDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var specificationAttribute = await _specificationAttributeRepository.GetByIdAsync(id);
            if (specificationAttribute == null)
            {
                return null;
            }
            var specificationAttributeDto = _mapper.Map<SpecificationAttributeDto>(specificationAttribute);

            return specificationAttributeDto;
        }

        public async Task<SpecificationAttributeDto?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("SpecificationAttribute adı boş olamaz.", nameof(name));
            }

            var specificationAttribute = await _specificationAttributeRepository.GetByNameAsync(name);
            if (specificationAttribute == null)
            {
                return null;
            }

            var specificationAttributeDto = _mapper.Map<SpecificationAttributeDto>(specificationAttribute);

            return specificationAttributeDto;
        }

        public async Task<PagedResult<SpecificationAttributeDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var brands = await _specificationAttributeRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<SpecificationAttributeDto>
            {
                Items = _mapper.Map<IEnumerable<SpecificationAttributeDto>>(brands.Items),
                TotalCount = brands.TotalCount,
                PageNumber = brands.PageNumber,
                PageSize = brands.PageSize
            };
        }

        public async Task<PagedResult<SpecificationAttributeDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var attributes = await _specificationAttributeRepository.GetPagedAsync(gridCommand);
            return new PagedResult<SpecificationAttributeDto>
            {
                Items = _mapper.Map<IEnumerable<SpecificationAttributeDto>>(attributes.Items),
                TotalCount = attributes.TotalCount,
                PageNumber = attributes.PageNumber,
                PageSize = attributes.PageSize
            };
        }

        public async Task<SpecificationAttributeDto> UpdateAsync(UpdateSpecificationAttributeDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingSpecificationAttribute = await _specificationAttributeRepository.GetByIdAsync(model.Id);
            if (existingSpecificationAttribute == null)
                throw new KeyNotFoundException($"ID {model.Id} ile SpecificationAttribute bulunamadı.");

            _mapper.Map(model, existingSpecificationAttribute);
            await _specificationAttributeRepository.UpdateAsync(existingSpecificationAttribute);

            return _mapper.Map<SpecificationAttributeDto>(existingSpecificationAttribute);
        }
    }
}
