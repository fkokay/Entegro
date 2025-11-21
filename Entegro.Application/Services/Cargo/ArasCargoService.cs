using ArasCargo;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;


namespace Entegro.Application.Services.Cargo
{
    public class ArasCargoService : IArasCargoService
    {
        private readonly IOrderService _orderService;

        public ArasCargoService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private readonly string _username = "feyzan";
        private readonly string _password = "a5w89m7nrf";
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

        public async Task SendCargo(ShipmentDto shipmentDto)
        {
            //shipment içine isDoorPayment eklenmeli ve printData
            //hazırlamış olduğu paket içindeki ürünlerin toplam fiyatı olacak --> paymentDoorPrice
            try
            {
                var service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);
                var orderDto = await _orderService.GetOrderByIdAsync(shipmentDto.OrderId);
                Order order = new Order();

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
                    order.IsCod = "1";
                    order.CodAmount = "0"; //hazırlamış olduğu paket içindeki ürünlerin toplam fiyatı olacak
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

        //public async Task<CargoResultModel> SendCargo(Sipamas siparis, int packageQuantity, string paymentType, bool isPaymentDoor, double paymentDoorPrice, string content)
        //{
        //    try
        //    {
        //        ServiceSoapClient service = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);

        //        Order order = new Order();
        //        order.TradingWaybillNumber = siparis.FATUEK?.ACIK3 + "-" + siparis.FATUEK?.ACIK5;
        //        order.InvoiceNumber = siparis.FATIRS_NO;
        //        order.IntegrationCode = siparis.FATIRS_NO;

        //        if (siparis.SIPAMASSAHAEK == null)
        //        {
        //            order.ReceiverName = siparis.CARI_ISIM;
        //            order.ReceiverAddress = siparis.CARI_ADRES;
        //            order.ReceiverPhone1 = siparis.CARI_TEL;
        //            order.ReceiverPhone2 = siparis.CARI_TEL;
        //            order.ReceiverPhone3 = siparis.CARI_TEL;
        //            order.ReceiverCityName = siparis.CARI_IL;
        //            order.ReceiverTownName = siparis.CARI_ILCE;
        //            order.ReceiverDistrictName = "";//Semt
        //            order.ReceiverQuarterName = "";//Mahalle
        //            order.ReceiverAvenueName = "";//Cadde
        //            order.ReceiverStreetName = "";//Sokak
        //        }
        //        else
        //        {
        //            order.ReceiverName = siparis.SIPAMASSAHAEK.TESLIMAT_ADI + " " + siparis.SIPAMASSAHAEK.TESLIMAT_SOYADI;
        //            order.ReceiverAddress = siparis.SIPAMASSAHAEK.TESLIMAT_ADRES;
        //            order.ReceiverPhone1 = siparis.SIPAMASSAHAEK.TESLIMAT_TEL;
        //            order.ReceiverPhone2 = siparis.CARI_TEL;
        //            order.ReceiverPhone3 = siparis.CARI_TEL;
        //            order.ReceiverCityName = siparis.SIPAMASSAHAEK.TESLIMAT_IL;
        //            order.ReceiverTownName = siparis.SIPAMASSAHAEK.TESLIMAT_ILCE;
        //            order.ReceiverDistrictName = "";//Semt
        //            order.ReceiverQuarterName = "";//Mahalle
        //            order.ReceiverAvenueName = "";//Cadde
        //            order.ReceiverStreetName = "";//Sokak
        //        }

        //        order.VolumetricWeight = "";//Desi
        //        order.Weight = "";//Ürün kg
        //        order.SpecialField1 = "";
        //        order.SpecialField2 = "";
        //        order.SpecialField3 = "";
        //        if (isPaymentDoor)
        //        {
        //            order.IsCod = "1";
        //            order.CodAmount = paymentDoorPrice.ToString();
        //            order.CodCollectionType = "0";
        //            order.CodBillingType = "0";
        //        }
        //        else
        //        {
        //            order.IsCod = "0";
        //        }

        //        order.PayorTypeCode = paymentType == "GÖ" ? "1" : "2"; // 1: Gönderici, 2: Alıcı
        //        order.Description = "";
        //        order.TaxNumber = siparis.VERGI_NUMARASI + siparis.TCKIMLIKNO;
        //        order.TaxOffice = siparis.VERGI_DAIRESI;
        //        order.IsWorldWide = "0";


        //        order.PieceCount = packageQuantity.ToString();//shipmentitem count
        //        order.PieceDetails = new PieceDetail[packageQuantity];
        //        for (int i = 1; i <= packageQuantity; i++)
        //        {
        //            order.PieceDetails[i - 1] = new PieceDetail();
        //            order.PieceDetails[i - 1].VolumetricWeight = "1";
        //            order.PieceDetails[i - 1].Weight = "";
        //            order.PieceDetails[i - 1].BarcodeNumber = siparis.FATIRS_NO + (i < 10 ? "0" + i : i.ToString());//ürün barkod
        //            order.PieceDetails[i - 1].ProductNumber = "";//ürün code
        //            order.PieceDetails[i - 1].Description = content;//ürün adı gidecek
        //        }

        //        var postdata = Serializer.Serialize(order);

        //        CargoResultModel result = new CargoResultModel();

        //        var response = await service.SetOrderAsync([order], username, password);
        //        if (response[0].ResultCode == "0")
        //        {
        //            var retryPolicy = Policy
        //        .Handle<Exception>()
        //        .WaitAndRetryAsync(
        //                retryCount: 3,
        //                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
        //                onRetry: (exception, timeSpan, retryCount, context) =>
        //                {
        //                    Console.WriteLine("{RetryCount}. deneme başarısız oldu, {WaitTime} saniye bekleniyor.", retryCount, timeSpan.TotalSeconds);
        //                });

        //            await retryPolicy.ExecuteAsync(async () =>
        //            {
        //                var barcodeResult = await Barcode(siparis.FATIRS_NO);

        //                if (barcodeResult == null)
        //                {
        //                    throw new Exception("Aras Kargo servisi ile iletişim kurulamadı.");
        //                }

        //                if (barcodeResult.ResultCode == 0)
        //                {
        //                    string printData = "";
        //                    foreach (var item in barcodeResult.ZebraZpl)
        //                    {
        //                        printData += item;
        //                    }
        //                    result.PrintData = printData;
        //                }
        //                else
        //                {
        //                    throw new Exception(barcodeResult.Message);
        //                }
        //            });
        //        }
        //        result.Success = response[0].ResultCode == "0";
        //        result.Message = response[0].ResultMessage;
        //        result.TrackingNumber = "";
        //        result.TrackingLink = "";

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        CargoResultModel result = new CargoResultModel();
        //        result.Success = false;
        //        result.Message = ex.Message;


        //        return result;
        //    }
        //}

    }
}
