
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;
using System.Xml.Linq;

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

        public async Task<ProductVariantAttributeValueDto?> GetByIdAsync(int id)
        {
            var productAttributeValue = await _productVariantAttributeValueRepository.GetByIdAsync(id);
            if (productAttributeValue == null)
            {
                return null;
            }

            var productAttributeValueDto = _mapper.Map<ProductVariantAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }

        public async Task<ProductVariantAttributeValueDto?> GetByNameAsync(int productVariantAttributeId, string name)
        {
            var productAttributeValue = await _productVariantAttributeValueRepository.GetByNameAsync(productVariantAttributeId, name);
            if (productAttributeValue == null)
            {
                return null;
            }

            var productAttributeValueDto = _mapper.Map<ProductVariantAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }
    }
}
