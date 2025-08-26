using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductIntegrationService : IProductIntegrationService
    {
        private readonly IProductIntegrationRepository _productIntegrationRepository;
        private readonly IMapper _mapper;
        public ProductIntegrationService(IProductIntegrationRepository productIntegrationRepository, IMapper mapper)
        {
            _productIntegrationRepository = productIntegrationRepository ?? throw new ArgumentNullException(nameof(productIntegrationRepository)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<int> CreateProductIntegrationAsync(CreateProductIntegrationDto createProductIntegration)
        {
            var productIntegration = _mapper.Map<ProductIntegration>(createProductIntegration);
            await _productIntegrationRepository.AddAsync(productIntegration);
            return productIntegration.Id;
        }

        public async Task<bool> DeleteProductIntegrationAsync(int productIntegrationId)
        {
            ProductIntegration? productIntegration = await _productIntegrationRepository.GetByIdAsync(productIntegrationId);

            if (productIntegration == null)
            {
                throw new KeyNotFoundException($"ProductIntegration with ID {productIntegrationId} not found.");
            }
            await _productIntegrationRepository.DeleteAsync(productIntegration);
            return true;
        }

        public async Task<ProductIntegrationDto?> GetByIdAsync(int productIntegrationId)
        {
            var productIntegration = await _productIntegrationRepository.GetByIdAsync(productIntegrationId);
            if (productIntegration == null)
            {
                return null;
            }

            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<ProductIntegrationDto?> GetByIntegrationCodeAsync(string productIntegrationCode)
        {
            var productIntegration = await _productIntegrationRepository.GetByIntegrationCodeAsync(productIntegrationCode);
            if (productIntegration == null)
            {
                return null;
            }

            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<ProductIntegrationDto?> GetByIntegrationSystemIdandIntegrationCodeAsync(int integrationSystemId, string integrationCode)
        {
            var productIntegration = await _productIntegrationRepository.GetByIntegrationSystemIdandIntegrationCodeAsync(integrationSystemId, integrationCode);
            if (productIntegration == null)
            {
                return null;
            }
            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<ProductIntegrationDto?> GetByProductIdandIntegrationSystemIdAsync(int productId, int integrationSystemId)
        {
            var productIntegration = await _productIntegrationRepository.GetByProductIdandIntegrationSystemIdAsync(productId, integrationSystemId);
            if (productIntegration == null)
            {
                return null;
            }

            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<IEnumerable<ProductIntegrationDto>> GetProductIntegrationAsync()
        {
            return _mapper.Map<IEnumerable<ProductIntegrationDto>>(await _productIntegrationRepository.GetAllAsync());
        }

        public async Task<PagedResult<ProductIntegrationDto>> GetProductIntegrationAsync(int pageNumber, int pageSize)
        {
            return _mapper.Map<PagedResult<ProductIntegrationDto>>(await _productIntegrationRepository.GetAllAsync(pageNumber, pageSize));
        }

        public async Task<bool> UpdateProductIntegrationAsync(UpdateProductIntegrationDto updateProductIntegration)
        {
            await _productIntegrationRepository.UpdateAsync(_mapper.Map<ProductIntegration>(updateProductIntegration));
            return true;
        }
    }
}
