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

        public async Task<ProductVariantAttributeCombinationDto> AddAsync(CreateProductVariantAttributeCombinationDto productAttributeMapping)
        {
            var model = _mapper.Map<ProductVariantAttributeCombination>(productAttributeMapping);
            await _productVariantAttributeCombinationRepository.AddAsync(model);

            return _mapper.Map<ProductVariantAttributeCombinationDto>(model);
        }

        public async Task DeleteAsync(int productAttributeMappingId)
        {
            var model = await _productVariantAttributeCombinationRepository.GetByIdAsync(productAttributeMappingId);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {productAttributeMappingId} not found.");
            }
            await _productVariantAttributeCombinationRepository.DeleteAsync(model);
        }

        public async Task<List<ProductVariantAttributeCombinationDto>> GetAllAsync()
        {
            var productVariantAttributeCombination = await _productVariantAttributeCombinationRepository.GetAllAsync();
            var ProductVariantAttributeCombinationDto = _mapper.Map<IEnumerable<ProductVariantAttributeCombinationDto>>(productVariantAttributeCombination);
            return ProductVariantAttributeCombinationDto.ToList();
        }

        public async Task<PagedResult<ProductVariantAttributeCombinationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
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

        public async Task<ProductVariantAttributeCombinationDto> UpdateAsync(UpdateProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            await _productVariantAttributeCombinationRepository.UpdateAsync(_mapper.Map<ProductVariantAttributeCombination>(productVariantAttributeCombination));
            return _mapper.Map<ProductVariantAttributeCombinationDto>(productVariantAttributeCombination);
        }
    }
}
