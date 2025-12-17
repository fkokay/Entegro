using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.ReturnRequest;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using MapsterMapper;
using Quartz;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ReturnSyncJob : IJob
    {
        private readonly ILogger<ReturnSyncJob> _logger;
        private readonly ITrendyolService _trendyolService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IMapper _mapper;
        public ReturnSyncJob(ILogger<ReturnSyncJob> logger, ITrendyolService trendyolService, IIntegrationSystemService integrationSystemService, IReturnRequestService returnRequestService, IMapper mapper, IProductIntegrationService productIntegrationService)
        {
            _logger = logger;
            _trendyolService = trendyolService;
            _integrationSystemService = integrationSystemService;
            _returnRequestService = returnRequestService;
            _mapper = mapper;
            _productIntegrationService = productIntegrationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("İade aktarım servisi başladı.");

            var integrationSystems = await _integrationSystemService.GetAllAsync(null, true);
            foreach (var item in integrationSystems)
            {
                switch (item.IntegrationSystemType)
                {
                    case Domain.Enums.IntegrationSystemType.Commerce:
                        //await CommerceOrderSync(item);
                        break;
                    case Domain.Enums.IntegrationSystemType.Marketplace:
                        await MarketplaceReturnSync(item);
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task MarketplaceReturnSync(IntegrationSystemDto item)
        {
            string marketPlaceType = item.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault() ?? "";
            switch (marketPlaceType)
            {

                case "Trendyol":
                    await TrendyolReturnSync(item);
                    break;
                default:
                    _logger.LogError("{0} pazaryerine ait iade çekme işlemi bulunamadı", marketPlaceType);
                    break;
            }
        }


        private async Task TrendyolReturnSync(IntegrationSystemDto item)
        {
            try
            {
                _logger.LogInformation("Trendyol iade senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);
                TrendyolApiContext context = GetTrendyolApiContext(item);

                var trendyolReturnRequests = await _trendyolService.GetReturnsAsync(context);

                if (trendyolReturnRequests == null || !trendyolReturnRequests.Any())
                {
                    _logger.Warn("Trendyol'dan hiç iade alınamadı.");
                    return;
                }
                TrendyolReturnMapper.ConfigureLogger(_logger);
                var requestList = TrendyolReturnMapper.ToDtoList(trendyolReturnRequests);

                foreach (var request in requestList)
                {
                    try
                    {
                        request.IntegrationSystemId = item.Id;

                        #region Exists Request
                        if (await _returnRequestService.ExistsByOrderNumberAsync(request.OrderNumber))
                        {
                            var existingRequest = await _returnRequestService.GetByOrderNumberAsync(request.OrderNumber);

                            if (existingRequest == null)
                                continue;

                            await _returnRequestService.UpdateAsync(_mapper.Map<UpdateReturnRequestDto>(existingRequest));
                            _logger.LogInformation("'{OrderNumber}' nolu iade güncellendi", request.OrderNumber);
                            continue;
                        }
                        #endregion

                        #region RequestItem
                        foreach (var requestItem in request.Items)
                        {
                            if (requestItem.Product != null)
                            {
                                var productIntegration = await _productIntegrationService.GetByIntegrationCodeAsync(requestItem.Product.Code);
                                var product = await _trendyolService.GetProductWithBarcodeAsync(context, requestItem.Product.Code);
                                if (productIntegration != null)
                                {
                                    requestItem.Product = null;
                                    requestItem.ProductId = productIntegration.ProductId;
                                    //requestItem.IntegrationProductImageUrl = product.images.FirstOrDefault().url;
                                }
                                else
                                {
                                    requestItem.Product = null;
                                    requestItem.ProductId = null;
                                    //requestItem.IntegrationProductImageUrl = product.images.FirstOrDefault().url;
                                }
                            }
                        }
                        #endregion

                        #region Request
                        var createRequest = _mapper.Map<CreateReturnRequestDto>(request);
                        await _returnRequestService.AddAsync(createRequest);

                        _logger.LogInformation("'{OrderNo}' nolu iade başarıyla kaydedildi.", request.OrderNumber);
                        #endregion


                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
                _logger.LogInformation("Trendyol iade senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private TrendyolApiContext GetTrendyolApiContext(IntegrationSystemDto item)
        {
            TrendyolApiContext context = new TrendyolApiContext();

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol SupplierId Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol ApiUser Ayarlanmamış");
            }

            if (!item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Any() || string.IsNullOrEmpty(item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault()))
            {
                _logger.Error("Trendyol ApiPassword Ayarlanmamış");
            }

            context.SupplierId = item.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiUser = item.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "";
            context.ApiPassword = item.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "";

            return context;
        }
    }
}
