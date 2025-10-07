using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductSpecificationAttribute;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class ProductSpecificationAttributeMappingService : IProductSpecificationAttributeMappingService
    {
        private readonly IProductSpecificationAttributeMappingRepository _productSpecificationAttributeMappingRepository;
        private readonly IMapper _mapper;
        public ProductSpecificationAttributeMappingService(
            IProductSpecificationAttributeMappingRepository productSpecificationAttributeMappingRepository,
            IMapper mapper)
        {
            _productSpecificationAttributeMappingRepository = productSpecificationAttributeMappingRepository ?? throw new ArgumentNullException(nameof(productSpecificationAttributeMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductSpecificationAttributeDto> AddAsync(CreateProductSpecificationAttributeDto productSpecificationAttribute)
        {
            var model = _mapper.Map<ProductSpecificationAttribute>(productSpecificationAttribute);
            await _productSpecificationAttributeMappingRepository.AddAsync(model);
            return _mapper.Map<ProductSpecificationAttributeDto>(model);
        }

        public async Task DeleteAsync(int id)
        {
            var productSpecificationAttribute = await _productSpecificationAttributeMappingRepository.GetByIdAsync(id);

            if (productSpecificationAttribute == null)
            {
                throw new KeyNotFoundException($"productSpecificationAttribute with ID {id} not found.");
            }
            await _productSpecificationAttributeMappingRepository.DeleteAsync(productSpecificationAttribute);
        }

        public async Task<bool> ExistsByIdAsync(int specificationAttributeOptionId, int productId)
        {
            if (specificationAttributeOptionId <= 0 && productId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(specificationAttributeOptionId), nameof(productId));
            }

            return await _productSpecificationAttributeMappingRepository.ExistsByIdAsync(specificationAttributeOptionId, productId);
        }

        public async Task<List<ProductSpecificationAttributeDto>> GetAllAsync()
        {
            var productSpecificationAttributes = await _productSpecificationAttributeMappingRepository.GetAllAsync();
            var productSpecificationAttributeDtos = _mapper.Map<IEnumerable<ProductSpecificationAttributeDto>>(productSpecificationAttributes);
            return productSpecificationAttributeDtos.ToList();
        }

        public async Task<ProductSpecificationAttributeDto?> GetByIdAsync(int id)
        {
            var productSpecificationAttribute = await _productSpecificationAttributeMappingRepository.GetByIdAsync(id);
            if (productSpecificationAttribute == null)
            {
                throw new KeyNotFoundException($"ProductSpecificationAttribute with ID {id} not found.");
            }

            var productSpecificationAttributeDto = _mapper.Map<ProductSpecificationAttributeDto>(productSpecificationAttribute);
            return productSpecificationAttributeDto;
        }

        public async Task<PagedResult<ProductSpecificationAttributeDto>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var productSpecificationAttribute = await _productSpecificationAttributeMappingRepository.GetPagedAsync(gridCommand, productId);

            var items = await productSpecificationAttribute.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ProductSpecificationAttributeDto>(x);
                return model;
            }).AsyncToList();

            return new PagedResult<ProductSpecificationAttributeDto>
            {
                Items = items,
                TotalCount = productSpecificationAttribute.TotalCount,
                PageNumber = productSpecificationAttribute.PageNumber,
                PageSize = productSpecificationAttribute.PageSize
            };
        }

        public async Task<List<ProductSpecificationAttributeDto>> GetSpecificationAttributeByProductId(int productId)
        {
            var productSpecificationAttributes = await _productSpecificationAttributeMappingRepository.GetSpecificationAttributeByProductId(productId);
            var productSpecificationAttributeDtos = _mapper.Map<IEnumerable<ProductSpecificationAttributeDto>>(productSpecificationAttributes);
            return productSpecificationAttributeDtos.ToList();
        }

        public async Task<ProductSpecificationAttributeDto> UpdateAsync(UpdateProductSpecificationAttributeDto productSpecificationAttribute)
        {
            await _productSpecificationAttributeMappingRepository.UpdateAsync(_mapper.Map<ProductSpecificationAttribute>(productSpecificationAttribute));
            return _mapper.Map<ProductSpecificationAttributeDto>(productSpecificationAttribute);
        }
    }
}
