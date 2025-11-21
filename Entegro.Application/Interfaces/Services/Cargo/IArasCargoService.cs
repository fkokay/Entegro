using ArasCargo;
using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.Interfaces.Services.Cargo
{
    public interface IArasCargoService
    {
        Task SendCargo(ShipmentDto shipmentDto);
        Task<BarcodeResult?> GetBarcode(string integrationCode);
        Task<DispatchResultInfo> CancelDispatch(string integrationCode);
        Task GetCargo(string queryType, string integrationCode);
        Task<GetCargoSearchResponseGetCargoSearchResult?> GetCargoSearch(string seri, string documentNo, string refCode);
        Task<GetCargoSearchByCodeResponseGetCargoSearchByCodeResult?> GetCargoSearchByCode(string code);
    }
}
