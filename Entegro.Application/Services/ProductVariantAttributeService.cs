
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ProductVariantAttributeService : IProductVariantAttributeService
    {
        private readonly IProductVariantAttributeRepository _productAttributeMappingRepository;
        private readonly IMapper _mapper;

        public ProductVariantAttributeService(IProductVariantAttributeRepository productAttributeMapping, IMapper mapper)
        {
            _productAttributeMappingRepository = productAttributeMapping ?? throw new ArgumentNullException(nameof(productAttributeMapping));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductVariantAttributeDto> AddAsync(CreateProductVariantAttributeDto productAttributeMapping)
        {
            var model = _mapper.Map<ProductVariantAttribute>(productAttributeMapping);
            await _productAttributeMappingRepository.AddAsync(model);
            return _mapper.Map<ProductVariantAttributeDto>(model);
        }

        public async Task DeleteAsync(int productAttributeMappingId)
        {
            var model = await _productAttributeMappingRepository.GetByIdAsync(productAttributeMappingId);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {productAttributeMappingId} not found.");
            }
            await _productAttributeMappingRepository.DeleteAsync(model);
        }

        public async Task<List<ProductVariantAttributeDto>> GetAllAsync()
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetAllAsync();
            var ProductAttributeMappingDto = _mapper.Map<IEnumerable<ProductVariantAttributeDto>>(productAttributeMapping);
            return ProductAttributeMappingDto.ToList();
        }

        public async Task<PagedResult<ProductVariantAttributeDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            var productAttributeMappings = await _productAttributeMappingRepository.GetAllAsync(pageNumber, pageSize);
            var productAttributeMappingDto = _mapper.Map<PagedResult<ProductVariantAttributeDto>>(productAttributeMappings);
            return productAttributeMappingDto;
        }

        public async Task<ProductVariantAttributeDto?> GetByAttibuteIdAsync(int productId, int attributeId)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetByAttributeIdAsync(productId, attributeId);
            if (productAttributeMapping == null)
            {
                return null;
            }

            var productAttributeMappingDto = _mapper.Map<ProductVariantAttributeDto>(productAttributeMapping);
            return productAttributeMappingDto;
        }

        public async Task<ProductVariantAttributeDto?> GetByIdAsync(int id)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetByIdAsync(id);
            if (productAttributeMapping == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {id} not found.");
            }

            var productAttributeMappingDto = _mapper.Map<ProductVariantAttributeDto>(productAttributeMapping);
            return productAttributeMappingDto;
        }

        public async Task<ProductVariantAttributeDto> UpdateAsync(UpdateProductVariantAttributeDto productAttributeMapping)
        {
            await _productAttributeMappingRepository.UpdateAsync(_mapper.Map<ProductVariantAttribute>(productAttributeMapping));
            return _mapper.Map<ProductVariantAttributeDto>(productAttributeMapping);
        }
    }
}
