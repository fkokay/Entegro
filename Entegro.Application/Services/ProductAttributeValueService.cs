using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly IProductAttributeValueRepository _productAttributeValueRepository;
        private readonly IMapper _mapper;

        public ProductAttributeValueService(IProductAttributeValueRepository productAttributeValueRepository, IMapper mapper)
        {
            _productAttributeValueRepository = productAttributeValueRepository ?? throw new ArgumentNullException(nameof(productAttributeValueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateProductAttributeValueDto productAttribute)
        {
            var model = _mapper.Map<ProductAttributeValue>(productAttribute);
            await _productAttributeValueRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int productAttributeValueId)
        {
            var productAttribute = await _productAttributeValueRepository.GetByIdAsync(productAttributeValueId);

            if (productAttribute == null)
            {
                throw new KeyNotFoundException($"ProductAttributeValue with ID {productAttributeValueId} not found.");
            }
            await _productAttributeValueRepository.DeleteAsync(productAttribute);
            return true;
        }

        public async Task<List<ProductAttributeValueDto>> GetAllAsync()
        {
            var productAttributeValues = await _productAttributeValueRepository.GetAllAsync();
            var productAttributeValueDtos = _mapper.Map<IEnumerable<ProductAttributeValueDto>>(productAttributeValues);
            return productAttributeValueDtos.ToList();
        }

        public async Task<PagedResult<ProductAttributeValueDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productAttributeValues = await _productAttributeValueRepository.GetAllAsync(pageNumber, pageSize);
            var productAttributeValueDtos = _mapper.Map<PagedResult<ProductAttributeValueDto>>(productAttributeValues);
            return productAttributeValueDtos;
        }

        public async Task<ProductAttributeValueDto?> GetByIdAsync(int id)
        {
            var productAttributeValue = await _productAttributeValueRepository.GetByIdAsync(id);
            if (productAttributeValue == null)
            {
                throw new KeyNotFoundException($"ProductAttributeValue with ID {id} not found.");
            }

            var productAttributeValueDto = _mapper.Map<ProductAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }

        public async Task<bool> UpdateAsync(UpdateProductAttributeValueDto productAttributeValue)
        {
            await _productAttributeValueRepository.UpdateAsync(_mapper.Map<ProductAttributeValue>(productAttributeValue));
            return true;
        }
    }
}
