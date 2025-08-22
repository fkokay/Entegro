using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeMapping;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductAttributeMappingService : IProductAttributeMappingService
    {
        private readonly IProductAttributeMappingRepository _productAttributeMappingRepository;
        private readonly IMapper _mapper;

        public ProductAttributeMappingService(IProductAttributeMappingRepository productAttributeMapping, IMapper mapper)
        {
            _productAttributeMappingRepository = productAttributeMapping ?? throw new ArgumentNullException(nameof(productAttributeMapping));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateProductAttributeMappingDto productAttributeMapping)
        {
            var model = _mapper.Map<ProductVariantAttribute>(productAttributeMapping);
            await _productAttributeMappingRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int productAttributeMappingId)
        {
            var model = await _productAttributeMappingRepository.GetByIdAsync(productAttributeMappingId);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {productAttributeMappingId} not found.");
            }
            await _productAttributeMappingRepository.DeleteAsync(model);
            return true;
        }

        public async Task<List<ProductAttributeMappingDto>> GetAllAsync()
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetAllAsync();
            var ProductAttributeMappingDto = _mapper.Map<IEnumerable<ProductAttributeMappingDto>>(productAttributeMapping);
            return ProductAttributeMappingDto.ToList();
        }

        public async Task<PagedResult<ProductAttributeMappingDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productAttributeMappings = await _productAttributeMappingRepository.GetAllAsync(pageNumber, pageSize);
            var productAttributeMappingDto = _mapper.Map<PagedResult<ProductAttributeMappingDto>>(productAttributeMappings);
            return productAttributeMappingDto;
        }

        public async Task<ProductAttributeMappingDto?> GetByAttibuteIdAsync(int id)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetByAttributeIdAsync(id);
            if (productAttributeMapping == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {id} not found.");
            }

            var productAttributeMappingDto = _mapper.Map<ProductAttributeMappingDto>(productAttributeMapping);
            return productAttributeMappingDto;
        }

        public async Task<ProductAttributeMappingDto?> GetByIdAsync(int id)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetByIdAsync(id);
            if (productAttributeMapping == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {id} not found.");
            }

            var productAttributeMappingDto = _mapper.Map<ProductAttributeMappingDto>(productAttributeMapping);
            return productAttributeMappingDto;
        }



        public async Task<bool> UpdateAsync(UpdateProductAttributeMappingDto productAttributeMapping)
        {
            await _productAttributeMappingRepository.UpdateAsync(_mapper.Map<ProductVariantAttribute>(productAttributeMapping));
            return true;
        }
    }
}
