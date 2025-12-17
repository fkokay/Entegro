using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.ReturnRequestItem;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public static class TrendyolReturnItemMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        private static ReturnRequestItemDto? ToDto(Item item, Claimitem claim)
        {
            if (item?.orderLine == null || claim == null)
                return null;

            return new ReturnRequestItemDto
            {
                ProductName = item.orderLine.productName,
                Barcode = item.orderLine.barcode,
                MerchantSku = item.orderLine.merchantSku,
                ProductColor = item.orderLine.productColor,
                ProductSize = item.orderLine.productSize,
                Price = (decimal)item.orderLine.price,

                CustomerClaimReasonName = claim.customerClaimItemReason?.name,
                CustomerClaimReasonCode = claim.customerClaimItemReason?.code,

                PlatformClaimReasonName = claim.trendyolClaimItemReason?.name,
                PlatformClaimReasonCode = claim.trendyolClaimItemReason?.code,

                PlatformName = "Trendyol",

                AutoApproveDate = claim.autoApproveDate > 0
                    ? DateTimeOffset.FromUnixTimeMilliseconds(claim.autoApproveDate).DateTime
                    : null,

                Note = claim.note,
                CustomerNote = claim.customerNote,
                Resolved = claim.resolved,
                AcceptedBySeller = claim.acceptedBySeller,

                ReturnRequestStatus = TrendyolStatusMapper.MapReturnStatus(claim.claimItemStatus?.name)
            };
        }


        public static IEnumerable<ReturnRequestItemDto> ToDto(Item item)
        {
            if (item?.claimItems == null)
                yield break;

            foreach (var claim in item.claimItems)
            {
                var dto = ToDto(item, claim);
                if (dto != null)
                    yield return dto;
            }
        }


        public static IEnumerable<ReturnRequestItemDto> ToDtoList(IEnumerable<Item> items)
        {
            if (items == null)
                yield break;

            foreach (var item in items)
            {
                foreach (var dto in ToDto(item))
                    yield return dto;
            }
        }
    }

}
