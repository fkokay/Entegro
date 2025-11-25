using ArasCargo;
using Entegro.Application.DTOs.Cargo;
using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.Interfaces.Services.Cargo
{
    public interface IArasCargoService
    {
        Task<ArasSendCargoResultDto> SendCargo(ShipmentDto shipmentDto);
        Task<BarcodeResult?> GetBarcode(string integrationCode, int shippingIntegrationId);
        Task<DispatchResultInfo> CancelDispatch(string integrationCode, int shippingIntegrationId);
        Task GetCargo(string queryType, string integrationCode, int shippingIntegrationId);
        Task<GetCargoSearchResponseGetCargoSearchResult?> GetCargoSearch(string seri, string documentNo, string refCode, int shippingIntegrationId);
        Task<GetCargoSearchByCodeResponseGetCargoSearchByCodeResult?> GetCargoSearchByCode(string code, int shippingIntegrationId);
    }
}
