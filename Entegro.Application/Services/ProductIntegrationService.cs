using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System.ComponentModel.Design;

namespace Entegro.Application.Services
{
    public class ProductIntegrationService : IProductIntegrationService
    {
        private readonly IProductIntegrationRepository _productIntegrationRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        public ProductIntegrationService(
            IProductIntegrationRepository productIntegrationRepository,
            IMapper mapper,
            IEventPublisher eventPublisher)
        {
            _productIntegrationRepository = productIntegrationRepository ?? throw new ArgumentNullException(nameof(productIntegrationRepository)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }
        public async Task<ProductIntegrationDto> CreateProductIntegrationAsync(CreateProductIntegrationDto createProductIntegration)
        {
            var productIntegration = _mapper.Map<ProductIntegration>(createProductIntegration);
            await _productIntegrationRepository.AddAsync(productIntegration);


            var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(productIntegration.Id);
            _eventPublisher.Publish(recordUpdatedEvent);
            return _mapper.Map<ProductIntegrationDto>(productIntegration);
        }

        public async Task DeleteProductIntegrationAsync(int productIntegrationId)
        {
            ProductIntegration? productIntegration = await _productIntegrationRepository.GetByIdAsync(productIntegrationId);

            if (productIntegration == null)
            {
                throw new KeyNotFoundException($"ProductIntegration with ID {productIntegrationId} not found.");
            }
            await _productIntegrationRepository.DeleteAsync(productIntegration);
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

        public async Task<ProductIntegrationDto?> GetByIntegrationSystemAndCodeAsync(int integrationSystemId, string integrationCode)
        {
            var productIntegration = await _productIntegrationRepository.GetByIntegrationSystemIdandIntegrationCodeAsync(integrationSystemId, integrationCode);
            if (productIntegration == null)
            {
                return null;
            }
            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<ProductIntegrationDto?> GetByProductAndIntegrationSystemAsync(int productId, int integrationSystemId)
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

        public async Task<PagedResult<ProductIntegrationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            return _mapper.Map<PagedResult<ProductIntegrationDto>>(await _productIntegrationRepository.GetAllAsync(pageNumber, pageSize));
        }

        public async Task<ProductIntegrationDto> UpdateProductIntegrationAsync(UpdateProductIntegrationDto updateProductIntegration)
        {
            await _productIntegrationRepository.UpdateAsync(_mapper.Map<ProductIntegration>(updateProductIntegration));

            var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(updateProductIntegration.Id);
            _eventPublisher.Publish(recordUpdatedEvent);

            return _mapper.Map<ProductIntegrationDto>(updateProductIntegration);
        }
    }
}
