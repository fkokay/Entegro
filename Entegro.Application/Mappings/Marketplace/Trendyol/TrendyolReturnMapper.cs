using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.ReturnRequest;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolReturnMapper
    {
        private static ILogger _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ReturnRequestDto? ToDto(Content trendyolReturnRequest)
        {
            if (trendyolReturnRequest == null)
            {
                return null;
            }

            TrendyolReturnItemMapper.ConfigureLogger(_logger);

            ReturnRequestDto returnRequest = new ReturnRequestDto();
            returnRequest.CargoProviderName = trendyolReturnRequest.cargoProviderName;
            returnRequest.CargoTrackingNumber = trendyolReturnRequest.cargoTrackingNumber.ToString();
            returnRequest.CargoTrackingLink = trendyolReturnRequest.cargoTrackingLink;
            returnRequest.ClaimDate = FromUnixTimeMilliseconds(trendyolReturnRequest.claimDate);
            returnRequest.LastModifiedDate = FromUnixTimeMilliseconds(trendyolReturnRequest.lastModifiedDate);
            returnRequest.OrderDate = FromUnixTimeMilliseconds(trendyolReturnRequest.orderDate);
            returnRequest.OrderNumber = trendyolReturnRequest.orderNumber;
            returnRequest.CustomerFirstName = trendyolReturnRequest.customerFirstName;
            returnRequest.CustomerLastName = trendyolReturnRequest.customerLastName;
            returnRequest.OrderOutboundPackageId = trendyolReturnRequest.orderOutboundPackageId;
            returnRequest.OrderShipmentPackageId = trendyolReturnRequest.orderShipmentPackageId;
            returnRequest.CreatedOnUtc = DateTime.UtcNow;
            returnRequest.UpdatedOnUtc = DateTime.UtcNow;

            returnRequest.Items = TrendyolReturnItemMapper.ToDtoList(trendyolReturnRequest.items).ToList();

            return returnRequest;
        }

        public static IEnumerable<ReturnRequestDto> ToDtoList(IEnumerable<Content> returnRequests)
        {
            if (returnRequests == null)
                yield break;

            foreach (var returnRequest in returnRequests)
            {
                var dto = ToDto(returnRequest);
                if (dto != null)
                    yield return dto;
            }
        }

        public static DateTime FromUnixTimeMilliseconds(long milliseconds)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            return dateTimeOffset.UtcDateTime;
        }
    }
}
