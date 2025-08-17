using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IMapper _mapper;

        public ProductAttributeService(IProductAttributeRepository productAttributeRepository, IMapper mapper)
        {
            _productAttributeRepository = productAttributeRepository ?? throw new ArgumentNullException(nameof(productAttributeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateProductAttributeDto productAttribute)
        {
            var model = _mapper.Map<ProductAttribute>(productAttribute);
            await _productAttributeRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int productAttributeId)
        {
            var productAttribute = await _productAttributeRepository.GetByIdAsync(productAttributeId);

            if (productAttribute == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {productAttributeId} not found.");
            }
            await _productAttributeRepository.DeleteAsync(productAttribute);
            return true;
        }

        public async Task<List<ProductAttributeDto>> GetAllAsync()
        {
            var productAttributes = await _productAttributeRepository.GetAllAsync();
            var productAttributeDtos = _mapper.Map<IEnumerable<ProductAttributeDto>>(productAttributes);
            return productAttributeDtos.ToList();
        }

        public async Task<PagedResult<ProductAttributeDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productAttributes = await _productAttributeRepository.GetAllAsync(pageNumber, pageSize);
            var productAttributeDtos = _mapper.Map<PagedResult<ProductAttributeDto>>(productAttributes);
            return productAttributeDtos;
        }

        public async Task<ProductAttributeDto?> GetByIdAsync(int id)
        {
            var productAttribute = await _productAttributeRepository.GetByIdAsync(id);
            if (productAttribute == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {id} not found.");
            }

            var productAttributeDto = _mapper.Map<ProductAttributeDto>(productAttribute);
            return productAttributeDto;
        }

        public async Task<bool> UpdateAsync(UpdateProductAttributeDto productAttribute)
        {
            await _productAttributeRepository.UpdateAsync(_mapper.Map<ProductAttribute>(productAttribute));
            return true;
        }
    }
}
