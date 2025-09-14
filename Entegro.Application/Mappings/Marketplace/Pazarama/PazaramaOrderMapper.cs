using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Pazarama
{
    public static class PazaramaOrderMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(PazaramaOrderDto pazaramaOrder)
        {
            if (pazaramaOrder == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();



            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<PazaramaOrderDto> pazaramaOrders)
        {
            if (pazaramaOrders == null)
                yield break;

            foreach (var pazaramaOrder in pazaramaOrders)
            {
                var dto = ToDto(pazaramaOrder);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
