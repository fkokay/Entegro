using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductMediaFile;
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
        public async Task<int> AddAsync(CreateProductMediaFileDto productImage)
        {
            var createProductImage = _mapper.Map<ProductMediaFile>(productImage);
            await _productImageMappingRepository.AddAsync(createProductImage);
            return createProductImage.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productImage = await _productImageMappingRepository.GetByIdAsync(id);

            if (productImage == null)
            {
                throw new KeyNotFoundException($"ProductImage with ID {id} not found.");
            }
            await _productImageMappingRepository.DeleteAsync(productImage);
            return true;
        }

        public async Task<List<ProductMediaFileDto>> GetAllAsync()
        {
            var productImages = await _productImageMappingRepository.GetAllAsync();
            var productImageDtos = _mapper.Map<IEnumerable<ProductMediaFileDto>>(productImages);
            return productImageDtos.ToList();
        }

        public async Task<PagedResult<ProductMediaFileDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var productImages = await _productImageMappingRepository.GetAllAsync(pageNumber, pageSize);
            var productImageMappingDtos = _mapper.Map<PagedResult<ProductMediaFileDto>>(productImages);
            return productImageMappingDtos;
        }

        public async Task<ProductMediaFileDto?> GetByIdAsync(int id)
        {
            var productImage = await _productImageMappingRepository.GetByIdAsync(id);
            if (productImage == null)
            {
                return null;
            }
            var productImageDto = _mapper.Map<ProductMediaFileDto>(productImage);
            return productImageDto;
        }

        public async Task<ProductMediaFileDto> GetByPictureIdProductIdAsync(int pictureId, int productId)
        {
            var productImages = await _productImageMappingRepository.GetByPictureIdProductIdAsync(pictureId, productId);
            var productImageMappingDtos = _mapper.Map<ProductMediaFileDto>(productImages);
            return productImageMappingDtos;
        }

        public async Task<bool> UpdateAsync(UpdateProductMediaFileeDto productImage)
        {
            await _productImageMappingRepository.UpdateAsync(_mapper.Map<ProductMediaFile>(productImage));
            return true;
        }
    }
}
