namespace Entegro.Domain.Enums
{
    public static class TrendyolStatusMapper
    {
        public static OrderStatus MapOrderStatus(string trendyolStatus)
        {
            return trendyolStatus switch
            {
                "Created" or "AwaitingConfirmation" => OrderStatus.Pending,
                "Picking" or "Invoiced" => OrderStatus.Processing,
                "Delivered" => OrderStatus.Complete,
                "Cancelled" => OrderStatus.Cancelled,
                _ => OrderStatus.Pending
            };
        }

        public static ShippingStatus MapShippingStatus(string trendyolStatus)
        {
            return trendyolStatus switch
            {
                // Daha hazırlık aşamasında, henüz kargoya verilmedi
                "Created" or "AwaitingConfirmation" or "Picking" or "Invoiced"
                    => ShippingStatus.NotYetShipped,

                // Kısmi kargo desteği varsa (örneğin bir paketin kısmı gönderilmiş)
                "PartiallyShipped"
                    => ShippingStatus.PartiallyShipped,

                // Kargo firması aldı, yolda
                "Shipped" or "AtCollectionPoint"
                    => ShippingStatus.Shipped,

                // Teslim tamamlandı
                "Delivered"
                    => ShippingStatus.Delivered,

                // Teslim edilemedi ya da iptal
                "UnDelivered" or "Cancelled"
                    => ShippingStatus.NotYetShipped,

                // Eğer hiç kargo gerekmiyorsa (dijital ürün vb.)
                "Digital"
                    => ShippingStatus.ShippingNotRequired,

                _ => ShippingStatus.NotYetShipped
            };
        }


        public static PaymentStatus MapPaymentStatus(string trendyolStatus)
        {
            return trendyolStatus switch
            {
                "Created" or "Picking" or "Invoiced" or "Delivered" => PaymentStatus.Paid,
                "Cancelled" or "UnDelivered" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }

        public static ReturnRequestStatus MapReturnStatus(string trendyolStatus)
        {
            return trendyolStatus switch
            {
                "ReturnRequested" => ReturnRequestStatus.Pending,
                "Returned" => ReturnRequestStatus.Received,
                "ReturnApproved" => ReturnRequestStatus.ReturnAuthorized,
                "Refunded" => ReturnRequestStatus.ItemsRefunded,
                "ReturnRejected" => ReturnRequestStatus.RequestRejected,
                "Cancelled" => ReturnRequestStatus.Cancelled,
                _ => ReturnRequestStatus.Pending
            };
        }
    }
}
