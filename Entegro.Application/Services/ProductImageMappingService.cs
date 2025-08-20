using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductImage;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductImageMappingService : IProductImageMappingService
    {
        private readonly IProductImageMappingRepository _productImageMappingRepository;
        private readonly IMapper _mapper;
        public ProductImageMappingService(IProductImageMappingRepository productImageMappingRepository, IMapper mapper)
        {
            _productImageMappingRepository = productImageMappingRepository ?? throw new ArgumentNullException(nameof(productImageMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddAsync(CreateProductImageDto productImage)
        {
            var createProductImage = _mapper.Map<ProductImageMapping>(productImage);
            await _productImageMappingRepository.AddAsync(createProductImage);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productImage = await _productImageMappingRepository.GetByIdAsync(id);

            if (productImage == null)
            {
                throw new KeyNotFoundException($"productImage with ID {id} not found.");
            }
            await _productImageMappingRepository.DeleteAsync(productImage);
            return true;
        }

        public async Task<List<ProductImageDto>> GetAllAsync()
        {
            var productImage = await _productImageMappingRepository.GetAllAsync();
            var productImages = _mapper.Map<IEnumerable<ProductImageDto>>(productImage);
            return productImages.ToList();
        }

        public async Task<PagedResult<ProductImageDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productImage = await _productImageMappingRepository.GetAllAsync(pageNumber, pageSize);
            var productImageDtos = _mapper.Map<PagedResult<ProductImageDto>>(productImage);
            return productImageDtos;
        }

        public async Task<ProductImageDto?> GetByIdAsync(int id)
        {
            var productImage = await _productImageMappingRepository.GetByIdAsync(id);
            if (productImage == null)
            {
                throw new KeyNotFoundException($"ProductImage with ID {id} not found.");
            }

            var productImageDto = _mapper.Map<ProductImageDto>(productImage);
            return productImageDto;
        }

        public async Task<bool> UpdateAsync(UpdateProductImageDto productImage)
        {
            await _productImageMappingRepository.UpdateAsync(_mapper.Map<ProductImageMapping>(productImage));
            return true;
        }
    }
}
