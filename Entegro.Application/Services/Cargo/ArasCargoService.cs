using ArasCargo;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Cargo;


namespace Entegro.Application.Services.Cargo
{
    public class ArasCargoService : IArasCargoService
    {
        private readonly string _username = "KULLANICI_ADI";
        private readonly string _password = "SIFRE";
        public async Task<DispatchResultInfo> CancelDispatch(string integrationCode)
        {
            try
            {
                ServiceSoapClient service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                return await service.CancelDispatchAsync(_username, _password, integrationCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarcodeResult?> GetBarcode(string integrationCode)
        {
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                return await service.GetBarcodeAsync(_username, _password, integrationCode);
            }
            catch (Exception ex)
            {
                throw new Exception("GetBarcode sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task GetCargo(string queryType, string integrationCode)
        {
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);

                switch (queryType.ToLower())
                {
                    case "info":
                        var info = await service.GetCargoInfoAsync(_username, _password, "", integrationCode);
                        // info burada elde edildi, istersen return edebilirsin
                        break;

                    case "transaction":
                        var trx = await service.GetCargoTransactionAsync(_username, _password, "", integrationCode);
                        break;

                    case "bywaybill":
                        var trxByWaybill = await service.GetCargoTransactionByWaybillIdAsync(_username, _password, integrationCode);
                        break;

                    default:
                        throw new Exception("Geçersiz queryType! info / transaction / bywaybill");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCargo sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task<GetCargoSearchResponseGetCargoSearchResult?> GetCargoSearch(string seri, string documentNo, string refCode)
        {
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var result = await service.GetCargoSearchAsync(_username, _password, seri, documentNo, refCode);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("GetCargoSearch sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task<GetCargoSearchByCodeResponseGetCargoSearchByCodeResult?> GetCargoSearchByCode(string code)
        {
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var result = await service.GetCargoSearchByCodeAsync(_username, _password, code);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("GetCargoSearchByCode sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task SendCargo(ShipmentDto shipmentDto, bool isDoorPayment)
        {

            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);


                var order = new Order
                {
                    UserName = _username,
                    Password = _password,
                    ReceiverName = "",
                    ReceiverAddress = "",
                    ReceiverPhone1 = "",
                    ReceiverCityName = "",
                    ReceiverTownName = "",
                    PieceCount = "",
                    Weight = "",
                    VolumetricWeight = "",
                    IntegrationCode = "",
                    Description = "",
                    CodAmount = "",
                    CodBillingType = isDoorPayment ? "1" : "0",
                    CodCollectionType = isDoorPayment ? "1" : "0"
                };


                var response = await service.SetOrderAsync(new[] { order }, _username, _password);

                if (response == null || response.Length == 0)
                    throw new Exception("ArasCargo gönderim cevabı boş döndü!");

                if (response[0].ResultCode != "0")
                    throw new Exception($"Aras hata: {response[0].ResultMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception("SendCargo sırasında hata oluştu: " + ex.Message, ex);
            }
        }
    }
}
