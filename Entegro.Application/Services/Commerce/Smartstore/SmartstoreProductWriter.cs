using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Notifications;
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
                    SmartstoreApiContext context = GetSmartstoreApiContext(productIntegration.IntegrationSystem);

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

                    await _smartstoreClient.UpsertProductAsync(context,request);

                    await EntegroNotification.SendNotification(NotificationType.Info, "Bildirim", $"Smartstore {product.Name} stok ve fiyat güncellendi");
                }
            }
        }

        public async Task DeleteProductAsync(SmartstoreApiContext context,string sku)
        {
            await _smartstoreClient.DeleteProductAsync(context, sku);
        }

        public async Task DeleteProductsAsync(SmartstoreApiContext context, IEnumerable<string> skus)
        {
            await _smartstoreClient.DeleteProductsAsync(context, skus);
        }

        public async Task UpsertProductAsync(SmartstoreApiContext context, UpsertProductRequest request)
        {
            await _smartstoreClient.UpsertProductAsync(context, request);
        }

        public async Task UpsertProductsAsync(SmartstoreApiContext context, IEnumerable<UpsertProductRequest> requests)
        {
            await _smartstoreClient.UpsertProductsAsync(context, requests);
        }

        private SmartstoreApiContext GetSmartstoreApiContext(IntegrationSystemDto item)
        {
            SmartstoreApiContext context = new SmartstoreApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiUrl Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiUser Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Smartstore ApiPassword Ayarlanmamış");
            }

            context.BaseUrl = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiPassword = item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }
    }
}
