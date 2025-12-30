using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Events;
using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
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
        public async Task<ProductIntegrationDto> AddAsync(CreateProductIntegrationDto createProductIntegration)
        {
            var productIntegration = _mapper.Map<ProductIntegration>(createProductIntegration);
            await _productIntegrationRepository.AddAsync(productIntegration);


            var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(productIntegration.Id);
            await _eventPublisher.Publish(recordUpdatedEvent);
            return _mapper.Map<ProductIntegrationDto>(productIntegration);
        }

        public async Task DeleteAsync(int productIntegrationId)
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

        public async Task<ProductIntegrationDto> UpdateAsync(UpdateProductIntegrationDto updateProductIntegration, bool isEvent = true)
        {
            var map = _mapper.Map<ProductIntegration>(updateProductIntegration);
            await _productIntegrationRepository.UpdateAsync(map);

            if (isEvent)
            {
                var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(updateProductIntegration.Id);
                await _eventPublisher.Publish(recordUpdatedEvent);
            }

            return _mapper.Map<ProductIntegrationDto>(updateProductIntegration);
        }

        public async Task<IEnumerable<ProductIntegrationDto>> GetProductIntegrationAllWithProductIdAsync(int productId)
        {
            return _mapper.Map<IEnumerable<ProductIntegrationDto>>(await _productIntegrationRepository.GetAllAsync(productId));
        }

        public async Task<ProductIntegrationDto?> GetByProductAndIntegrationSystemAsync(int productId, int integrationSystemId, int productVariantAttributeCombinationId)
        {
            var productIntegration = await _productIntegrationRepository.GetByProductIdandIntegrationSystemIdAsync(productId, integrationSystemId, productVariantAttributeCombinationId);
            if (productIntegration == null)
            {
                return null;
            }

            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }

        public async Task<ProductIntegrationDto?> GetByProductIdandProductIntegrationIdAsync(int productId, int integrationId)
        {
            var productIntegration = await _productIntegrationRepository.GetByProductIdandProductIntegrationIdAsync(productId, integrationId);
            if (productIntegration == null)
            {
                return null;
            }

            var productIntegrationDto = _mapper.Map<ProductIntegrationDto>(productIntegration);
            return productIntegrationDto;
        }
    }
}
