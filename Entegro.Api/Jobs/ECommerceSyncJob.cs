using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ECommerceSyncJob : IJob
    {
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ECommerceSyncJob> _logger;
        public ECommerceSyncJob(IProductIntegrationService productIntegrationService, IProductService productService, INotificationService notificationService, ILogger<ECommerceSyncJob> logger, IIntegrationSystemService integrationSystemService)
        {
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _notificationService = notificationService;
            _logger = logger;
            _integrationSystemService = integrationSystemService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            int? parameter = null;
            if (dataMap.ContainsKey("Parameter"))
                parameter = (int)dataMap["Parameter"];


            if (parameter.HasValue)
                await TransferProductsToStore(parameter.Value);
        }


        public async Task TransferProductsToStore(int integrationSystemId)
        {
            try
            {
                var allProduct = await _productService.GetProductsAsync();
                var integrationSystem = await _integrationSystemService.GetByIdAsync(integrationSystemId);
                foreach (var product in allProduct)
                {
                    var productIntegration =
                        await _productIntegrationService
                            .GetByProductAndIntegrationSystemAsync(product.Id, integrationSystemId);
                    if (productIntegration == null)
                    {
                        await _productIntegrationService.AddAsync(new CreateProductIntegrationDto
                        {
                            IntegrationCode = product.Code,
                            Price = product.Price,
                            ProductId = product.Id,
                            IntegrationSystemId = integrationSystemId,
                            Active = true,
                            LastSyncDate = null
                        });
                    }
                }

                await _notificationService.SendNotification(
                    NotificationType.Info,
                    "Bildirim",
                    $"{integrationSystem.Name} Adlı Mağazaya Ürün Aktarımları Tamamlandı."
                );
            }
            catch (Exception ex)
            {
                _logger.Error($"Hata:{ex.Message}");
            }
        }

    }
}
