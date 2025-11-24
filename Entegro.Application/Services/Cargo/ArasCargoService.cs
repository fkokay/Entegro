using ArasCargo;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;
using Microsoft.Extensions.Logging;
using Polly;


namespace Entegro.Application.Services.Cargo
{
    public class ArasCargoService : IArasCargoService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<ArasCargoService> _logger;
        private readonly IIntegrationSystemService _integrationSystemService;

        public ArasCargoService(IOrderService orderService, ILogger<ArasCargoService> logger, IIntegrationSystemService integrationSystemService)
        {
            _orderService = orderService;
            _logger = logger;
            _integrationSystemService = integrationSystemService;
        }

        private string _username = "";
        private string _password = "";
        public async Task<DispatchResultInfo> CancelDispatch(string integrationCode, int shippingIntegrationId)
        {
            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(shippingIntegrationId);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                ServiceSoapClient service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                return await service.CancelDispatchAsync(_username, _password, integrationCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarcodeResult?> GetBarcode(string integrationCode, int shippingIntegrationId)
        {
            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(shippingIntegrationId);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                return await service.GetBarcodeAsync(_username, _password, integrationCode);
            }
            catch (Exception ex)
            {
                throw new Exception("GetBarcode sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task GetCargo(string queryType, string integrationCode, int shippingIntegrationId)
        {
            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(shippingIntegrationId);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);

                switch (queryType.ToLower())
                {
                    case "info":
                        var info = await service.GetCargoInfoAsync(_username, _password, "", integrationCode);
                        // info burada elde edildi,return edilebilir
                        break;

                    case "transaction":
                        var trx = await service.GetCargoTransactionAsync(_username, _password, "", integrationCode);
                        break;

                    case "bywaybill":
                        var trxByWaybill = await service.GetCargoTransactionByWaybillIdAsync(_username, _password, integrationCode);
                        break;

                    default:
                        _logger.LogError("Geçersiz queryType! info / transaction / bywaybill");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCargo sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task<GetCargoSearchResponseGetCargoSearchResult?> GetCargoSearch(string seri, string documentNo, string refCode, int shippingIntegrationId)
        {
            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(shippingIntegrationId);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var result = await service.GetCargoSearchAsync(_username, _password, seri, documentNo, refCode);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCargoSearch sırasında hata oluştu: " + ex.Message, ex);
                return null;
            }
        }

        public async Task<GetCargoSearchByCodeResponseGetCargoSearchByCodeResult?> GetCargoSearchByCode(string code, int shippingIntegrationId)
        {
            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(shippingIntegrationId);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var result = await service.GetCargoSearchByCodeAsync(_username, _password, code);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCargoSearchByCode sırasında hata oluştu: " + ex.Message, ex);
                return null;
            }
        }

        public async Task SendCargo(ShipmentDto shipmentDto)
        {
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var orderDto = await _orderService.GetOrderByIdAsync(shipmentDto.OrderId);

                var integrationSystem = await _integrationSystemService.GetByIdAsync(shipmentDto.ShippingIntegrationId.Value);
                if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Cargo)
                {
                    _username = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Username").Value;
                    _password = integrationSystem.IntegrationSystemParameters.First(p => p.Key == "Password").Value;
                }
                Order order = new Order();
                order.TradingWaybillNumber = orderDto.OrderNumber;
                order.InvoiceNumber = orderDto.OrderNumber;
                order.IntegrationCode = orderDto.OrderNumber;


                order.UserName = _username;
                order.Password = _password;
                order.ReceiverName = orderDto.Customer.Name;
                order.ReceiverAddress = orderDto.ShippingAddress.Address1;
                order.ReceiverPhone1 = orderDto.Customer.PhoneNumber;
                order.ReceiverCityName = orderDto.ShippingAddress.City;
                order.ReceiverTownName = orderDto.ShippingAddress.Town;
                order.ReceiverDistrictName = "";
                order.ReceiverQuarterName = "";
                order.ReceiverAvenueName = "";
                order.ReceiverStreetName = "";
                order.VolumetricWeight = "";//Desi
                order.Weight = "";//Ürün kg
                order.SpecialField1 = "";
                order.SpecialField2 = "";
                order.SpecialField3 = "";

                if (shipmentDto.IsPaymentDoor)
                {
                    decimal paymentDoorPrice = shipmentDto.ShipmentItems.Sum(x => x.Quantity * x.OrderItem.UnitPrice);
                    order.IsCod = "1";
                    order.CodAmount = paymentDoorPrice.ToString(); //hazırlamış olduğu paket içindeki ürünlerin toplam fiyatı olacak
                    order.CodCollectionType = "0";
                    order.CodBillingType = "0";
                }
                else
                {
                    order.IsCod = "0";
                }

                order.PayorTypeCode = shipmentDto.PaymentType == false ? "1" : "2"; // 1: Gönderici, 2: Alıcı
                order.Description = "";
                order.TaxNumber = order.TaxNumber;
                order.TaxOffice = order.TaxOffice;
                order.IsWorldWide = "0";


                int packageQuantity = shipmentDto.ShipmentItems.Count;
                order.PieceCount = packageQuantity.ToString();
                order.PieceDetails = new PieceDetail[packageQuantity];
                for (int i = 1; i <= packageQuantity; i++)
                {
                    order.PieceDetails[i - 1] = new PieceDetail();
                    order.PieceDetails[i - 1].VolumetricWeight = "1";
                    order.PieceDetails[i - 1].Weight = "";
                    order.PieceDetails[i - 1].BarcodeNumber = orderDto.OrderItems[i - 1].Product.Barcode.ToString();//ürün barkod
                    order.PieceDetails[i - 1].ProductNumber = orderDto.OrderItems[i - 1].Product.Code.ToString();//ürün code
                    order.PieceDetails[i - 1].Description = orderDto.OrderItems[i - 1].Product.Name.ToString();//ürün adı gidecek
                }


                var response = await service.SetOrderAsync(new[] { order }, _username, _password);
                if (response[0].ResultCode == "0")
                {
                    var retryPolicy = Policy.Handle<Exception>()
                        .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogInformation("{RetryCount}. deneme başarısız oldu, {WaitTime} saniye bekleniyor.", retryCount, timeSpan.TotalSeconds);
                        });

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var barcodeResult = await GetBarcode(orderDto.OrderNumber, shipmentDto.ShippingIntegrationId.Value);

                        if (barcodeResult == null)
                        {
                            _logger.Error("Aras Kargo servisi ile iletişim kurulamadı.");
                        }

                        if (barcodeResult.ResultCode == 0)
                        {
                            string printData = "";
                            foreach (var item in barcodeResult.ZebraZpl)
                            {
                                printData += item;
                            }
                            shipmentDto.PrintData = printData;
                        }
                        else
                        {
                            _logger.Error(barcodeResult.Message);
                        }
                    });
                }
                if (response == null || response.Length == 0)
                    _logger.Error("ArasCargo gönderim cevabı boş döndü!");

                if (response[0].ResultCode != "0")
                    _logger.Error($"Aras hata: {response[0].ResultMessage}");

            }
            catch (Exception ex)
            {
                _logger.LogError("SendCargo sırasında hata oluştu: " + ex.Message, ex);
            }
        }
    }
}
