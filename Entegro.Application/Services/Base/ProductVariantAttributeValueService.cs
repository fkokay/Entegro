using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
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

        public async Task DeleteAsync(int id)
        {
            var model = await _productVariantAttributeValueRepository.GetByIdAsync(id);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttributeValue with ID {id} not found.");
            }
            await _productVariantAttributeValueRepository.DeleteAsync(model);
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

        public async Task<ProductVariantAttributeValueDto?> GetByProductVariantAttributeIdAsync(int productVariantAttributeId)
        {
            var productAttributeValue = await _productVariantAttributeValueRepository.GetByProductVariantAttributeIdAsync(productVariantAttributeId);
            if (productAttributeValue == null)
            {
                return null;
            }

            var productAttributeValueDto = _mapper.Map<ProductVariantAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }

        public async Task<PagedResult<ProductVariantAttributeValueDto>> GetPagedAsync(GridCommand gridCommand, int productVariantAttributeId)
        {
            var values = await _productVariantAttributeValueRepository.GetPagedAsync(gridCommand, productVariantAttributeId);

            var items = await values.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ProductVariantAttributeValueDto>(x);
                return model;
            }).AsyncToList();

            return new PagedResult<ProductVariantAttributeValueDto>
            {
                Items = items,
                TotalCount = values.TotalCount,
                PageNumber = values.PageNumber,
                PageSize = values.PageSize
            };
        }

        public async Task<ProductVariantAttributeValueDto> UpdateAsync(UpdateProductVariantAttributeValueDto data)
        {
            await _productVariantAttributeValueRepository.UpdateAsync(_mapper.Map<ProductVariantAttributeValue>(data));
            return _mapper.Map<ProductVariantAttributeValueDto>(data);
        }
    }
}
