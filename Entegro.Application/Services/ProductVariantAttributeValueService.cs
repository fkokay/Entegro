
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ProductVariantAttributeValueService : IProductVariantAttributeValueService
    {
        private readonly IProductVariantAttributeValueRepository _productVariantAttributeValueRepository;
        private readonly IMapper _mapper;

        public ProductVariantAttributeValueService(IProductVariantAttributeValueRepository productVariantAttributeValueRepository, IMapper mapper)
        {
            _productVariantAttributeValueRepository = productVariantAttributeValueRepository ?? throw new ArgumentNullException(nameof(productVariantAttributeValueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductVariantAttributeValueDto> AddAsync(CreateProductVariantAttributeValueDto data)
        {
            var model = _mapper.Map<ProductVariantAttributeValue>(data);
            await _productVariantAttributeValueRepository.AddAsync(model);
            return _mapper.Map<ProductVariantAttributeValueDto>(model);
        }

        public async Task<ProductVariantAttributeValueDto?> GetByNameAsync(string name)
        {
            var productAttributeValue = await _productVariantAttributeValueRepository.GetByNameAsync(name);
            if (productAttributeValue == null)
            {
                return null;
            }

            var productAttributeValueDto = _mapper.Map<ProductVariantAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }
    }
}
