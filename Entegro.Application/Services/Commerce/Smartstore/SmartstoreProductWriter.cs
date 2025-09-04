using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Domain.Entities;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreProductWriter : ICommerceProductWriter, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly SmartstoreClient _smartstoreClient;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductService _productService;
        private readonly ILogger<SmartstoreProductWriter> _logger;
        public SmartstoreProductWriter(
            SmartstoreClient smartstoreClient,
            IProductIntegrationService productIntegrationService,
            IProductService productService,
            ILogger<SmartstoreProductWriter> logger)
        {
            _smartstoreClient = smartstoreClient;
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(ProductIntegrationRecordUpdatedEvent recordUpdatedEvent)
        {
            var productIntegration = await _productIntegrationService.GetByIdAsync(recordUpdatedEvent.ProductIntegrationId);
            if (productIntegration == null)
            {
                return;
            }

            if (productIntegration.IntegrationSystem.IntegrationSystemType == IntegrationSystemType.Commerce)
            {
                string? commerceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "CommerceType").Select(m => m.Value).FirstOrDefault();

                if (commerceType == "Smartstore")
                {
                    object? customData = string.IsNullOrEmpty(productIntegration.Custom) ? null : JsonConvert.DeserializeObject<SmartstoreProductIntegrationCustomDto>(productIntegration.Custom);

                    var product = await _productService.GetProductByIdAsync(productIntegration.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning($"Product with ID {productIntegration.ProductId} not found.");
                        return;
                    }

                    product.Code = productIntegration.IntegrationCode;
                    product.Price = productIntegration.Price;

                    var request = new UpsertProductRequest
                    {
                        Product = product,
                        CustomData = customData
                    };

                    await _smartstoreClient.UpsertProductAsync(request);
                }
            }
        }

        public async Task DeleteProductAsync(string sku)
        {
            await _smartstoreClient.DeleteProductAsync(sku);
        }

        public async Task DeleteProductsAsync(IEnumerable<string> skus)
        {
            await _smartstoreClient.DeleteProductsAsync(skus);
        }

        public async Task UpsertProductAsync(UpsertProductRequest request)
        {
            await _smartstoreClient.UpsertProductAsync(request);
        }

        public async Task UpsertProductsAsync(IEnumerable<UpsertProductRequest> requests)
        {
            await _smartstoreClient.UpsertProductsAsync(requests);
        }
    }
}
