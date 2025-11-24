using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;
using Quartz;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class SendCargoJob : IJob
    {
        private readonly ILogger<SendCargoJob> _logger;
        private readonly IShipmentService _shipmentService;
        private readonly IArasCargoService _arasCargoService;

        public SendCargoJob(IShipmentService shipmentService, ILogger<SendCargoJob> logger, IArasCargoService arasCargoService)
        {
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
                    _logger.LogInformation("SendCargoJob başlatıldı.");
                    foreach (var shipment in shipments)
                    {
                        if (shipment.ShippingIntegrationId.HasValue && string.IsNullOrEmpty(shipment.PackageNo))
                        {
                            switch (shipment.Carrier)
                            {
                                case "Aras Kargo":
                                    await HandleArasKargoAsync(shipment);
                                    break;

                                case "Yurtiçi Kargo":
                                    await HandleYurticiKargoAsync(shipment);
                                    break;

                                default:
                                    _logger.LogWarning($"Desteklenmeyen kargo tipi: {shipment.Carrier}");
                                    break;
                            }
                        }
                    }
                }

                else
                    _logger.LogInformation("SendCargoJob'da işlenecek gönderi yok");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata  {ex.Message}");
            }
        }

        private async Task HandleArasKargoAsync(ShipmentDto shipment)
        {
            _logger.LogInformation($"Aras Kargo gönderisi hazırlanıyor. ShipmentId: {shipment.Id}");
            await _arasCargoService.SendCargo(shipment);
            _logger.LogInformation($"Aras Kargo gönderisi tamamlandı. ShipmentId: {shipment.Id}");
        }
        private async Task HandleYurticiKargoAsync(ShipmentDto shipment)
        {
            _logger.LogInformation($"Yurtiçi Kargo gönderisi hazırlanıyor. ShipmentId: {shipment.Id}");

        }
    }
}
