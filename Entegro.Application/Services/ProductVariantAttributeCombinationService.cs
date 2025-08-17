using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductVariantAttributeCombinationService : IProductVariantAttributeCombinationService
    {
        private readonly IProductVariantAttributeCombinationRepository _productVariantAttributeCombinationRepository;
        private readonly IMapper _mapper;

        public ProductVariantAttributeCombinationService(IProductVariantAttributeCombinationRepository productVariantAttributeCombinationRepository, IMapper mapper)
        {
            _productVariantAttributeCombinationRepository = productVariantAttributeCombinationRepository ?? throw new ArgumentNullException(nameof(productVariantAttributeCombinationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(ProductVariantAttributeCombinationDto productAttributeMapping)
        {
            var model = _mapper.Map<ProductVariantAttributeCombination>(productAttributeMapping);
            await _productVariantAttributeCombinationRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int productAttributeMappingId)
        {
            var model = await _productVariantAttributeCombinationRepository.GetByIdAsync(productAttributeMappingId);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {productAttributeMappingId} not found.");
            }
            await _productVariantAttributeCombinationRepository.DeleteAsync(model);
            return true;
        }

        public async Task<List<ProductVariantAttributeCombinationDto>> GetAllAsync()
        {
            var productVariantAttributeCombination = await _productVariantAttributeCombinationRepository.GetAllAsync();
            var ProductVariantAttributeCombinationDto = _mapper.Map<IEnumerable<ProductVariantAttributeCombinationDto>>(productVariantAttributeCombination);
            return ProductVariantAttributeCombinationDto.ToList();
        }

        public async Task<PagedResult<ProductVariantAttributeCombinationDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productVariantAttributeCombination = await _productVariantAttributeCombinationRepository.GetAllAsync(pageNumber, pageSize);
            var productVariantAttributeCombinationDto = _mapper.Map<PagedResult<ProductVariantAttributeCombinationDto>>(productVariantAttributeCombination);
            return productVariantAttributeCombinationDto;
        }

        public async Task<ProductVariantAttributeCombinationDto?> GetByIdAsync(int id)
        {
            var productVariantAttributeCombination = await _productVariantAttributeCombinationRepository.GetByIdAsync(id);
            if (productVariantAttributeCombination == null)
            {
                throw new KeyNotFoundException($"productVariantAttributeCombination with ID {id} not found.");
            }

            var productVariantAttributeCombinationDto = _mapper.Map<ProductVariantAttributeCombinationDto>(productVariantAttributeCombination);
            return productVariantAttributeCombinationDto;
        }

        public async Task<bool> UpdateAsync(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            await _productVariantAttributeCombinationRepository.UpdateAsync(_mapper.Map<ProductVariantAttributeCombination>(productVariantAttributeCombination));
            return true;
        }
    }
}
