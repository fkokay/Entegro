using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;
using Quartz;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ShipmentJob : IJob
    {
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly ILogger<SendCargoJob> _logger;
        private readonly IArasCargoService _arasCargoService;
        public ShipmentJob(ISettingService settingService, IShipmentService shipmentService, ILogger<SendCargoJob> logger, IArasCargoService arasCargoService)
        {
            _settingService = settingService;
            _shipmentService = shipmentService;
            _logger = logger;
            _arasCargoService = arasCargoService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var shipments = await _shipmentService.GetAllAsync();
                if (shipments.Any())
                {
                    _logger.LogInformation("ShipmentJob başlatıldı.");
                    foreach (var shipment in shipments)
                    {
                        if (shipment.ShippingIntegrationId.HasValue && string.IsNullOrEmpty(shipment.PackageNo))
                        {
                            switch (shipment.Carrier)
                            {
                                case "Aras Kargo":
                                    await HandleArasKargoAsync(shipment);
                                    break;

                                default:
                                    _logger.LogWarning($"Desteklenmeyen kargo tipi: {shipment.Carrier}");
                                    break;
                            }
                        }
                    }
                }

                else
                    _logger.LogInformation("ShipmentJob'da işlenecek gönderi yok");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata  {ex.Message}");
            }
        }


        private async Task HandleArasKargoAsync(ShipmentDto shipment)
        {
            _logger.LogInformation($"Aras Kargo gönderisi hazırlanıyor. ShipmentId: {shipment.Id}");
            var result = await _arasCargoService.SendCargo(shipment);

            if (result.Success)
                _logger.LogInformation($"Aras Kargo gönderisi tamamlandı. PrintData: {result.PrintData} Takip Numarası:{result.TrackingNumber}");

            else
                _logger.LogError($"Aras Kargo gönderisi başarısız oldu. Hata Mesajı: {result.Message}");

        }
    }
}
