namespace Entegro.Domain.Enums
{
    public class PazaramaStatusMapper
    {
        public static OrderStatus MapOrderStatus(int pazaramaStatus)
        {
            return pazaramaStatus switch
            {
                3 => OrderStatus.Pending,            // Siparişiniz Alındı
                12 => OrderStatus.Processing,        // Siparişiniz Hazırlanıyor
                13 => OrderStatus.Cancelled,         // Tedarik Edilemedi

                6 => OrderStatus.Cancelled,          // İade Edildi
                11 => OrderStatus.Complete,          // Teslim Edildi

                _ => OrderStatus.Pending             // Bilinmeyen durum
            };
        }

    }
}
