using ArasCargo;
using Entegro.Application.DTOs.Order;

namespace Entegro.Application.Interfaces.Services.Cargo
{
    public interface IArasCargoService
    {
        Task SendCargo(OrderDto order, bool isDoorPayment);
        Task<BarcodeResult?> GetBarcode(string integrationCode);
        Task<DispatchResultInfo> CancelDispatch(string integrationCode);
        Task GetCargo(string queryType, string integrationCode);
        Task<GetCargoSearchResponseGetCargoSearchResult?> GetCargoSearch(string seri, string documentNo, string refCode);
        Task<GetCargoSearchByCodeResponseGetCargoSearchByCodeResult?> GetCargoSearchByCode(string code);
    }
}
